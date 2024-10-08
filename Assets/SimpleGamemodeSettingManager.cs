using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class SimpleGamemodeSettingManager : MonoBehaviour
{
    ////このスクリプトはいつでも名前、所属を変更出来るようにする必要がある為、必ず本スクリプトの参照等の行為を行う際は記述すること。
    //>現在どこもこのスクリプトを取得していません。good!

    InGameManager inGameManager;
    PlayableDateManager pdManager;
    SceneTransitionManager stm;

    //アタッチ類
    public GameObject imageText_AddSubSc_StarterDecksObj;//イメージテキスト群「AddSubSc_StarterDeck」
    public GameObject imageText_AddSubSc_ShieldBuildsObj;//イメージテキスト群「AddSubSc_ShieldBuild」

    public Text mapNameDisplayText;
    //
    int stageNumberInt_min;//最小値。
    int stageNumberInt_max;//最大値。

    void Start()
    {
        inGameManager = GetComponent<InGameManager>();
        pdManager = GetComponent<PlayableDateManager>();
        stm = GetComponent<SceneTransitionManager>();

        stageNumberInt_min = 1;
        stageNumberInt_max = 4;
        RefreshVisStageName();
        RefreshVisAddSubScMode();
    }

    void Update()
    {
        /*
        if(inGameManager.currentSceneDate.sceneGameMode == "Menu" || inGameManager.currentSceneDate == null)
        {
            
        }*/
        if (Input.GetKey("joystick button 3"))//Yボタン
        {
            
            if (Input.GetKeyDown("joystick button 5"))//RBボタン
            {
                Debug.Log("ステージ変更");
                if (pdManager.stageNumberInt < stageNumberInt_max)
                {
                    pdManager.stageNumberInt += 1;
                }
                RefreshVisStageName();
            }
            else if (Input.GetKeyDown("joystick button 4"))//LBボタン
            {
                Debug.Log("ステージ変更");
                if (pdManager.stageNumberInt > stageNumberInt_min)
                {
                    pdManager.stageNumberInt -= 1;
                }
                RefreshVisStageName();
            }

            
        }


        if (Input.GetKey("joystick button 2"))//Xボタン
        {
            if (Input.GetKeyDown("joystick button 5"))//LBボタン
            {
                Debug.Log("追加要素変更");
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

    public void RefreshVisAddSubScMode()//モードの追加要素の表記更新。
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

    public void RefreshVisStageName()//ステージの名前表記更新。
    {
        int i = pdManager.stageNumberInt;
        string stageName = stm.stageProfileDate[i].sceneVisName;
        mapNameDisplayText.text = stageName;
    }
}
