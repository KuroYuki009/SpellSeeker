using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerGSWInput : MonoBehaviour
{
    PlayingData playingData;//
    PlayerAdopt_Card playerAdopt_Card;

    Animator animator_GSW;

    public bool selectEndSW;//�I�����������Ă��邩�ǂ����BInGameManager���̔���Ɏg�p����B

    public string rootswitch;//switch���̏������򂫎g�p���܂��B

    public string loadType;//�ǂݍ��ރ^�C�v��I���B("SpellDate"�ł���΃J�[�h�n�̓ǂݍ��݂��s���B)

    public List<SpellData> pick_SpellDates;//�O�����瑗���Ă���I���ł���f�[�^�BSpellData�p(ScriptableObject���g�p����)
    int canPickInt;//�I���ł���v�f�̐��B(���ɍ��킹�đI��g�̔z�u��ύX�B)

    //�_�C�i�~�b�N�I�����J�[�\���ړ��Ɏg�p���܂��B
    InputAction selectMove;

    public GameObject dynamic_pickCursorObj;//�J�[�\���ƂȂ�UI���A�^�b�`�B
    RectTransform dynamic_Cursor_rtf;//�J�[�\���ƂȂ�UI���A�^�b�`�B
    Image dynamic_Cursor_img;

    [SerializeField]int nowSelectChoiceInt;//���݂ǂ�ɃJ�[�\�������킹�Ă��邩���擾����ׂ�int�l�B

    public SpellData[] spellCardDates = new SpellData[4];//�X�y���f�[�^���i�[����ׂɎg�p����B

    ////UI

    //�I��g (0_��A1_���A2_���A3_�E�A�ɂȂ�悤�ɁB�܂��Q�Ǝ��A�������ɂ���B)//�A�^�b�`�K�{�B
    public List<GameObject> selectPickCursorWindowObject;//�v�f��I������ۂ̃C���[�W�摜�\��������g�g�݃I�u�W�F�N�g�B
    public List<Image> selectPickCursorWindowImage;//�v�f��I������ۂ̃C���[�W�摜�\��������g�g�݂�Image�R���|�[�l���g�B
    public  List<Image> selectPickImageWindow;//�v�f��I������ۂ̃C���[�W�摜�\��������ꏊ�B
    public bool[] canSelectPickWindowSW = new bool[4];//�I��g���I���ł��邩�ǂ�����bool�B
    public List<GameObject>canSelectPickWindowObj;//���ݑI���ł���g���B
    public List<Image> canSelectPickImage;//���ݑI���ł���g���̃C���[�W�摜�\�������B
    public List<SpellData> canSelectPickDate_SpellDate;//���ݑI���ł���f�[�^�BSpellDate�p�B

    public GameObject object_GSW;//GeneralSelectWindow���̂��́B
    public GameObject object_GSWStandBy;//GSW�̃X�^���o�C��Ԃɕ\������I�u�W�F�N�g�B

    public Color32 pick_NoneColorString;

    //back_
    public Image backGroundNumber;//�I����Ԃɕ\������ԍ��C���[�W�B
    public Image standbyBackNumber;//���I��ҋ@��Ԃɕ\������ԍ��C���[�W�B
    public List<Sprite> backNumber_Sprite;//�v���C���[�̔ԍ��X�v���C�g���A�^�b�`����K�v������B

    //generalSelectTipsPanel
    public GameObject generalSelectTipsPanelObj;//�ėp�I��Tips�p�l���I�u�W�F�N�g�B

    //card_Manual
    public GameObject card_ManualPanelObj;//�J�[�h�����p�l���I�u�W�F�N�g�B

    public Image card_imageSprite;//�f�[�^�̃C���[�W�摜��\������B
    public Text card_dateTitleText;//�f�[�^�̖��O��\�L����e�L�X�g�B
    public Text card_manualTipsText;//�f�[�^�̐������ɓ����镶�͂�\������e�L�X�g�B
    public Text card_commentOutText;//�f�[�^�̂ǂ��ł��������͂�\������e�L�X�g�B

    public Text card_spellCostText;//�X�y���g�p�ɕK�v�ȃ}�i�R�X�g��\������e�L�X�g�B
    public Text card_spellDamageText;//�X�y���̃_���[�W�l��\������e�L�X�g�B
    public Text card_spellDamageOftenText;//�X�y���̍U������\������e�L�X�g�B

    //���ʉ�
    AudioSource audioSource;
    public AudioClip selectSE;
    public AudioClip enterSE;

    void Start()
    {
        playingData = GetComponent<PlayingData>();
        playerAdopt_Card = GetComponent<PlayerAdopt_Card>();
        animator_GSW = object_GSW.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        dynamic_Cursor_rtf = dynamic_pickCursorObj.GetComponent<RectTransform>();
        dynamic_Cursor_img = dynamic_pickCursorObj.GetComponent<Image>();

        PlayerInput playerInput = GetComponent<PlayerInput>();//�{�̈ȊO����̓��͂œ����׈ꎞ�R�����g�A�E�g�B
        selectMove = playerInput.actions["SelectMove"];//�w�肷�����

        backGroundNumber.sprite = backNumber_Sprite[playingData.playerNumber];//�I����ԃW�ɕ\������ԍ���ύX�B
        standbyBackNumber.sprite = backNumber_Sprite[playingData.playerNumber];//���I��ҋ@��Ԏ��ɕ\������ԍ���ύX�B
    }

    
    void Update()
    {
        switch(rootswitch)
        {
            case "SetUpLoad"://�Z�b�g�A�b�v�B
                SetUpLoad();
                break;
            case "ChoiceSelect"://�I�𒆁B
                InputCursor_Dynamic();
                break;
            case "PickSelect"://�I�������B
                if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
                {
                    playerAdopt_Card.Select_PickCallBack();
                }
                break;

        }
    }

    public void SetUpLoad()//�f�[�^�̓ǂݍ��݂���I��ҋ@��Ԃւ̈ڍs�܂ōs���B
    {
        if(selectEndSW == true)
        {
            selectEndSW = false;
        }

        object_GSW.SetActive(true);

        animator_GSW.SetTrigger("Blink_SelectWindow_Trigger");//�I��g�̂�����A�j���[�V�������Đ��B

        PlayerInput pI = GetComponent<PlayerInput>();
        pI.currentActionMap = pI.actions.actionMaps[2];//�擾����PlayerInput�̃A�N�V�����}�b�v��3�Ԗ�(Player)�ɕύX�B

        dynamic_Cursor_img.color = playingData.playerVisualColor;//CursorColor��ݒ�B

        //�ǂ̂悤�ȏ�񂪑����Ă��邩���擾����BString�^loadType�ϐ��B
        //��������̑I�����ł��邩�̊m�F�B
        if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
        {
            canPickInt = pick_SpellDates.Count;//
        }

        for (int i = 0; 4 > i; i++)//��x�I���\�g���Ȃ����B
        {
            canSelectPickWindowSW[i] = false;//���ׂĂ̓��͉\�����𖳗͉��B
            spellCardDates[i] = null;//spellCardDates���̃f�[�^�����ׂď����B
            selectPickCursorWindowObject[i].SetActive(false);//���ׂĔ�\���B
        }
        if (canSelectPickDate_SpellDate != null) canSelectPickDate_SpellDate.Clear();//�폜
            //�������Ƃɂ��I��g�z�u�̕ύX�B
        if (canPickInt == 1)//�����I���\����1�̏ꍇ�B
        {
            selectPickCursorWindowObject[0].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[0]);//�㕔��ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[0]);
            canSelectPickWindowSW[0] = true;
            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[0] = pick_SpellDates[0];
            }
        }
        else if(canPickInt == 2)//�����I���\����2�̏ꍇ�B
        {
            selectPickCursorWindowObject[2].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[2]);//������ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[2]);
            canSelectPickWindowSW[2] = true;
            selectPickCursorWindowObject[3].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[3]);//�E����ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[3]);
            canSelectPickWindowSW[3] = true;

            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[2] = pick_SpellDates[0];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[1]);
                spellCardDates[3] = pick_SpellDates[1];
            }
        }
        else if(canPickInt == 3)//�����I���\��3���̏ꍇ�B
        {
            selectPickCursorWindowObject[2].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[2]);//������ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[2]);
            canSelectPickWindowSW[2] = true;
            selectPickCursorWindowObject[0].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[0]);//�㕔����ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[0]);
            canSelectPickWindowSW[0] = true;
            selectPickCursorWindowObject[3].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[3]);//�E����ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[3]);
            canSelectPickWindowSW[3] = true;

            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[2] = pick_SpellDates[0];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[1]);
                spellCardDates[0] = pick_SpellDates[1];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[2]);
                spellCardDates[3] = pick_SpellDates[2];
            }
        }
        else if(canPickInt == 4)//�����I���\����4�̏ꍇ�B
        {
            selectPickCursorWindowObject[0].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[0]);//�㕔��ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[0]);
            canSelectPickWindowSW[0] = true;
            selectPickCursorWindowObject[1].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[1]);//������ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[1]);
            canSelectPickWindowSW[1] = true;
            selectPickCursorWindowObject[2].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[2]);//������ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[2]);
            canSelectPickWindowSW[2] = true;
            selectPickCursorWindowObject[3].SetActive(true);//�\���B
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[3]);//�E����ǉ��B
            canSelectPickImage.Add(selectPickImageWindow[3]);
            canSelectPickWindowSW[3] = true;

            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[0] = pick_SpellDates[0];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[1]);
                spellCardDates[1] = pick_SpellDates[1];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[2]);
                spellCardDates[2] = pick_SpellDates[2];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[3]);
                spellCardDates[3] = pick_SpellDates[3];
            }
        }

        for (int i = 0; canPickInt > i; i++)//�摜�����X�V�B
        {
            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
                canSelectPickImage[i].sprite = canSelectPickDate_SpellDate[i].imageSprite;//�C���[�W�摜���X�V����B
        }

        rootswitch = "ChoiceSelect";
    }

    ////���ݕK�v�����m�F���BSetUp�œ�������������\��B
    /*
    void Refresh()//�e��i�[�ϐ��������������s���B
    {
        for (int i = 0; canPickInt > i; i++)//�摜�����X�V�B
        {
            if(loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
                canSelectPickImage[i].sprite = canSelectPickDate_SpellDate[i].imageSprite;//�C���[�W�摜���X�V����B
        }
    }
    */
    

    public void InputEnter(InputAction.CallbackContext context)//����{�^�����́B
    {
        if(rootswitch == "ChoiceSelect")//���݂̑I������Ă��鏈����"ChoiceSelect"�ł���΁A
        {
            PickSelect();
        }
    }

    void InputCursor_Dynamic()//�_�C�i�~�b�N�I�����J�[�\���ړ��B(���̎��܂ňړ�������Ɗe�����ɃX�i�b�v����܂��B)
    {
        Vector2 moveVC2 = selectMove.ReadValue<Vector2>();
        Vector3 moveInput = new Vector3(moveVC2.x, moveVC2.y, 0);

        if (moveInput.y >= 0.75f && canSelectPickWindowSW[0] == true)//1
        {
            selectPickCursorWindowImage[0].color = playingData.playerVisualColor;
            selectPickCursorWindowImage[1].color = pick_NoneColorString;
            selectPickCursorWindowImage[2].color = pick_NoneColorString;
            selectPickCursorWindowImage[3].color = pick_NoneColorString;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(0, 35f, 0);

            if(nowSelectChoiceInt != 1)
            {
                audioSource.PlayOneShot(selectSE);//���ʉ���炷�B
                nowSelectChoiceInt = 1;
            }
            
            PickInfoDateRefresh(0);
            // Debug.Log("��");
        }
        else if (moveInput.y <= -0.75f && canSelectPickWindowSW[1] == true)//2
        {
            selectPickCursorWindowImage[0].color = pick_NoneColorString;
            selectPickCursorWindowImage[1].color = playingData.playerVisualColor;
            selectPickCursorWindowImage[2].color = pick_NoneColorString;
            selectPickCursorWindowImage[3].color = pick_NoneColorString;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(0, -35f, 0);

            if (nowSelectChoiceInt != 2)
            {
                audioSource.PlayOneShot(selectSE);//���ʉ���炷�B
                nowSelectChoiceInt = 2;
            }

            PickInfoDateRefresh(1);
            // Debug.Log("��");
        }

        else if (moveInput.x <= -0.75f && canSelectPickWindowSW[2] == true)//3
        {
            selectPickCursorWindowImage[0].color = pick_NoneColorString;
            selectPickCursorWindowImage[1].color = pick_NoneColorString;
            selectPickCursorWindowImage[2].color = playingData.playerVisualColor;
            selectPickCursorWindowImage[3].color = pick_NoneColorString;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(-35f, 0, 0);

            if (nowSelectChoiceInt != 3)
            {
                audioSource.PlayOneShot(selectSE);//���ʉ���炷�B
                nowSelectChoiceInt = 3;
            }

            PickInfoDateRefresh(2);
            // Debug.Log("��");
        }
        else if (moveInput.x >= 0.75f && canSelectPickWindowSW[3] == true)//4
        {
            selectPickCursorWindowImage[0].color = pick_NoneColorString;
            selectPickCursorWindowImage[1].color = pick_NoneColorString;
            selectPickCursorWindowImage[2].color = pick_NoneColorString;
            selectPickCursorWindowImage[3].color = playingData.playerVisualColor;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(35, 0, 0);

            if (nowSelectChoiceInt != 4)
            {
                audioSource.PlayOneShot(selectSE);//���ʉ���炷�B
                nowSelectChoiceInt = 4;
            }

            PickInfoDateRefresh(3);
            // Debug.Log("�E");
        }
        else
        {
            dynamic_Cursor_rtf.anchoredPosition = new Vector3(0, 0, 0) + new Vector3(35 * moveInput.x, 35 * moveInput.y, 0);//�X�i�b�v����Ȃ���Ԃł̑���B
            if(nowSelectChoiceInt != 0)
            {
                selectPickCursorWindowImage[0].color = pick_NoneColorString;
                selectPickCursorWindowImage[1].color = pick_NoneColorString;
                selectPickCursorWindowImage[2].color = pick_NoneColorString;
                selectPickCursorWindowImage[3].color = pick_NoneColorString;

                nowSelectChoiceInt = 0;
            }
            if (moveInput == new Vector3(0, 0, 0))//�����A�������͂���Ă��Ȃ��ꍇ�B
            {
                //�����ł��ׂĂ̐����p�l���𖳌�������B
                card_ManualPanelObj.SetActive(false);//�J�[�h�����p�l�����N���B
            }
        }
    }

    void PickInfoDateRefresh(int i)//�I�𒆂̗v�f�̏��(Tips)��ǂݍ��ށB
    {
        if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
        {
            if (spellCardDates[i] != null)
            {
                card_ManualPanelObj.SetActive(true);//�J�[�h�����p�l�����N���B

                card_imageSprite.sprite = spellCardDates[i].imageSprite;//�C���[�W�摜��K���B
                card_dateTitleText.text = spellCardDates[i].spellName;//�X�y����������B
                card_manualTipsText.text = spellCardDates[i].manualText;//������������B
                card_commentOutText.text = spellCardDates[i].commentText;//�R�����g�A�E�g������B

                card_spellCostText.text = spellCardDates[i].manaCostString;//string�^�}�i�R�X�g�l������B
                card_spellDamageText.text = spellCardDates[i].damageString;//string�^�_���[�W�l������B
                card_spellDamageOftenText.text = spellCardDates[i].attackOftenString;//string�^�U���񐔂�����B
            }
            else
            {
                //Debug.Log("�����i�[����Ă��Ȃ�")
            }
        }

        

    }

    void PickSelect()//�I�񂾐������i�[����
    {
        if(nowSelectChoiceInt == 1)//�P(��)��I��
        {
            Debug.Log("���I���B");
            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                //�����Ƀf�b�L�ɉ�����A
                playingData.onHandDeckDate.Add(spellCardDates[0]);
            }

            audioSource.PlayOneShot(enterSE);//���ʉ���炷�B

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 2)//�Q(��)��I��
        {
            Debug.Log("����I���B");
            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                //�����Ƀf�b�L�ɉ�����A
                playingData.onHandDeckDate.Add(spellCardDates[1]);
            }

            audioSource.PlayOneShot(enterSE);//���ʉ���炷�B

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 3)//�R(��)��I��
        {
            Debug.Log("����I���B");
            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                //�����Ƀf�b�L�ɉ�����A
                playingData.onHandDeckDate.Add(spellCardDates[2]);
            }

            audioSource.PlayOneShot(enterSE);//���ʉ���炷�B

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 4)//�S(�E)��I��
        {
            Debug.Log("�E��I���B");
            if (loadType == "SpellDate")//�����Ăяo���^�C�v��"SpellDate"�ł���΁A
            {
                //�����Ƀf�b�L�ɉ�����A
                playingData.onHandDeckDate.Add(spellCardDates[3]);
            }

            audioSource.PlayOneShot(enterSE);//���ʉ���炷�B

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 0)//���I���B
        {
            Debug.Log("���I��");
        }
    }

    public void SelectEnd()//�I���I���B
    {
        animator_GSW.SetTrigger("Fade_StandbyWindow_Trigger");
        selectEndSW = true;//�I�����I�����Ă���Ƃ���B(true��)
    }

    public void FallWindow()
    {
        animator_GSW.SetTrigger("FallWindow_Trigger");//�A�j���[�V�����Đ��B
    }
    public void HUD_Hidden()//HUD���\���ɂ���B
    {
        object_GSW.SetActive(false);
    }
}
