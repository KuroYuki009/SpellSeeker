using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAdopt_Card : MonoBehaviour
{
    //�擾�\�J�[�h�̃v�[�����w���B
    public PoolCardDate conflictPCD;

    PlayerGSWInput playerGSW_input;

    public GameObject choiceCountVis_Group;
    public Text choiceCountVis_Text;

    [SerializeField]List<SpellData> selectCardDate;//�I�����ꂽ�J�[�h�f�[�^�B�����GSWInput���Ɏ����Ă����B
    //
    [SerializeField]int chooseableCountInt;//�I���񐔁B�񐔕��I�����鎖���o����B
    int giveValueInt;
    public string RouteString;
    void Start()
    {
        playerGSW_input = GetComponent<PlayerGSWInput>();
    }

    private void Update()
    {
        switch(RouteString)
        {
            case "SelectPickStandby"://�I�������܂ł̑ҋ@�B

                break;

            case "DrawProcess_Conflict_Shield":
                AddCard_Conflict_Shield_Process();
                break;

            case "SelectPick_End":
                SelectPickEnd();
                break;
        }
    }

    void VisChoiceCountRefresh()
    {
        if(chooseableCountInt == 0)
        {
            choiceCountVis_Group.SetActive(false);
        }
        else
        {
            choiceCountVis_Group.SetActive(true);
            choiceCountVis_Text.text = chooseableCountInt.ToString();
        }
    }

    public void AddCard_Conflict_Shield(int giveValue,int rollcount)//�v���C���[�Ƀ����_���ȃJ�[�h�̑I������^����B(count�ɂ͉��񒊑I���邩������K�v������B)
    {
        giveValueInt = giveValue;
        if(chooseableCountInt == 0)
        {
            chooseableCountInt = rollcount;
        }
        else
        {
            chooseableCountInt += rollcount;
        }

        RouteString = "DrawProcess_Conflict_Shield";
    }

    void AddCard_Conflict_Shield_Process()
    {
        chooseableCountInt -= 1;
        VisChoiceCountRefresh();//�\�L�̍X�V�B

        // ���I���s�������BpoolCardDate_1�̓X�^���_�[�h�J�[�h�̃v�[���B
        int poolCD_MaxSizeInt_1 = conflictPCD.poolCardDate_1.Count;

        if (selectCardDate != null) selectCardDate.Clear();//���ׂč폜����B

        for (int i = 0; giveValueInt > i; i++)
        {
            int rundomInt = Random.Range(0, poolCD_MaxSizeInt_1);//�J�[�h�v�[���Ɋ܂܂��J�[�h�������ő�l�ɐݒ肵�A����ׂɐ�����I�o�B
            //Debug.Log(conflictPCD.poolCardDate_1[rundomInt].spellName + selectCardDate.Count);

            selectCardDate.Add(conflictPCD.poolCardDate_1[rundomInt]);// conflict�p�̃J�[�h�v�[������1�ԃv�[��(standard)�̒�����I�o�B������i�[����B
        }
        playerGSW_input.pick_SpellDates = selectCardDate;//�I�o�����J�[�h�f�[�^��GSW���Ɉڂ��B
        playerGSW_input.loadType = "SpellDate";//�ǂݍ��݃^�C�v��SpellData�ɐݒ�B
        playerGSW_input.rootswitch = "SetUpLoad";//�Z�b�g�A�b�v�������s����������I��ҋ@��Ԃɂ�����B

        RouteString = "SelectPickStandby"; 
    }

    void SelectPickEnd()
    {
        if (chooseableCountInt > 0)
        {
            RouteString = "DrawProcess_Conflict_Shield";
        }
        else
        {
            playerGSW_input.SelectEnd();//�I���I���̃R�[���o�b�N���s���B
            Debug.Log("�I���I��");

            RouteString = null;
        }
    }
    public void Select_PickCallBack()//�O������̃A�N�Z�X�ō쓮������B
    {
        RouteString = "SelectPick_End";
        playerGSW_input.rootswitch = null;
    }

    
}
