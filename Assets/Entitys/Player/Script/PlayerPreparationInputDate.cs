using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PlayerPreparationInputDate : MonoBehaviour
{
    PlayingData playingData;
    public PlayableDateManager pDM;

    public bool PID_InputSafety;//���͂��o���邩�ǂ����̈��S���u�B(false�ŉ������)

    public GameObject headObg;
    public GameObject bodyObj;
    public GameObject cursorObj;
    public GameObject cursorArrowObj;

    //�i���𐔒l�Ƃ��ď�������ׂɎg�p����B
    //���C���c���[���l�B
    int treeParagraphInt;//���C���Ŏg�p�����s�B
    int treeParagraphInt_Max = 0;//�ő�l
    int treeParagraphInt_Min = -1;//�ŏ��l

    //�L�����J�X�^�}�C�Y���l�B
    /*
    int tree_CharaCustom_Main_Int;//�L�����J�X�^���̃��C���Ŏg�p�����s�B
    int tree_CharaCustomInt_Main_Max = 0;//�L�����J�X�^���̍ő�l
    int tree_CharaCustomInt_Main_Min = -1;//�L�����J�X�^���̍ŏ��l
    */
    int tree_CharaCustomInt_VC_Int;//�L�����J�X�^�����̃r�W���A���J���[�Ŏg�p�����s�B
    int tree_CharaCustomInt_VC_Max = 8;//�r�W���A���J���[�̍ő�l�B
    int tree_CharaCustomInt_VC_Min = 0;//�r�W���A���J���[�̍ŏ��l�B
    /*
    //�p���b�g�J�X�^�����l�B
    int tree_PalletCustom_Main_Int;//�p���b�g�J�X�^���̃��C���Ŏg�p�����s�B
    int tree_PalletCustom_Main_Max;//�p���b�g�J�X�^���̍ő�l
    int tree_PalletCustom_Main_Min;//�p���b�g�J�X�^���̍ŏ��l

    int tree_PalletCustom_SA_Int;//�p���b�g�J�X�^�����̃Z�J���h�A�N�V�����Ŏg�p�����s�B
    int tree_PalletCustom_SA_Max = 3;//�Z�J���h�A�N�V�����̍ő�l�B
    int tree_PalletCustom_SA_Min = 0;//�Z�J���h�A�N�V�����̍ŏ��l�B
    */

    //�G�N�X�g���c���[���l�B
    int lowerTree_Paragraph_Int;//�����o�[�̑I�����s���ׂɎg�p�����B
    int lowerTree_Paragraph_Max;//�����o�[�I���̍ő�l�B
    int lowerTree_Paragraph_Min;//�����o�[�I���̍ŏ��l�B




    //�E�B���h�E�N�����������B
    // public bool charaCustomSW;//�L�����J�X�^�}�C�YWindow�Ɉړ����Ă��邩���ɒl�ŕ\���Ă���B
    // public bool palletCustomSW;//�p���b�g�J�X�^�}�C�Y�Ɉړ����Ă��邩���ɒl�ŕ\���Ă���B
    public bool readySW;//���������Ɉړ����Ă��邩���ɒl�ŕ\���Ă���B

    public List<GameObject> pick_Cursor_sprite;//�J�[�\���X�v���C�g��؂�ւ���B�A�^�b�`�K�{�B

    public Image colorPanel_Image;//�C���[�W�J���[��ύX����ۂ̕`��Image�B�A�^�b�`�K�{�B
    public List<Color32> colorDate;//�C���[�W�J���[��ύX����ۂɎQ�Ƃ���ꏊ�B

    public Image PCW_WindowBackImage;//�E�B���h�E�̔w�i�C���[�W�B�A�^�b�`�K�{�B
    public GameObject Ready_ImageGroup;//�u���������v���Ӗ�����C���[�W�B
    public Image Ready_Image;//�u���������v���Ӗ�����C���[�W�B

    //���ʉ�
    AudioSource ppid_AS;
    public AudioClip cursorMoveSE;
    public AudioClip enterSE;
    public AudioClip cancelSE;

    void Start()
    {
        playingData = GetComponent<PlayingData>();
        ppid_AS = GetComponent<AudioSource>();
        PlayerInput pi = GetComponent<PlayerInput>();//�v���C���[�C���v�b�g���擾�B

        pi.currentActionMap = pi.actions.actionMaps[1];//�擾����PlayerInput�̃A�N�V�����}�b�v��1�Ԗ�(CharaSetting)�ɕύX�B
        tree_CharaCustomInt_VC_Int = playingData.playerNumber;//�v���C���[�i���o�[���J���[Custom��Int�ɍ��킹��B

        inputValueRefresh();

        WindowbackUpdate();
        TreeVisRefresh();//�\�����e�̍X�V���s���B
        ColorVisRefresh();
        ModelLayerRefresh();
    }

    void ModelLayerRefresh()
    {
        if(playingData.playerNumber == 0)
        {
            headObg.layer = 6;//"Player_1"
            bodyObj.layer = 6;
            cursorObj.layer = 6;
            cursorArrowObj.layer = 6;
        }
        else if (playingData.playerNumber == 1)
        {
            headObg.layer = 7;//"Player_2"
            bodyObj.layer = 7;
            cursorObj.layer = 7;
            cursorArrowObj.layer = 7;
        }
        else if (playingData.playerNumber == 2)
        {
            headObg.layer = 8;//"Player_3"
            bodyObj.layer = 8;
            cursorObj.layer = 8;
            cursorArrowObj.layer = 8;
        }
        else if (playingData.playerNumber == 3)
        {
            headObg.layer = 9;//"Player_4"
            bodyObj.layer = 9;
            cursorObj.layer = 9;
            cursorArrowObj.layer = 9;
        }
    }

    public void inputValueRefresh()//�Ăяo�����Ƃł��ׂĂ�UI�ݒu�l��������������B(InGameManager�����j���[�ɖ߂��Ă����ۂɌĂяo���B)
    {
        readySW = false;
        Ready_ImageGroup.SetActive(false);
        treeParagraphInt = 0;

        TreeVisRefresh();//�\�����e�̍X�V���s���B
    }

    public void UpPoint(InputAction.CallbackContext context)//��Ɉړ��B
    {
        if (context.performed)
        {
            if (treeParagraphInt < treeParagraphInt_Max && readySW == false)
            {
                ppid_AS.PlayOneShot(cursorMoveSE);//���ʉ���炷�B
                treeParagraphInt += 1;
                Debug.Log(treeParagraphInt);
                TreeVisRefresh();//�\�����e�̍X�V���s���B
            }
            /*
            if (charaCustomSW == true)
            {
                if (tree_CharaCustom_Main_Int < tree_CharaCustomInt_Main_Max)
                {
                    tree_CharaCustom_Main_Int += 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            else if (palletCustomSW == true)
            {
                if(tree_PalletCustom_Main_Int < tree_PalletCustom_Main_Max)
                {
                    tree_PalletCustom_Main_Int += 1;
                }
            }
            else if (readySW == true)//Ready��Ԃ̏ꍇ�B
            {

            }
            else//�����I�����Ă��锻�肪�����ꍇ�B(���C���c���[��I����ԁB)
            {
                if (treeParagraphInt < treeParagraphInt_Max)
                {
                    treeParagraphInt += 1;
                    Debug.Log(treeParagraphInt);
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            */
        }
    }

    public void DownPoint(InputAction.CallbackContext context)//���Ɉړ��B
    {
        if (context.performed) 
        {
            if (treeParagraphInt > treeParagraphInt_Min && readySW == false)
            {
                ppid_AS.PlayOneShot(cursorMoveSE);//���ʉ���炷�B
                treeParagraphInt -= 1;
                Debug.Log(treeParagraphInt);
                TreeVisRefresh();//�\�����e�̍X�V���s���B
            }
            /*
            if(charaCustomSW == true)
            {
                if (tree_CharaCustom_Main_Int > tree_CharaCustomInt_Main_Min)
                {
                    tree_CharaCustom_Main_Int -= 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            else if(palletCustomSW == true)
            {
                if(tree_PalletCustom_Main_Int > tree_PalletCustom_Main_Min)
                {
                    tree_PalletCustom_Main_Int -= 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            else if(readySW == true)//Ready��Ԃ̏ꍇ�B
            {

            }
            else//�����I�����Ă��锻�肪�����ꍇ�B(���C���c���[��I����ԁB)
            {
                if (treeParagraphInt > treeParagraphInt_Min)
                {
                    treeParagraphInt -= 1;
                    Debug.Log(treeParagraphInt);
                    ColorVisRefresh();//�J���[�\�����e�̍X�V���s���B
                }
            }
            */
        }
    }

    public void RightInout(InputAction.CallbackContext context)//�E���́B
    {
        if (context.performed)
        {
            if (treeParagraphInt == 0)
            {
                if (tree_CharaCustomInt_VC_Int < tree_CharaCustomInt_VC_Max)
                {
                    ppid_AS.PlayOneShot(cursorMoveSE);//���ʉ���炷�B
                    tree_CharaCustomInt_VC_Int += 1;
                    ColorVisRefresh();//�J���[�\�����e�̍X�V���s���B
                    // WindowbackUpdate();
                    WindowbackUpdate();
                }
            }
            /*
            if (charaCustomSW == true)
            {
                if(tree_CharaCustomInt_VC_Int < tree_CharaCustomInt_VC_Max)
                {
                    tree_CharaCustomInt_VC_Int += 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            else if (palletCustomSW == true)
            {
                if(tree_PalletCustom_SA_Int < tree_PalletCustom_SA_Max)
                {
                    tree_PalletCustom_SA_Int += 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            */
        }
    }
    public void LeftInput(InputAction.CallbackContext context)//�����́B
    {
        if (context.performed)
        {
            if (treeParagraphInt == 0)
            {
                if (tree_CharaCustomInt_VC_Int > tree_CharaCustomInt_VC_Min)
                {
                    ppid_AS.PlayOneShot(cursorMoveSE);//���ʉ���炷�B
                    tree_CharaCustomInt_VC_Int -= 1;
                    ColorVisRefresh();//�J���[�\�����e�̍X�V���s���B
                    // WindowbackUpdate();
                    WindowbackUpdate();
                }
            }
            /*
            if (charaCustomSW == true)
            {
                if (tree_CharaCustomInt_VC_Int > tree_CharaCustomInt_VC_Min)
                {
                    tree_CharaCustomInt_VC_Int -= 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            else if (palletCustomSW == true)
            {
                if (tree_PalletCustom_SA_Int > tree_PalletCustom_SA_Min)
                {
                    tree_PalletCustom_SA_Int -= 1;
                    TreeVisRefresh();//�\�����e�̍X�V���s���B
                }
            }
            */
        }
    }

    public void InputEnter(InputAction.CallbackContext context)//����{�^���̓��́B
    {
        if (context.performed)
        {
            if(treeParagraphInt == 0)//�C���[�W�J���[��������͂���B
            {
                ppid_AS.PlayOneShot(enterSE);//���ʉ���炷�B
                WindowbackUpdate();
            }
            else if(treeParagraphInt == -1)
            {
                ppid_AS.PlayOneShot(enterSE);//���ʉ���炷�B
                SetEnter();
            }
            /*
            if(charaCustomSW == true)//�L�����J�X�^����I�����Ă��锻�肪����ꍇ�B
            {
                //��������{�^���Őݒ肷��悤�ɂ���Ȃ�΂����̏����ŕϐ����`�����鏈�����s���K�v������B
            }
            else if(palletCustomSW == true)//�p���b�g�J�X�^����I�����Ă��锻�肪����ꍇ�B
            {
                //��������{�^���Őݒ肷��悤�ɂ���Ȃ�΂����̏����ŕϐ����`�����鏈�����s���K�v������B
            }
            else//�����I�����Ă��锻�肪�����ꍇ�B
            {
                if (treeParagraphInt == 0)
                {
                    charaCustomSW = true;//�L�����J�X�^����I�����Ă���B
                }
                else if (treeParagraphInt == -1)
                {
                    palletCustomSW = true;//�p���b�g�J�X�^����I�����Ă���B
                }
                else if (treeParagraphInt == -2)
                {
                    readySW = true;
                }
            }
            */
        }
    }

    public void InputCancel(InputAction.CallbackContext context)//���ۃ{�^���̓��́B
    {
        if (context.performed)
        {
            if(PID_InputSafety == false)
            {
                if (treeParagraphInt == -1)
                {
                    ppid_AS.PlayOneShot(cancelSE);//���ʉ���炷�B
                    readySW = false;
                    Ready_ImageGroup.SetActive(false);
                }
            }
        }
    }

    void TreeVisRefresh()//���C���c���[�̕\�����e�̍X�V���s���B
    {
        if (treeParagraphInt == 0)
        {
            pick_Cursor_sprite[0].SetActive(true);

            pick_Cursor_sprite[1].SetActive(false);
        }
        else if(treeParagraphInt == -1)
        {
            pick_Cursor_sprite[1].SetActive(true);

            pick_Cursor_sprite[0].SetActive(false);
        }
    }

    void WindowbackUpdate()//�F�̓K���B
    {
        Color32 color_D = colorDate[tree_CharaCustomInt_VC_Int];

        
        playingData.playerVisualColor = color_D;
        color_D.a = 200;
        PCW_WindowBackImage.color = color_D;
        color_D.a = 150;
        Ready_Image.color = color_D;
    }
    void ColorVisRefresh()//�T���v���C���[�W�J���[�̕\�����e�̍X�V���s���B
    {
        if(colorPanel_Image.color != colorDate[tree_CharaCustomInt_VC_Int])
        {
            colorPanel_Image.color = colorDate[tree_CharaCustomInt_VC_Int];
        }
    }

    void SetEnter()//�������I���A���������肵���B
    {
        playingData.teamNumber = playingData.playerNumber;//[�e�X�g�@�\]���g�̔ԍ����`�[���ԍ��ɒ������B
        Color32 color_D = colorDate[tree_CharaCustomInt_VC_Int];
        pDM.playerCursorMaterials[playingData.playerNumber].color = color_D;
        pDM.playerLockOnLineMaterial[playingData.playerNumber].color = color_D;
        pDM.playerOverRayMaterial[playingData.playerNumber].color = color_D;

        pDM.playableChara_UI_PalletFrame_Image[playingData.playerNumber].color = color_D;
        readySW = true;
        pDM.ReadySW_detection();//PDM�̑S����Ready���������������m����B();//PDM�̑S����Ready���������������m����B
        Ready_ImageGroup.SetActive(true);
    }
}
