using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class HandCardManager : MonoBehaviour
{
    [Header("�f�b�L�f�[�^")]//---
    #region
    [Tooltip("�ҏW�ς݂̃f�b�L�f�[�^���w���B")]
    public DeckData deckInst;// �ҏW�ς݂̃f�b�L�f�[�^�B

    [Tooltip("���݂̃f�b�L�f�[�^�B")]
    public List<SpellData> deckCards;//���݂̃f�b�L�f�[�^�B
    [HideInInspector]public List<SpellData> depositDeck;//�V���t������ۂɈ�x�f�b�L�f�[�^���L���b�V������ׂ̊i�[�`�B

    [Tooltip("���݂̎�D�ɂ���J�[�h")]
    public List<SpellData> handCards;//��D
    #endregion


    [Header("�n�_")]//---
    #region
    [Tooltip("�J�[�h���g�p�������� �������s���ׂ̎n�_�ƂȂ���W�� �I�u�W�F�N�g�Ŏw�肷��B")]
    public GameObject instPos;//�v���n�u�̐����n�_�Ƃ��Ďg�p������W�I�u�W�F�N�g�B
    #endregion


    [Header("�n���h�P�[�X �A�j���֌W")]//---
    #region
    [Tooltip("�n���h�P�[�X�� �A�j������Ɏg�p����� �A�j���[�^�[�ł��B")]
    public Animator handAnimator;//�n���h�P�[�X�̃A�j���[�V�����B(UI��Pallet���Ɋ܂܂��Q�[���I�u�W�F�N�gActionAnima)

    [Tooltip("�����[�h�E�h���[���� �\�������� �c��N�[���^�C���̃e�L�X�g�B")]
    public Text cooltime_Text;//�N�[���^�C���̃e�L�X�g�B

    [Tooltip("�����[�h�E�h���[���� �\�������� ���o�I�Ȏc�莞�Ԃ̉��o�C���[�W�摜�B")]
    public Image cooltime_FillImage;//�N�[���^�C���̓h��Ԃ��B

    [Space]

    [Tooltip("�L���X�g�p�l���̃A�j������Ɏg�p����� �A�j���[�^�[�ł��B")]
    public Animator castPanelAnimator;//�n���h�P�[�X�̃L���X�g�p�l���̃A�j������A�j���[�^�[�B(UI��Pallet���Ɋ܂܂��Q�[���I�u�W�F�N�gCastPanelAnima)

    [Tooltip("�L���X�g�p�l���ɓ����Ă���J�[�h�� ���O ��\������e�L�X�g�ł��B")]
    public Text castCardNameText;//�L���X�g�p�l���ɓ����Ă���J�[�h�̖��O�̕\������B

    [Tooltip("�L���X�g�p�l���ɓ����Ă���J�[�h�� �_���[�W�l ��\������e�L�X�g�ł��B")]
    public Text castCardDamageText;//�L���X�g�p�l���ɓ����Ă���J�[�h�̃_���[�W�l�̕\���B

    [Space]

    [Tooltip("�e�}�O�p�l���� GUIImage����Ɏg�p����� �X�N���v�g�B")]
    public List<PickCard> pickWindow;// �n���h�P�[�X�̕`�ʗpGUIImage�B

    [Tooltip("�J�[�h�}�O����̍ۂ� �\�������� None�J�[�h�f�[�^")]
    public SpellData noneCardData;// ��\���p�̃J�[�h�f�[�^�B

    [Tooltip("�L���X�g�p�l�����̃J�[�h�����p�ł��Ȃ��ۂɕ\��������킹�J�o�[�摜�B")]
    public Image CastCardCover;//�J�[�h���g�p�ł��Ȃ��ꍇ�ɕ\������J�o�[�摜�B

    [Tooltip("�L���X�g�p�l�������}�i�\���� �A�j������Ɏg�p����� �A�j���[�^�[�ł��B")]
    public Animator manaCostWindow_0_Animator;//�L���X�g�p�l�������̃}�i�\���E�B���h�E��Animator�R���|�[�l���g�B

    [Tooltip("�e�}�O�p�l���� ���݂��Ă�e�J�[�h�̃R�X�g��\��������ׂ� �e�L�X�g�Q�ł��B")]
    public List<Text> manaCostText;//�n���h�P�[�X�̊e�J�[�h�̃R�X�g��`�ʂ���B

    [Tooltip("�f�b�L�� �c��J�[�h������ �\������e�L�X�g�ł��B")]
    public Text deckCountText;//�f�b�L�Ɋ܂܂��J�[�h�c��
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�n���h�P�[�X�� �����[�h�E�h���[���� �ŏ��̓���� �Đ������T�E���h�ł��B")]
    public AudioClip ac_HandCase_Reload_Start;

    [Tooltip("�n���h�P�[�X�� �����[�h�E�h���[���� �ҋ@���Ă���� �Đ�����郋�[�v�T�E���h�ł��B")]
    public AudioClip ac_HandCase_Reload_Charge;

    [Tooltip("�n���h�P�[�X�� �����[�h�E�h���[���� �Ō�̕߂� �Đ������T�E���h�ł��B")]
    public AudioClip ac_HandCase_Reload_End;


    [Tooltip("�J�[�h�̔j������ �Đ������T�E���h�ł��B")]
    public AudioClip cardFoldSE;


    [Tooltip("���b�N�I���� �ߑ����� �Đ������T�E���h�ł��B")]
    public AudioClip lockOn_InSE;

    [Tooltip("���b�N�I���� �������� �Đ������T�E���h�ł��B")]
    public AudioClip lockOn_OutSE;
    #endregion

    ////--------------------------------------------------

    StatusManager statusManager;//�X�N���v�g�uStatusManager�v�i�[�p�B
    PlayingData playingData;//�X�N���v�g�uplayingData�v�i�[�p
    PlayerMoving playerMoving;//�X�N���v�g�uplayerMoving�v�i�[�p

    [HideInInspector] public LineRenderer lineRenderer;
    AudioSource audioSource;


    [HideInInspector] public string switchRoot;

    //���̃X�N���v�g�̃Z���t�e�B(false�ŉ������)
    [HideInInspector] public bool HCM_ProcessSafety;
    
    bool standbySW;//�����������ۂ��B

    [HideInInspector] public int deckCount;//���݂̃f�b�L�J�[�h�������i�[����ϐ��B
    int handCard_max = 5;//��D����B���󂱂̐��l��ύX���鎖�͂ł��܂���B


    //�N�[���^�C���֌W
    #region
    [HideInInspector] public float drawCT;//���݂̃N�[���^�C���l�B
    [HideInInspector] public float drawCT_max;//�h���[�ɔ�������N�[���^�C���̒����B

    [HideInInspector] public float deckReloadCT;//���݂̃N�[���^�C���l�B
    [HideInInspector] public float deckReloadCT_max;//�f�b�L�����[�h�ɔ�������N�[���^�C���̒����BbasisTime��sumTime�����킹�����v�l������Ɋi�[���Ďg�p����Z�i�B
    [HideInInspector] public float deckReloadCT_basisTime;//�f�b�L�����[�h�ɔ�������N�[���^�C���̊�b���l�B
    [HideInInspector] public float deckReloadCT_sumTime;//�g�p�����J�[�h�ɍ��킹�������鎞�Ԓl�����炩���ߊi�[����B
    #endregion


    //���b�N�I���V�X�e���n
    #region
    [HideInInspector] public GameObject lockOnTarget;//���b�N�I�������I�u�W�F�N�g���i�[�B
    StatusManager TargetSM;//���b�N�I�������I�u�W�F�N�g��StatusManager�B

    bool canlockOnSW;//���b�N�I���ł��邩���ɒl�ŕ\���B
    float lockOnDistance;//���b�N�I���\�����B
    float cantLockOnTime;//���b�N�I�����N�[���^�C���B
    float lockOnOutTime;//���b�N�I���ێ����ԁB���b�N�I�����ɕǉz���ɂȂ����ۂ̉����܂ł̎��ԁB

    float lineSize;//���݂̃��b�N�I�����C���̉����B
    float lockOnLineInstPoint;//���b�N�I�����ɕ`�ʂ�����̕\�����鍂���B
    #endregion


    ////--------------------------------------------------

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        playingData = GetComponent<PlayingData>();
        playerMoving = GetComponent<PlayerMoving>();
        audioSource = GetComponent<AudioSource>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();//�R���|�\�l���g���擾�B

        switchRoot = "SetUp";
    }

    void Update()
    {
        if(HCM_ProcessSafety == false)
        {
            switch (switchRoot)
            {

                case "SetUp"://�������s���O�ɔ��������鏈���B
                    SetUp();
                    break;


                case "HandCard_Cast":// �J�[�h�g�p
                    HandCard_Cast_Process();
                    break;

                case "HandCard_Trash":// �J�[�h�j��
                    HandCard_Trash_Process();
                    break;

            

                case "SetUpDeckCharge":// �v���C���[���r���h�����f�b�L�f�[�^���f�b�L�ɓ���鏈���B
                    Debug.Log("�f�b�L�`���[�W");
                    break;

                case "DeckShuffle":// �f�b�L�̃V���b�t�����s������
                    DeckShuffle();// �f�b�L�V���b�t�����J�n����B
                    break;

                case "MixingDeckCharge":// �V���b�t�������f�b�L��{�f�b�L�ɕ�[���s������
                    deckCards = new List<SpellData>(depositDeck);
                    depositDeck.Clear();
                    switchRoot = "CardDraw";
                    break;

                case "CardDraw":// �{�f�b�L����J�[�h�����o���A��D�ɓ����B
                    HandDraw();
                    break;

                case "HundFalldToLoad"://��D�����ׂĔj�����A�N�[���^�C���𔭐����������CardDrow�Ɉڍs����B
                    HundFalldToLoad();
                    break;


                case "DrawCoolTime"://�h���[���ɔ������鎞�Ԍo�ߏ����B
                    DrowingTime();
                    break;

                case "DeckReloadingTime"://�f�b�L�����[�h���ɔ������鎞�Ԍo�ߏ����B
                    DeckReloadingTime();
                    break;

                default:
                    Standby();
                    break;
            }

            LockOnProcessing();//���b�N�I���̏������s���B
        }
    }

    public void SetUp()// Start�ŏ���������e�B
    {
        Debug.Log("�Z�b�g�A�b�v���Ă܂�");
        drawCT_max = 1.2f;�@�@�@�@�@�@// �h���[�ɕK�v�Ȏ��Ԃ�1.2�b�ɐݒ�B
        deckReloadCT_basisTime = 3.0f;// �����[�h�ɕK�v�Ȏ��Ԃ�3.0�b�ɐݒ�B

        lockOnDistance = 30.0f;// ���b�N�I���ł��鋗����ݒ�B

        //�������B
        deckReloadCT_sumTime = 0;
        cooltime_Text.enabled = false;
        cooltime_FillImage.enabled = false;
        lockOnLineInstPoint = (gameObject.transform.localScale.y * -0.9f) + gameObject.transform.position.y;//���b�N�I�����̕`�ʐ��̍�����ݒ�B
 
        //�f�b�L�f�[�^������B
        if (playingData.onHandDeckDate.Count == 0)
        {
            //
            deckCards = new List<SpellData>(deckInst.spells);
        }
        else
        {
            deckCards = new List<SpellData>(playingData.onHandDeckDate);
        }

        lineRenderer.enabled = false;//�R���|�[�l���g�̖������B

        depositDeck.Clear();
        handCards.Clear();

        HundWindowRefresh();//�\�L�̍X�V�B

        switchRoot = "DeckShuffle";
    }
    

    void Standby()// �v���C���[�̓��͂��󂯕t����B
    {
        if (standbySW == false) standbySW = true;//�X�^���o�C��ԂɂȂ�B

        // �s�b�N�E�B���h�E��̃J�[�h������R�X�g�𖞂����Ă���ꍇ�J�o�[�摜���\���ɂ���B
        if (statusManager.manaPoint >= handCards[0].manaCost && CastCardCover.enabled == true) CastCardCover.enabled = false;
        else if (statusManager.manaPoint < handCards[0].manaCost && CastCardCover.enabled == false) CastCardCover.enabled = true;

        if (handCards[0] == noneCardData)//�n���h�P�[�X���̃J�[�h����0�ꍇ�B
        {
            standbySW = false;// �X�^���o�C��Ԃ𖳌�������B
            switchRoot = "HundFalldToLoad";// ��D�̎�O��noneCardDate�������ꍇ�A�h���[���s���B
        }
    }


    void DeckShuffle()//�f�b�L�V���b�t��
    {
        deckCount = deckCards.Count;// �ꎞ�f�b�L���� �����J�E���g���擾�B

        while (deckCount > 0)// �V���b�t���p��List����ɂȂ�܂ŏ������s���B
        {
            int rundamInt = Random.Range(0, deckCount);// �^�������� ����������o���B�ő�l�� ���̃f�b�L�Ɋ܂܂��J�[�h�����B

            depositDeck.Add(deckCards[rundamInt]);//---// ����o���������̗v�f�Ɋ܂܂��J�[�h�� �V���b�t���p��List���� �C���Q�[���p��List�ɓn���B
            deckCards.RemoveAt(rundamInt);//-----------// �����āA�V���b�t���p�̃��X�g����J�[�h�����O����B

            deckCount = deckCards.Count;// �����J�E���g���X�V�B
        }

        if (deckCount <= 0) switchRoot = "MixingDeckCharge";// �����J�E���g�l��0�ɂȂ�܂ŃV���b�t���������I�������A���̏���"MixingDeckCharge"�Ɉڍs����B
    }
    

    public void HundWindowRefresh()// ��D�̍ēǂݍ��݁B����ɂ�荡�����Ă����D�̏�񂪍X�V�����B
    {
        for (int i = 0; i < handCards.Count; i++)// ��D�̖����� �����𑖂点��B
        {
            pickWindow[i].cardDate = handCards[i];// �J�[�h�� �C���[�W�摜�� �\��������E�B���h�E���� �f�[�^��n���A
            pickWindow[i].CardRefresh();//--------// �E�B���h�E���� �\���X�V�֐����N������B

            if (handCards[i] == noneCardData)//---// �����A���̃J�[�h�� "���ł��Ȃ�" �J�[�h�������ꍇ�A
            {
                manaCostText[i].enabled = false;  // �C���[�W�摜�����ɕ\�����Ă���}�i�R�X�g�� �e�L�X�g���\���ɂ���B
            }
            else//--------------------------------// �����Ŗ�����΁A�J�[�h�̃}�i�R�X�g�l�� �e�L�X�g�ɕ\��������B
            {
                manaCostText[i].enabled = true;   
                manaCostText[i].text =�@�@�@�@�@�@
                    string.Format("{00}", handCards[i].manaCost);
            }

            if(i == 0)//--------------------------// ��D�̒��� ��ԓ��ɂ���J�[�h�ł���΁A
            {
                if (handCards[0] == noneCardData) 
                {
                    castCardNameText.text = " ";
                    castCardDamageText.text = " ";
                }
                else//----------------------------// ���O�ƃ_���[�W�l�� �Q�Ƃ��A���ꂼ��� �e�L�X�g�ɕ\��������B
                {
                    castCardNameText.text = handCards[0].spellName;
                    if (handCards[0].damageString != null)
                        castCardDamageText.text = handCards[0].damageString + handCards[0].attackOftenString;
                }
            }
        }
        
        deckCountText.text = string.Format("{00}", deckCards.Count);//�f�b�L�c�ʕ\�����X�V����B

        switchRoot = default;// ���̏������I�������A�ҋ@��Ԃɖ߂�B
    }


    void HandDraw()// �f�b�L�����D�ɃJ�[�h�f�[�^��n���B
    {

        for(int i = 0; i < handCard_max; i++)
        {
            if(deckCards.Count <= 0)
            {
                handCards.Add(noneCardData);
            }
            else
            {
                handCards.Add(deckCards[0]);
                deckCards.RemoveAt(0);
            }
        }
        HundWindowRefresh();
    }


    void HundFalldToLoad()// ��D��j�����鏈���B�܂���������R�D(�f�b�L)����J�[�h���h���[���鏈���ɂȂ���B
    {

        handCards.Clear();// ��D�̃J�[�h�����ׂĔj���B

        audioSource.PlayOneShot(ac_HandCase_Reload_Start);//StartSE���Đ�����B

        audioSource.loop = true;//-------------------// ���[�v���I���B
        audioSource.clip = ac_HandCase_Reload_Charge;// ������ݒ�B
        audioSource.Play();//------------------------// �Đ��B

        if (deckCards.Count > 0)// �f�b�L���ɃJ�[�h���L��A�h���[���\�ȏꍇ�B
        {
            for (int i = 0; i < handCard_max; i++)
            {
                pickWindow[i].cardDate = noneCardData;
                pickWindow[i].CardRefresh();
                //Debug.Log("��D�`�ʓK���I");
            }

            drawCT = drawCT_max;//�h���[�ɕK�v�ȃN�[���^�C�����Đݒ�B

            handAnimator.SetBool("DrawBool", true);
            switchRoot = "DrawCoolTime";//�h���[�̏������s���B
        }
        else if(deckCards.Count <= 0)//�f�b�L��0�ɂȂ��Ă���ꍇ�Ɏ擾����B
        {
            //�f�b�L�f�[�^������B
            if (playingData.onHandDeckDate.Count == 0)
            {
                deckCards = new List<SpellData>(deckInst.spells);
            }
            else
            {
                deckCards = new List<SpellData>(playingData.onHandDeckDate);
            }

            deckReloadCT_max = deckReloadCT_basisTime + deckReloadCT_sumTime;//���v�N�[���^�C���̐��l���v�Z�B(��b�l�Ə�Z�l�����킹�B���v���Ԃɓ����B)
            deckReloadCT_sumTime = 0;//sumTime���[���ɂ���B
            deckReloadCT = deckReloadCT_max;//�f�b�L�����[�h�ɕK�v�ȃN�[���^�C�����Đݒ�B

            handAnimator.SetBool("DeckReloadBool", true);
            switchRoot = "DeckReloadingTime";//�f�b�L�����[�h�̏������s���B
        }
        else
        {
            switchRoot = default;
        }
    }


    void DrowingTime()//��D���h���[�����܂ł̎��ԏ����B
    {
        if(drawCT >= 0)
        {
            drawCT -= 1 * Time.deltaTime;

            if(cooltime_FillImage.enabled == false) cooltime_FillImage.enabled = true;
            if (cooltime_Text.enabled == false) cooltime_Text.enabled = true;


            cooltime_Text.text = string.Format("{00}", drawCT);//
            cooltime_FillImage.fillAmount = drawCT / drawCT_max;
        }
        else
        {
            Debug.Log("�h���[�N�[���^�C���I��");
            switchRoot = "CardDraw";

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//���[�v�𖳌��B
            audioSource.Stop();//�I�[�f�B�I�𖳌�������B
            audioSource.clip = null;//�ݒ艹���𖳌����B

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SE���Đ�����B

            handAnimator.SetBool("DrawBool", false);

            //switchRoot = "CardDraw";
        }
    }


    void DeckReloadingTime()//�R�D���V���b�t�������܂ł̎��ԏ����B
    {
        if(deckReloadCT >= 0)
        {

            deckReloadCT -= 1 * Time.deltaTime;

            if (cooltime_FillImage.enabled == false) cooltime_FillImage.enabled = true;
            if (cooltime_Text.enabled == false) cooltime_Text.enabled = true;

            cooltime_Text.text = string.Format("{00}", deckReloadCT);//
            cooltime_FillImage.fillAmount = deckReloadCT / deckReloadCT_max;
        }
        else
        {
            Debug.Log("�����[�h�N�[���^�C���I��");
            switchRoot = "DeckShuffle";//�f�b�L�����[�h�̏������s���B

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//���[�v�𖳌��B
            audioSource.Stop();//�I�[�f�B�I�𖳌�������B
            audioSource.clip = null;//�ݒ艹���𖳌����B

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SE���Đ�����B

            Debug.Log("�f�b�L��[");
            handAnimator.SetBool("DeckReloadBool", false);
            //switchRoot = "DeckShuffle";//�f�b�L�����[�h�̏������s���B
        }
    }


    ////--------------------------------------------------

    //// �J�[�h�̎g�p ////--------------------------------
    
    public void CardCast(InputAction.CallbackContext context)// �J�[�h�g�p�̓��͎�t
    {
        if (HCM_ProcessSafety == false)
        {
            if (statusManager.shockSt == false)// �X�e�[�^�X �V���b�N��Ԃ��ǂ���
            {
                if (standbySW == true)
                {
                    if (context.phase == InputActionPhase.Performed)// �{�^�����������Ƃ��Ɉ�x�������s����B
                    {
                        if (statusManager.manaPoint >= handCards[0].manaCost)
                        {
                            statusManager.Mana_Inflict_Expense(handCards[0].manaCost);//�R�X�g�������B
                            HandCard_Cast_Process();//�������J�n�B

                            // Debug.Log("�J�[�h�g�p");

                            standbySW = false;
                        }
                        else
                        {
                            // Debug.Log(gameObject + ">> �X�y���g�p�ɕK�v�ȃ}�i������Ȃ��B");
                            manaCostWindow_0_Animator.SetTrigger("CostNonEnough_Trigger");
                        }
                    }
                }
            }
        }

    }

    void HandCard_Cast_Process()// �J�[�h�g�p�̏����B
    {
        if (handCards.Count >= 0)
        {
            standbySW = false;//standby��Ԃ𖳌�������B

            castPanelAnimator.SetTrigger("CastTrigger");//�A�j���[�V�������Đ��B

            Vector3 vc3InstPos =
                instPos.transform.position;//InstObj��Vector3�ɕϊ��A�i�[�B

            GameObject instPrefab =
                Instantiate(handCards[0].spellPrefab, vc3InstPos, instPos.transform.rotation);//�J�[�h�f�[�^�����Ƀv���n�u�𐶐�����B

            instPrefab.GetComponent<SpellPrefabManager>().ownerObject = gameObject;//�X�y���v���n�u�}�l�[�W���[�ɃI�u�W�F�N�g����n���B

            //�g�p�����J�[�h�����炷�����B
            handCards.RemoveAt(0);
            pickWindow[handCards.Count - 1].CardRefresh();
            handCards.Add(noneCardData);

            HundWindowRefresh();

        }
    }


    ////--------------------------------------------------

    //// �J�[�h�̔j�� ////--------------------------------

    public void CardTrash(InputAction.CallbackContext context)// �J�[�h�j���̓��͎�t
    {
        if (HCM_ProcessSafety == false)
        {
            if (statusManager.shockSt == false)// �X�e�[�^�X �V���b�N��Ԃ��ǂ����B
            {
                if (standbySW == true)
                {
                    if (context.phase == InputActionPhase.Performed)// �{�^�����������Ƃ��Ɉ�x�������s����B
                    {
                        HandCard_Trash_Process();//�������J�n�B

                        //Debug.Log("�J�[�h�j��");

                        standbySW = false;
                    }
                }
            }
        }
    }

    void HandCard_Trash_Process()// �J�[�h�j���̏���
    {
        if (handCards.Count >= 0)
        {
            castPanelAnimator.SetTrigger("TrashTrigger");//�A�j���[�V�������Đ��B
            audioSource.PlayOneShot(cardFoldSE);//����炷�B
            statusManager.Mana_Inflict_Revenue(handCards[0].trashMana);//�}�i�|�C���g�̎擾�B

            //�g�p�����J�[�h�����炷�����B
            handCards.RemoveAt(0);
            pickWindow[handCards.Count - 1].CardRefresh();
            handCards.Add(noneCardData);

            HundWindowRefresh();

        }
    }


    ////--------------------------------------------------

    //// ���b�N�I���A�N�V���� ////------------------------

    public void LockOnSearch(InputAction.CallbackContext context)//���b�N�I�����s���B
    {
        if (HCM_ProcessSafety == false)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                if (lockOnTarget == null && canlockOnSW == true)
                {

                    Vector3 ori = gameObject.transform.position;
                    Ray ray = new Ray(ori, transform.forward);

                    int layerMask = ~LayerMask.GetMask(new string[] { "Default", "StructureObject", "InstObject", "Search",gameObject.tag});//��Ώۂ�Layer�����O����B
                    RaycastHit hit;

                    if (Physics.BoxCast(transform.position, new Vector3(1.5f, 1.5f, 1.5f), transform.forward, out hit, Quaternion.identity, lockOnDistance, layerMask))
                    {
                        if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != gameObject.tag)//�X�N���v�g�uStatusManager�v���t���Ă��āA���g�ƈႤ�^�O���t���Ă���ꍇ�ɔ�������B
                        {
                            Debug.Log("���b�N�I���I");

                            audioSource.PlayOneShot(lockOn_InSE);//�@���ʉ���炷�B

                            lockOnTarget = hit.transform.gameObject;// �W�I�� �I�u�W�F�N�g���� �i�[����B

                            playerMoving.lockOnTargetObj = lockOnTarget;// PlayerMoving��Target���i�[�B

                            cantLockOnTime = 0.2f;//�N�[���^�C����ݒ�B�ʏ���Z���Ԃ̃N�[���^�C���B

                            canlockOnSW = false;//���b�N�I����s�\�ɂ���B

                            if (TargetSM == null) TargetSM = lockOnTarget.GetComponent<StatusManager>();//�^�[�Q�b�g���擾�B
                            
                        }
                        //string name = hit.collider.gameObject.name;//�q�b�g�����I�u�W�F�N�g�̖��O���i�[�B
                        //Debug.Log(name);//�q�b�g�����I�u�W�F�N�g�̖��O�����O�ɏo���B

                    }
                }
                else if (lockOnTarget != null && canlockOnSW == true)
                {
                    LockOnOut(); //���b�N�I�����������ꂽ�ۂ̏����𑖂点��B
                }
            }
        }
    }

    //--------------------------------------------------------------

    void LockOnProcessing()
    {
        if(canlockOnSW == false)//���b�N�I���ł��Ȃ���Ԃł���ꍇ�A
        {
            //canTime�̐��l���o�ߒቺ������B
            if (cantLockOnTime >= 0)
            {
                cantLockOnTime -= 1 * Time.deltaTime;
            }
            else if (cantLockOnTime <= 0.0f && cantLockOnTime != 0)
            {
                cantLockOnTime = 0.0f;
                canlockOnSW = true;
            }
        }

        //// �G���ǉz���ɂ��邩�̊m�F���s���B
        
        if(lockOnTarget != null)// �^�[�Q�b�g���i�[����Ă���ꍇ�A
        {
            LockOnLineChain();// ���b�N�I�����Ă���I�u�W�F�N�g�Ƃ̊Ԃɐ���`�ʂ�����B

            Vector3 pos = lockOnTarget.transform.position - transform.position;

            Ray ray = new Ray(transform.position, pos);// �G�̍��W��ڕW�ʒu�ɐݒ�B

            int layerMask = LayerMask.GetMask(new string[] { "Default","StructureObject", "InstObject",lockOnTarget.tag}); // Layer�����ݒ肷��B�^�O���ƃ��C���[���������ł��鎖���K�{�B
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, lockOnDistance,layerMask))
            {

                if (hit.collider.gameObject != lockOnTarget)//�������b�N�I�����Ă���I�u�W�F�N�g���ǉz���ɂȂ�ƁB
                {
                    //���b�N�I�������܂ł̎��Ԍo�߁A�������s���B

                    if (lockOnOutTime >= 0)
                    {
                        lockOnOutTime -= 1 * Time.deltaTime;
                    }
                    else if (lockOnOutTime <= 0)
                    {
                        LockOnOut(); //���b�N�I�����������ꂽ�ۂ̏����𑖂点��B
                    }
                }
                else if (lockOnOutTime <= 0.5f)//�����A���b�N�I�����Ă���I�u�W�F�N�g�̃��C���[�����o���Ă���Ԃ́B
                {
                    //���b�N�I�������܂ł̎��Ԃ����Z�b�g�B
                    lockOnOutTime += 0.1f;
                }

                if (TargetSM.hitPoint <= 0) LockOnOut();//�������b�N�I�����Ă���Ώۂ�HP��0�̏ꍇ�A���b�N�I�����I�t�ɂ���B
            }
        }
    }

    void LockOnOut()//���b�N�I�����������ꂽ�ۂ̂̏����B
    {
        Debug.Log("���b�N�I�������I");

        audioSource.PlayOneShot(lockOn_OutSE);//���ʉ���炷�B

        lineRenderer.enabled = false;//�R���|�[�l���g�𖳌���������B
        lineSize = 0f;
        lockOnTarget = null;
        TargetSM = null;
        if (playerMoving == null) playerMoving = GetComponent<PlayerMoving>();
        playerMoving.lockOnTargetObj = lockOnTarget;

        cantLockOnTime = 0.5f;//�N�[���^�C����ݒ�B

        canlockOnSW = false;//���b�N�I����s�\�ɂ���B
    }

    void LockOnLineChain()//���b�N�I�����̃I�u�W�F�N�g�Ԃ̐���\��������B
    {
        lineRenderer.enabled = true;
        Vector3 p_vc3 = new Vector3(gameObject.transform.position.x, lockOnLineInstPoint, gameObject.transform.position.z);
        Vector3 t_vc3 = new Vector3(lockOnTarget.transform.position.x, lockOnLineInstPoint, lockOnTarget.transform.position.z);

        
        lineRenderer.SetPosition(0, p_vc3);
        lineRenderer.SetPosition(1, t_vc3);

        if (lineSize <= 0.5f)
        {
            lineSize += 5 * Time.deltaTime;
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;
        }
    }

    ////--------------------------------------------------
}
