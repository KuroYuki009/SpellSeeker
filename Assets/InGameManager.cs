using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class InGameManager : MonoBehaviour
{
    public SceneTransitionManager sceneTM;
    public PlayableDateManager playableDM;
    
    public GameObject battleAnimationUI_Obj;//�X�^�[�g���ɍĐ�����A�j���[�V�����̃I�u�W�F�N�g���i�[�B
    Animator battleAnima;//animator���擾�A�i�[�B

    public GameObject PCW_Group;//�v���C���[�J�X�^���E�B���h�E�̃O���[�v�B
    public GameObject GSW_Group;//�ėp�Z���N�g�E�B���h�E�̃O���[�v�B
    public GameObject MenuUI_ExtrasGroup;//menuUI���̓����UI�O���[�v�B
    public GameObject PalletUI_Group;//�v���C���[�̃C���Q�[���E�B���h�E�̃O���[�v�B
    CanvasGroup PalletGroup_CanvasGroup;
    public GameObject PtJ_Group;//�W���C���X�v���C�g�̃O���[�v�B

    //Scene������ɏ������s���B
    public SceneProfileDate currentSceneDate;//���݂̃V�[���̗v�f�B

    //switch���Ɏg�p����string�^�B�e�Q�[�����[�h�̏����Ɏg�p�����B
    [SerializeField]string gameModeRoute;
    [SerializeField]bool inBattleSW;//�퓬���������̓�ɒl�B(true���퓬���B)

    //�Q�[�����[�h�ǉ����W���[���B
    public bool shieldBuild_ModeSW = true;//�V�[���h��B��ɒl�B

    public StatusDate mode_conflict_offsetStatusDate;//conflict���̃I�t�Z�b�g(����)�X�e�[�^�X�B

    public List<GameObject> playableObject;//�v���C���[�̃v���C�A�u���I�u�W�F�N�g���i�[����B
    public List<StatusManager> playableStatusManager;//�v���C���[�̃X�e�[�^�X�}�l�[�W���[���i�[����B

    public List<Transform> playerSpawnPoint;//�v���C���[�̃X�|�[���n�_�B
    public Vector3 exclusionSpawnPoint;//�|���ꂽ��Ɉړ�����ꏊ�B

    public GameObject[] beingPlayerObj = new GameObject[4];//�������̃v���C���[�I�u�W�F�N�g���i�[����ׂ̔z��B4�g�𐶐��B
    public List<GameObject> eliminatePlayerObj;//�|���ꂽ�v���C���[�I�u�W�F�N�g���i�[�B
    GameObject depositRoundWinnerObj;//���E���h�ɏ��������v���C���[�I�u�W�F�N�g���ꎞ�i�[����B
    public ParticleSystem eliminateEffect;//���j���ꂽ�ۂɔ�������p�[�e�B�N���B

    public List<Sprite> numberSprites;//���E���h�I�����ɂɏ��������v���C���[�̐�����\������ۂɎg�p����B
    public Image UI_winerNumberSprite;//���E���h�I�����ɕ\������ImageUI�B

    public Image GSW_BackCover_Image;//�I�𒆁A���������Ȃ�����J�o�[�C���[�W�B

    bool slowTimeSW;//���݂��X���[��Ԃ����ɒl�ŕ\���B
    float slowTimeScale = 0.1f;//�ǂꂭ�炢�x���Ȃ邩��ݒ肷��B�f�t�H���g��0.2f;
    float finishSlowTime;

    //���ʉ���
    AudioSource inGameManagerAS;

    public AudioClip battleReadySE;//�퓬���J�n����O�̌��ʉ��B(AreYouReady?�̕ӂ�ŗ����B)
    public AudioClip battleStartSE;//�퓬�J�n���ɍĐ�������ʉ��B(BattleStart�̕ӂ�ŗ����B)
    public AudioClip battleEndE;//�퓬���I�������ۂɍĐ�����鉹���ʉ��B

    //--------

    //���j���[�V���v���r�W���A��
    //SimpleGamemodeSettingManager sgsManager;
    void Start()
    {
        exclusionSpawnPoint = new Vector3(100, 0, 100);
        if (sceneTM == null) sceneTM = GetComponent<SceneTransitionManager>();
        if (playableDM == null) playableDM = GetComponent<PlayableDateManager>();
        battleAnima = battleAnimationUI_Obj.GetComponent<Animator>();
        inGameManagerAS = GetComponent<AudioSource>();
        PalletGroup_CanvasGroup = PalletUI_Group.GetComponent<CanvasGroup>();

        //sgsManager = GetComponent<SimpleGamemodeSettingManager>();

        //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("�}�E�X�J�[�\�����\���ɂ��܂����B");
    }

    void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)//�Q�[���̍ċN���B
        {
            Debug.Log("�ċN������");

            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
            Application.Quit();
        }

        if (inBattleSW == true)
        {
            switch (gameModeRoute)
            {
                //�R���t���N�g�n���[�h�̏���-------------
                case "Conflict_Classic_inBattleProcess":
                    Conflict_Classic_inBattleProcess();
                    break;
                case "Conflict_EndRoundProcess":
                    Conflict_EndRoundProcess();
                    break;
                case "WhatNextGame":
                    WhatNextGame();
                    break;
            }
        }
        else
        {
            switch (gameModeRoute)
            {
                //�Q�[�����[�h�ǉ����W���[���̏���-------------
                case "ShieldBuild_PickStandby"://�I��҂��̏�ԁB
                    ShieldBuild_PickStandby();
                    break;
            }
        }
        

        if (slowTimeSW == true)
        {
            if (finishSlowTime <= 1.5f)
            {
                SlowTimeEffect_in();
                finishSlowTime += 1 * Time.unscaledDeltaTime;

            }
            else if (finishSlowTime >= 1.5f)
            {
                SlowTimeEffect_out();
                slowTimeSW = false;
            }
        }
        else if(slowTimeSW == false && finishSlowTime != 0)
        {
            finishSlowTime = 0f;
        }
    }
    public void SceneEntryProcess()//�ڍs���ɑ����Ă���M���ŏ������J�n�B
    {
        gameModeRoute = null;
        DetectionCurrentMode();//�Q�[�����[�h�����o�B
    }

    public void DetectionCurrentMode()//�Q�[�����[�h�����o�B
    {
        Debug.Log("�Q�[�����[�h�����o���܂�");
        inBattleSW = false;//��xBattleSW�𖳌�������B

        if (currentSceneDate.sceneGameMode == "Menu")
        {
            // ������������ɖ߂��B
            playableDM.maxEntryJoinPlayerInt = 4;

            PalletUI_Group.SetActive(false);
            GSW_Group.SetActive(false);
            GSW_BackCover_Image.enabled = false;
            MenuFunctionRefresh();//���j���[�ɑJ�ڐڑ������ۂɋ@�\�𕜋�������B
        }
        else//����ȊO�̏ꍇ�A
        {
            // ��x�����������|����B
            playableDM.maxEntryJoinPlayerInt = playableDM.joinPlayerInt;

            // �e��UI����x��\���ɂ���B
            PCW_Group.SetActive(false);
            GSW_Group.SetActive(false);
            GSW_BackCover_Image.enabled = false;
            MenuUI_ExtrasGroup.SetActive(false);

            PalletUI_Group.SetActive(true);
            PtJ_Group.SetActive(false);
            for (int c = 0; c < playableDM.playableChara_OBJ.Count; c++)
            {
                playableDM.playableChara_PM[c].CameraSearch();
                PlayerInput pI = playableDM.playableChara_OBJ[c].GetComponent<PlayerInput>();
                pI.currentActionMap = pI.actions.actionMaps[0];//�擾����PlayerInput�̃A�N�V�����}�b�v��0�Ԗ�(Player)�ɕύX�B
            }

        }

        if (currentSceneDate.sceneGameMode == "Conflict_Classic")//�Q�[�����[�h���ΐ탂�[�h�ł���ꍇ�B
        {
            PalletGroup_AlphaHide();//�v���C���[�p���b�g�Q�̃A���t�@�l����x �[���ɂ���B

            if (shieldBuild_ModeSW == true)//�V�[���h�킪�I���ɂȂ��Ă���ꍇ�A
            {
                //�Q�[�����J�n����O�ɃV�[���h�f�b�L��g�܂���B
                ShieldBuild_SetUp();

                PlayerSpawnPoint_Snap();
            }
            else//�V�[���h�킪�I���ɂȂ��Ă��Ȃ��ꍇ�A
            {
                //���̂܂܃o�g���t�F�C�Y�Ɉڍs������B
                for (int i = 0; i < playableDM.playableChara_OBJ.Count; i++)
                {
                    playableDM.playeable_PD[i].onHandDeckDate.Clear();
                }

                Conflict_Classic_SetUp();//�Z�b�g�A�b�v�̏������s���B
                PlayerSpawnPoint_Snap();
                BeforeStartingAnima();// Animation���Đ������A�I�_�̃C�x���g�ɃX�^�[�g������B
            }
        }
    }

    void MenuFunctionRefresh()//���C�����j���[�֖߂�ۂ̑���B
    {
        for (int c = 0; c < playableDM.joinPlayerInt; c++)
        {
            PCW_Group.SetActive(true);
            PtJ_Group.SetActive(true);
            MenuUI_ExtrasGroup.SetActive(true);

            PlayerPreparationInputDate ppID = playableDM.playableChara_PID[c];//�擾�B
            ppID.inputValueRefresh();//���l�̏��������s���B
            ppID.PID_InputSafety = false;

            PlayerInput pI = playableDM.playableChara_OBJ[c].GetComponent<PlayerInput>();
            pI.currentActionMap = pI.actions.actionMaps[1];//�擾����PlayerInput�̃A�N�V�����}�b�v��1�Ԗ�(UI)�ɕύX�B

            Animator animator = playableDM.playableChara_UI_PCW[c].GetComponent<Animator>();
            animator.SetTrigger("OpenWindow_Trigger");
        }
    }

    void PlayerSpawnPoint_Snap()//�v���C���[�X�|�[���n�_�Ɉړ�������B
    {
        //�e�v���C���[�̐ݒ肪�I�������ɃX�|�[���ʒu��ύX�B
        GameObject[] spawnPointObj = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");//�X�e�[�W��̃X�|�[���n�_��T���B
        for (int i = 0; i < spawnPointObj.Length; i++)//�������X�|�[���n�_�ƂȂ�I�u�W�F�N�g��Transform�ɕϊ����i�[�B
        {
            playerSpawnPoint.Add(spawnPointObj[i].GetComponent<Transform>());
        }

        int joinPlayerCount = playableDM.playableChara_OBJ.Count;
        List<GameObject> playerObj = playableDM.playableChara_OBJ;

        for (int c = 0; c < joinPlayerCount; c++)//�X�|�[���n�_�������_���ɐU�蕪����B���̏ꍇ�ݒ肳�ꂽ�X�|�[���n�_�̓�����g�p�����B
        {
            playerObj[c].transform.position = spawnPointObj[c].transform.position;
            playerObj[c].transform.rotation = spawnPointObj[c].transform.rotation;
        }
    }

    void BeforeStartingAnima()//�퓬�J�n�O�̃A�j���[�V�������Đ��B(�J�n�O�J�E���g�_�E���Ȃ�)
    {
        battleAnimationUI_Obj.SetActive(true);

        int BSC_T_ParamHash = Animator.StringToHash("BattleStartCutIn_Trigger");
        battleAnima.SetTrigger(BSC_T_ParamHash);

    }

    public void BattleReadySE_Play()//�Q�[���J�n�O�̃T�E���h�Đ����s���ׂ̃��]�b�g�B
    {
        inGameManagerAS.PlayOneShot(battleReadySE);//������se�𗬂��B
    }
    public void BattleStartSE_Play()//�J�n���̃T�E���h�Đ����s���ׂ̃��]�b�g�B
    {
        inGameManagerAS.PlayOneShot(battleStartSE);//�����ŊJ�n��SE���Đ�������B
    }
    public void BattleEndSE_Play()//�퓬���I�������ۂɃT�E���h�Đ����s���ׂ̃��]�b�g�B
    {
        inGameManagerAS.PlayOneShot(battleEndE);
    }

    public void StartBattlePhase()////UI���Đ����A�I�������m������o�g���t�F�C�Y���J�n������ꍇ�ɓǂݍ��܂���B
    {
        for (int i = 0;i < playableDM.joinPlayerInt;i++)
        {
            Debug.Log("�o�g���t�F�C�Y�ڍs"+i); 
            //StartusManager��SM_ProcessSafety���I�t�ɂ���B
            playableDM.playableChara_SM[i].SM_ProcessSafety = false;
            //PlayerMoving�̃X�N���v�g�G�i�u�����I���ɂ���B���̎��ɃJ�������n���B
            PlayerMoving pm = playableDM.playableChara_PM[i];
            pm.enabled = true;
            GameObject mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");//
            pm.visCamera = mainCamObj.GetComponent<Camera>();
            playableDM.playableChara_PM[i].enabled = true;
            //HandCardManager��HCM_ProcessSafety���I�t�ɂ���B
            playableDM.playableChara_HCM[i].HCM_ProcessSafety = false;

            StartCoroutine(PalletGroup_AlphaFadeIn());

            int playerTeam = playableObject[i].GetComponent<PlayingData>().teamNumber;
            if (playerTeam == 0)
            {
                playableObject[i].tag = "Player_1";
                playableObject[i].layer = 6;
            }
            else if (playerTeam == 1)
            {
                playableObject[i].tag = "Player_2";
                playableObject[i].layer = 7;
            }
            else if(playerTeam == 2)
            {
                playableObject[i].tag = "Player_3";
                playableObject[i].layer = 8;
            }
            else if (playerTeam == 3)
            {
                playableObject[i].tag = "Player_4";
                playableObject[i].layer = 9;
            }
            else
            {
                //playableDM.playableChara_OBJ[i].layer = LayerMask.GetMask(new string[] { "Player_1" });
                Debug.Log("�v���C���[��PlayingData�Ƀ`�[�����w�肳��Ă��܂���B");
            }

            playableStatusManager[i].this_Eliminated = false;//���炩���ߌ��j��Ԃ�����������B

        }

        inBattleSW = true;//�퓬���ɂ���B
        gameModeRoute = "Conflict_Classic_inBattleProcess";
    }

    ////�e�Q�[�����[�h�����B
    
    //���[�h_Conflict_Classic ------------------------
    void Conflict_Classic_SetUp()//�R���t���N�g�E�N���V�b�N�̃Z�b�g�A�b�v�����B�I���ƁA���̏����Ɉڍs�B
    {
        playableObject = playableDM.playableChara_OBJ;//�v���C�A�u���f�[�^�}�l�\�W���[����v���C�A�u��OBJ���i�[����B
        playableStatusManager = playableDM.playableChara_SM;//�v���C�A�u���f�[�^�}�l�\�W���[����v���C�A�u��SM���i�[����B

        for (int i = 0; i < playableDM.joinPlayerInt; i++)
        {
            HandCardManager hcm = playableDM.playableChara_HCM[i];//HandCardManager���i�[�B
            //hcm.SetUp();
            hcm.switchRoot = "SetUp";

            //�A�N�V�����}�b�v�̕ύX
            PlayerInput pI = playableDM.playableChara_OBJ[i].GetComponent<PlayerInput>();
            pI.currentActionMap = pI.actions.actionMaps[0];//�擾����PlayerInput�̃A�N�V�����}�b�v��1�Ԗ�(Player)�ɕύX�B

            StatusManager sm = playableDM.playableChara_SM[i];
            sm.maxHitPoint = mode_conflict_offsetStatusDate.maxHitPoint;//�ő�̗͂̐ݒ�B
            sm.hitPoint = mode_conflict_offsetStatusDate.maxHitPoint;//���ݑ̗͒l���ő�̗͂̐��l�ő��������B
            sm.maxManaPoint = mode_conflict_offsetStatusDate.maxManaPoint;//�ő�}�i���̐ݒ�B
            sm.manaPoint = 0;
            sm.manaWantTime = mode_conflict_offsetStatusDate.manaChargeWantTime;//�}�i�`���[�W�ɕK�v�Ȏ��Ԃ̐ݒ�B
            sm.manaProgressTime = 0;
            sm.maxSpeed = mode_conflict_offsetStatusDate.maxSpeed;//�ő�ړ����x�̐ݒ�B
            sm.moveSpeed = mode_conflict_offsetStatusDate.moveSpeed;//�ړ����̑��x�ݒ�B

            beingPlayerObj[i] = playableObject[i];//�v���C�A�u���I�u�W�F�N�g���琶�����̃v���C���[�Ɋi�[����B
        }
    }

    void Conflict_Classic_inBattleProcess()//�����ȊO�S���G�̃t���[�t�H�[�I�[���B�Ō�܂Ő������Ă���҂������B
    {
        if (beingPlayerObj[0] != null)
        {
            if (playableStatusManager[0].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[0].enabled = true;//���jCover��\���ɂ���B
                eliminatePlayerObj.Add(beingPlayerObj[0]);
                beingPlayerObj[0] = null;

                PlayableInvalidation(0);//�Ώۂ̃v���C�A�u���𖳌����B
                FinishFlagConfirmation();
            }
        }

        if (beingPlayerObj[1] != null)
        {
            if (playableStatusManager[1].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[1].enabled = true;//���jCover��\���ɂ���B
                eliminatePlayerObj.Add(beingPlayerObj[1]);
                beingPlayerObj[1] = null;

                PlayableInvalidation(1);//�Ώۂ̃v���C�A�u���𖳌����B
                FinishFlagConfirmation();

            }
        }

        if (beingPlayerObj[2] != null)
        {
            if (playableStatusManager[2].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[2].enabled = true;//���jCover��\���ɂ���B
                eliminatePlayerObj.Add(beingPlayerObj[2]);
                beingPlayerObj[2] = null;

                PlayableInvalidation(2);//�Ώۂ̃v���C�A�u���𖳌����B
                FinishFlagConfirmation();
            }
        }

        if (beingPlayerObj[3] != null)
        {
            if (playableStatusManager[3].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[3].enabled = true;//���jCover��\���ɂ���B
                eliminatePlayerObj.Add(beingPlayerObj[3]);
                beingPlayerObj[3] = null;

                PlayableInvalidation(3);//�Ώۂ̃v���C�A�u���𖳌����B
                FinishFlagConfirmation();
            }
        }
    }

    void PlayableInvalidation(int player)//�w�肵���ԍ��̃v���C�A�u���̖������B
    {
        //�����Ő퓬���Ɏg�p�����@�\�����S�ɏ�����������K�v������B
        //StartusManager��SM_ProcessSafety���I���ɂ���B
        GameObject paOb = playableDM.playableChara_OBJ[player];

        //�v���C�A�u���̕\���̖������B
        paOb.GetComponent<Collider>().enabled = false;//�R���C�_�[�̖������B
        paOb.transform.GetChild(0).gameObject.SetActive(false);//���f���������ڌQ�𖳗͉��B
        paOb.transform.GetChild(1).gameObject.SetActive(false);//�J�[�\���Q�𖳗͉��B

        Instantiate(eliminateEffect, paOb.transform.position, transform.rotation);//���j���ꂽ�ۂɕ`�ʂ����G�t�F�N�g�𐶐�����B

        playableDM.playableChara_SM[player].SM_ProcessSafety = true;
        playableDM.playableChara_SM[player].this_Eliminated = false;//���j��Ԉ��������I�t�ɂ���B
                                                               //PlayerMoving�̃X�N���v�genable���I�t�ɂ���B���̎��ɃJ�������n���B
        PlayerMoving pm = playableDM.playableChara_PM[player];
        pm.enabled = false;
        GameObject mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");//�����͂��炩���߃G���[��h�����߂ɃJ��������点�Ă���B
        pm.visCamera = mainCamObj.GetComponent<Camera>();
        playableDM.playableChara_PM[player].enabled = false;

        HandCardManager hcm = playableDM.playableChara_HCM[player];

        //HandCardManager��HCM_ProcessSafety���I���ɂ���B
        hcm.HCM_ProcessSafety = true;
        //HandCardManager�̂��̑��������B
        hcm.lockOnTarget = null;
        hcm.lineRenderer.enabled = false;
        //HandCardManager��swichRoot�����l���Z�b�g�A�b�v�ɕύX����B
        hcm.switchRoot = "SetUp";
        hcm.HundWindowRefresh();//�\�L�̍X�V�B
    }

    void FinishFlagConfirmation()//���E���h�����̃t���O�𖞂����Ă��邩�̊m�F�B
    {
        int aliveObj = 0;
        for (int ic = 0;ic < beingPlayerObj.Length;ic++)
        {
            if(beingPlayerObj[ic] != null)
            {
                aliveObj++;
                depositRoundWinnerObj = beingPlayerObj[ic];
            }
        }

        if(aliveObj >= 1 && aliveObj == 1)//���������c�����v���C���[����l�ɂȂ��Ă����ꍇ�B
        {
            slowTimeSW = true;

            StartCoroutine(PalletGroup_AlphaFadeOut());

            //Animation�Đ��B
            battleAnimationUI_Obj.SetActive(true);
            //int BEC_T_ParamHash = Animator.StringToHash("Break_BattleEndCutIn_Trigger");
            battleAnima.SetTrigger("Break_BattleEndCutIn_Trigger");
            Conflict_Classic_RoundFinish(depositRoundWinnerObj);
        }
    }

    

    void Conflict_Classic_RoundFinish(GameObject winnerObj)//���E���h�I�������B
    {
        string winnerName = null;
        if(winnerObj.GetComponent<PlayingData>() != null)
        {
            PlayingData winerPD = winnerObj.GetComponent<PlayingData>();
            if(winerPD.playerNumber == 0)
            {
                winnerName = "P1";
                UI_winerNumberSprite.sprite = numberSprites[0];
            }
            else if(winerPD.playerNumber == 1)
            {
                winnerName = "P2";
                UI_winerNumberSprite.sprite = numberSprites[1];
            }
            else if (winerPD.playerNumber == 2)
            {
                winnerName = "P3";
                UI_winerNumberSprite.sprite = numberSprites[2];
            }
            else if (winerPD.playerNumber == 3)
            {
                winnerName = "P4";
                UI_winerNumberSprite.sprite = numberSprites[3];
            }
            else
            {
                winnerName = "404";
            }

            Debug.Log("����" + winnerName);

            Invoke("Conflict_EndRoundProcess",10f);
        }
    }

    public void Conflict_EndRoundProcess()//���E���h�I�����ɔ��������鏈���B
    {
        CancelInvoke();

        if(shieldBuild_ModeSW == true)
        {
            for (int i = 0; playableDM.joinPlayerInt > i; i++)
            {
                playableDM.PlayeableChara_GSW[i].selectEndSW = false;
            }
        }

        for (int i = 0; i < playableDM.joinPlayerInt; i++)
        {
            //�����Ő퓬���Ɏg�p�����@�\�����S�ɏ�����������K�v������B
            Debug.Log("�Z�[�t���[�h�ڍs" + i);

            GameObject paOb = playableDM.playableChara_OBJ[i];

            //StartusManager��SM_ProcessSafety���I���ɂ���B
            playableDM.playableChara_SM[i].SM_ProcessSafety = true;
            playableDM.playableChara_SM[i].this_Eliminated = false;//���j��Ԉ��������I�t�ɂ���B
            //PlayerMoving�̃X�N���v�g�G�i�u�����I�t�ɂ���B���̎��ɃJ�������n���B
            PlayerMoving pm = playableDM.playableChara_PM[i];
            pm.enabled = false;
            GameObject mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");//�����͂��炩���߃G���[��h�����߂ɃJ��������点�Ă���B
            pm.visCamera = mainCamObj.GetComponent<Camera>();
            playableDM.playableChara_PM[i].enabled = false;
            playableDM.playableCharaUI_DestroyCover[i].enabled = false;//���jCover���\���ɂ���B

            //�v���C�A�u���̕\��������L����������B
            paOb.GetComponent<Collider>().enabled = true;//�R���C�_�[�̗L�����B
            paOb.transform.GetChild(0).gameObject.SetActive(true);//���f���������ڌQ��L�����B
            paOb.transform.GetChild(1).gameObject.SetActive(true);//�J�[�\���Q��L�����B

            HandCardManager hcm = playableDM.playableChara_HCM[i];
            //HandCardManager��swichRoot�����l���Z�b�g�A�b�v�ɕύX����B
            hcm.switchRoot = "SetUp";
            //HandCardManager��HCM_ProcessSafety���I���ɂ���B
            hcm.HCM_ProcessSafety = true;
            //HandCardManager�̂��̑��������B
            hcm.lockOnTarget = null;
            hcm.lineRenderer.enabled = false;
            hcm.HundWindowRefresh();//�\�L�̍X�V�B
        }

        gameModeRoute = "WhatNextGame";
    }
    ////���[�h�R���|�[�l���g�����B

    //�ǉ����W���[��_ShieldBuild ------------------------
    void ShieldBuild_SetUp()//
    {
        GSW_Group.SetActive(true);

        GSW_BackCover_Image.enabled = true;//�w�i�J�o�[���I���ɂ���B

        for (int c = 0; c < playableDM.playableChara_OBJ.Count; c++)
        {
            //actionmap�̕ύX�B
            PlayerInput pI = playableDM.playableChara_OBJ[c].GetComponent<PlayerInput>();
            pI.currentActionMap = pI.actions.actionMaps[2];//�擾����PlayerInput�̃A�N�V�����}�b�v��3�Ԗ�(Player)�ɕύX�B

            if(playableDM.playeable_PD[c].onHandDeckDate.Count != 0)
            {
                playableDM.playeable_PD[c].onHandDeckDate.Clear();//�f�b�L�f�[�^���폜�B
            }

            //Pick�̐\���B
            playableDM.playeableChara_Adopt_Card[c].AddCard_Conflict_Shield(3,20);//(giveValue,rollcount)//3�����ꖇ��20��Pick������B
            //Debug.Log("�J�[�h�I��\��");
        }
        gameModeRoute = "ShieldBuild_PickStandby";

    }
    void ShieldBuild_PickStandby()
    {
        //Debug.Log("�I�����ؒ�..............�B");

        int joinPlayerInt = playableDM.joinPlayerInt;
        int pickPlayerCount = 0;

        for(int i = 0;joinPlayerInt > i;i++)
        {
            if (playableDM.PlayeableChara_GSW[i].selectEndSW == true)
            {
                pickPlayerCount++;
            }
        }
        
        if (joinPlayerInt <= pickPlayerCount)
        {
            //�S�����I�����I������B
            Debug.Log("�S���I���I���܂����B");
            for (int i = 0; joinPlayerInt > i; i++)
            {
                playableDM.PlayeableChara_GSW[i].FallWindow();//�A�j���[�V�����̍Đ����s���B
            }

            GSW_BackCover_Image.enabled = false;//�w�i�J�o�[���I�t�ɂ���B

            Conflict_Classic_SetUp();//�Z�b�g�A�b�v�̏������s���B
            BeforeStartingAnima();//Animation���Đ������A�I�_�̃C�x���g�ɃX�^�[�g������B
            gameModeRoute = null;
        }
        
    }

    //------------------------------

    public void WhatNextGame()//�Q�[����Finish������A���Ɉڍs����v�f�B
    {
        //���݂̃Q�[�����[�h���r���A��������V�������E���h�A�X�e�[�W�A���C�����j���[�ւ̋A�҂ւ̏������s���B

        sceneTM.loadStageNamber = 0;
        sceneTM.StartLoad();
        gameModeRoute = null;
    }

    //���o�p
    void SlowTimeEffect_in()//���Ԃ��������ɂȂ�B�@//����͌����������ۂɎg�p����邱�Ƃ�z�肵�Ă���B
    {
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
    void SlowTimeEffect_out()//���Ԃ����ʂ�ɂȂ�B�@//����͌����������ۂɎg�p����邱�Ƃ�z�肵�Ă���B
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    ////�v���C���[�p���b�g�Q�̕\���E��\���̉��o�p�̊֐������B
    //
    void PalletGroup_AlphaHide()//�v���C���[�p���b�g�̃O���[�v�Q���A���t�@�l����0�ɂ���B
    {
        PalletGroup_CanvasGroup.alpha = 0;
    }

    IEnumerator PalletGroup_AlphaFadeIn()//�v���C���[�p���b�g�̃O���[�v�Q�����X�Ƀt�F�[�h�A�E�g������B
    {
        PalletGroup_CanvasGroup.alpha = 0;
        float downTime = 0;

        while (downTime <= 1)
        {
            yield return new WaitForSecondsRealtime(0.02f);// 0.02�b�҂��܂��B
            
            downTime += 0.1f;
            PalletGroup_CanvasGroup.alpha = downTime;
        }
        yield break;
    }

    IEnumerator PalletGroup_AlphaFadeOut()//�v���C���[�p���b�g�̃O���[�v�Q�����X�Ƀt�F�[�h�C������B
    {
        PalletGroup_CanvasGroup.alpha = 1;
        float downTime = 1;

        while (downTime >= 0)
        {
            yield return new WaitForSecondsRealtime(0.02f);// 0.02�b�҂��܂��B

            downTime -= 0.1f;
            PalletGroup_CanvasGroup.alpha = downTime;
        }
        yield break;
    }
}
