using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleGamemodeSettingManager : MonoBehaviour
{
    ////���̃X�N���v�g�͂��ł����O�A������ύX�o����悤�ɂ���K�v������ׁA�K���{�X�N���v�g�̎Q�Ɠ��̍s�ׂ��s���ۂ͋L�q���邱�ƁB
    //>���݂ǂ������̃X�N���v�g���擾���Ă��܂���Bgood!

    InGameManager inGameManager;
    PlayableDateManager pdManager;
    //�A�^�b�`��
    public GameObject imageText_AddSubSc_StarterDecksObj;//�C���[�W�e�L�X�g�Q�uAddSubSc_StarterDeck�v
    public GameObject imageText_AddSubSc_ShieldBuildsObj;//�C���[�W�e�L�X�g�Q�uAddSubSc_ShieldBuild�v

    public GameObject imageText_Stage_GridObj;//
    public GameObject imageText_Stage_ConcreteObj;//
    public GameObject imageText_Stage_HighPlaceObj;//

    //
    int stageNumberInt_min;//�ŏ��l�B
    int stageNumberInt_max;//�ő�l�B

    void Start()
    {
        inGameManager = GetComponent<InGameManager>();
        pdManager = GetComponent<PlayableDateManager>();

        stageNumberInt_min = 1;
        stageNumberInt_max = 3;
        RefreshVisStageName();
        RefreshVisAddSubScMode();
    }

    void Update()
    {
        /*
        if(inGameManager.currentSceneDate.sceneGameMode == "Menu" || inGameManager.currentSceneDate == null)
        {
            
        }*/
        if (Input.GetKey("joystick button 3"))//Y�{�^��
        {
            
            if (Input.GetKeyDown("joystick button 5"))//RB�{�^��
            {
                Debug.Log("�X�e�[�W�ύX");
                if (pdManager.stageNumberInt < stageNumberInt_max)
                {
                    pdManager.stageNumberInt += 1;
                }
                RefreshVisStageName();
            }
            else if (Input.GetKeyDown("joystick button 4"))//LB�{�^��
            {
                Debug.Log("�X�e�[�W�ύX");
                if (pdManager.stageNumberInt > stageNumberInt_min)
                {
                    pdManager.stageNumberInt -= 1;
                }
                RefreshVisStageName();
            }

            
        }


        if (Input.GetKey("joystick button 2"))//X�{�^��
        {
            if (Input.GetKeyDown("joystick button 5"))//LB�{�^��
            {
                Debug.Log("�ǉ��v�f�ύX");
                if (inGameManager.shieldBuild_ModeSW == true)
                {
                    inGameManager.shieldBuild_ModeSW = false;
                }
                else if (inGameManager.shieldBuild_ModeSW == false)
                {
                    inGameManager.shieldBuild_ModeSW = true;
                }
                RefreshVisAddSubScMode();
            }
        }
    }

    public void RefreshVisAddSubScMode()//���[�h�̒ǉ��v�f�̕\�L�X�V�B
    {
        if(inGameManager.shieldBuild_ModeSW == true)
        {
            imageText_AddSubSc_ShieldBuildsObj.SetActive(true);
            imageText_AddSubSc_StarterDecksObj.SetActive(false);
        }
        else
        {
            imageText_AddSubSc_StarterDecksObj.SetActive(true);
            imageText_AddSubSc_ShieldBuildsObj.SetActive(false);
        }
    }

    public void RefreshVisStageName()//�X�e�[�W�̖��O�\�L�X�V�B
    {
        Debug.Log(pdManager.stageNumberInt);

        if(pdManager.stageNumberInt == 1)//Grid�X�e�[�W�B
        {
            imageText_Stage_GridObj.SetActive(true);
            imageText_Stage_ConcreteObj.SetActive(false);//
            imageText_Stage_HighPlaceObj.SetActive(false);
        }
        else if(pdManager.stageNumberInt == 2)//Concrete�X�e�[�W�B
        {
            imageText_Stage_ConcreteObj.SetActive(true);//
            imageText_Stage_GridObj.SetActive(false);
            imageText_Stage_HighPlaceObj.SetActive(false);
        }
        else if(pdManager.stageNumberInt == 3)//HighPlace�X�e�[�W�B
        {
            imageText_Stage_HighPlaceObj.SetActive(true);
            imageText_Stage_GridObj.SetActive(false);
            imageText_Stage_ConcreteObj.SetActive(false);//
        }
    }
}
