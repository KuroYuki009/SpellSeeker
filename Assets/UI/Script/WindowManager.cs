using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WindowManager : MonoBehaviour
{
    public PlayableDateManager pbDM;
    public PlayerPreparationInputDate p_PID; //PlayerPreparationInputDate

    public RectTransform rect_MenuUI;//���j���[����UI���܂Ƃ߂�RTF�B
    public RectTransform rect_InGameUI;//�Q�[�������Ŏg�p����UI���܂Ƃ߂�RectTF�B
    public RectTransform rect_PCWGroup;//�J�X�^���E�B���h�E���܂Ƃ߂�RectTF�B
    public RectTransform rect_GSWGroup;//�ėp�Z���N�g�E�B���h�E���܂Ƃ߂�RectTF�B

    public List<RectTransform> pcw_snapRTFs;//pcw�p�̃X�i�b�v�|�C���g�B
    public List<RectTransform> gsw_snapRTFs;//pcw�p�̃X�i�b�v�|�C���g�B

    public List<GameObject> fillSpriteObject;//JtPSprite�B

�@�@public List<JoinToPushSpriteController> spriteController;//JtPS�̃X�N���v�g���i�[�B
    int stanbyCount;//�ҋ@���Ă���Player�ԍ��B

    //�v���C���[�̃p���b�gUI���X�i�b�v����ۂɎg�p����B
    public List<RectTransform> snapPoint_Classic;
    public List<RectTransform> snapPoint_Corner;
    public List<RectTransform> snapPoint_Duel;

    //�ۊǐ��l 
    bool joinSpOpenSW;
    float joinSpOpenElapseTime;
    void Start()
    {
        if (pbDM == null) pbDM = GetComponent<PlayableDateManager>();

        SetUp();
    }

    void Update()
    {
       
    }

    void SetUp()
    {
        int count = fillSpriteObject.Count;
        for (int i = 0; count > i; i++)
        {
            JoinToPushSpriteController ptpSC = fillSpriteObject[i].GetComponent<JoinToPushSpriteController>();
            ptpSC.wm = gameObject.GetComponent<WindowManager>();
            spriteController.Add(ptpSC);
            
        }
    }

    public void WindowSetUp(int number) //PlayableDateManager�Ŏ擾����������b��UI�����w�肳�ꂽ�ꏊ�Ɉړ��A�Ȃ�тɊi�[���s���B
    {
        stanbyCount = number;//�ҋ@���l�ɓ����B

        RectTransform pcw_RT = pbDM.playableChara_UI_PCW[number].GetComponent<RectTransform>();//�J�X�^���E�B���h�E�̍��W���擾�B
        RectTransform gsw_RT = pbDM.playableChara_UI_GSW[number].GetComponent<RectTransform>();//�ėp�^�Z���N�g�E�B���h�E�̍��W���擾�B
        pbDM.playableChara_UI_GSW[number].SetActive(false);//���W���擾�������_�ł���P�̂𖳌������Ă����B

        RectTransform pallet_RT = pbDM.playableChara_UI_Pallet[number].GetComponent<RectTransform>();//�p���b�gUI�̍��W���擾�B

        pcw_RT.transform.SetParent(rect_PCWGroup.transform,false);
        gsw_RT.transform.SetParent(rect_GSWGroup.transform, false);
        
        pallet_RT.transform.SetParent(rect_InGameUI.transform, false);

        pcw_RT.position = pcw_snapRTFs[number].position;//�J�X�^���E�B���h�E�̈ʒu��ύX�B
        gsw_RT.position = gsw_snapRTFs[number].position;//�ėp�Z���N�g�E�B���h�E�̈ʒu��ύX�B

        Destroy(pbDM.playable_CanvasOBJ);//�J�X�^���E�B���h�E�ƃp���b�g�V�X�e��UI�����L�����o�X�Ɉڂ����猳�̃L�����o�X���폜����B

        spriteController[number].Join_FillAnima(number);

    }
    /*
    public void WindowLeaving(int Number)
    {
        spriteController[Number].Leaving_Window();
    }
    */

    public void PCW_Open(int Number)
    {
        pbDM.playableChara_UI_PCW[Number].GetComponent<Animator>().SetTrigger("OpenWindow_Trigger");
    }

    public void Snap_Corner()//�p�ɃX�i�b�v����B
    {
        if (pbDM.joinPlayerInt == 2 || pbDM.joinPlayerInt == 1)
        {
            for (int i = 0; i < pbDM.joinPlayerInt; i++)
            {
                RectTransform rect = pbDM.playableChara_UI_Pallet[i].GetComponent<RectTransform>();
                rect.pivot = snapPoint_Duel[i].pivot;
                rect.transform.position = snapPoint_Duel[i].transform.position;
            }
        }
        else
        {
            for (int i = 0; i < pbDM.joinPlayerInt; i++)
            {
                RectTransform rect = pbDM.playableChara_UI_Pallet[i].GetComponent<RectTransform>();
                rect.pivot = snapPoint_Corner[i].pivot;
                rect.transform.position = snapPoint_Corner[i].transform.position;
                
            }
        }

    }
}
