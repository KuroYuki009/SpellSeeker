using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAdopt_Card : MonoBehaviour
{
    //取得可能カードのプールを指す。
    public PoolCardDate conflictPCD;

    PlayerGSWInput playerGSW_input;

    public GameObject choiceCountVis_Group;
    public Text choiceCountVis_Text;

    [SerializeField]List<SpellData> selectCardDate;//選択されたカードデータ。これをGSWInput側に持っていく。
    //
    [SerializeField]int chooseableCountInt;//選択回数。回数分選択する事が出来る。
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
            case "SelectPickStandby"://選択完了までの待機。

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

    public void AddCard_Conflict_Shield(int giveValue,int rollcount)//プレイヤーにランダムなカードの選択権を与える。(countには何回抽選するかを入れる必要がある。)
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
        VisChoiceCountRefresh();//表記の更新。

        // 抽選を行う処理。poolCardDate_1はスタンダードカードのプール。
        int poolCD_MaxSizeInt_1 = conflictPCD.poolCardDate_1.Count;

        if (selectCardDate != null) selectCardDate.Clear();//すべて削除する。

        for (int i = 0; giveValueInt > i; i++)
        {
            int rundomInt = Random.Range(0, poolCD_MaxSizeInt_1);//カードプールに含まれるカード枚数を最大値に設定し、無作為に数字を選出。
            //Debug.Log(conflictPCD.poolCardDate_1[rundomInt].spellName + selectCardDate.Count);

            selectCardDate.Add(conflictPCD.poolCardDate_1[rundomInt]);// conflict用のカードプールから1番プール(standard)の中から選出。それを格納する。
        }
        playerGSW_input.pick_SpellDates = selectCardDate;//選出したカードデータをGSW側に移す。
        playerGSW_input.loadType = "SpellDate";//読み込みタイプをSpellDataに設定。
        playerGSW_input.rootswitch = "SetUpLoad";//セットアップ処理を行いそこから選択待機状態にさせる。

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
            playerGSW_input.SelectEnd();//選択終了のコールバックを行う。
            Debug.Log("選択終了");

            RouteString = null;
        }
    }
    public void Select_PickCallBack()//外部からのアクセスで作動させる。
    {
        RouteString = "SelectPick_End";
        playerGSW_input.rootswitch = null;
    }

    
}
