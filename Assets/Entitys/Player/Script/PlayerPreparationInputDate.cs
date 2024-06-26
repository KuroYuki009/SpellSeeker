using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PlayerPreparationInputDate : MonoBehaviour
{
    PlayingData playingData;
    public PlayableDateManager pDM;

    public bool PID_InputSafety;//入力が出来るかどうかの安全装置。(falseで解除状態)

    public GameObject headObg;
    public GameObject bodyObj;
    public GameObject cursorObj;
    public GameObject cursorArrowObj;

    //段落を数値として処理する為に使用する。
    //メインツリー数値。
    int treeParagraphInt;//メインで使用される行。
    int treeParagraphInt_Max = 0;//最大値
    int treeParagraphInt_Min = -1;//最小値

    //キャラカスタマイズ数値。
    /*
    int tree_CharaCustom_Main_Int;//キャラカスタムのメインで使用される行。
    int tree_CharaCustomInt_Main_Max = 0;//キャラカスタムの最大値
    int tree_CharaCustomInt_Main_Min = -1;//キャラカスタムの最小値
    */
    int tree_CharaCustomInt_VC_Int;//キャラカスタム内のビジュアルカラーで使用される行。
    int tree_CharaCustomInt_VC_Max = 8;//ビジュアルカラーの最大値。
    int tree_CharaCustomInt_VC_Min = 0;//ビジュアルカラーの最小値。
    /*
    //パレットカスタム数値。
    int tree_PalletCustom_Main_Int;//パレットカスタムのメインで使用される行。
    int tree_PalletCustom_Main_Max;//パレットカスタムの最大値
    int tree_PalletCustom_Main_Min;//パレットカスタムの最小値

    int tree_PalletCustom_SA_Int;//パレットカスタム内のセカンドアクションで使用される行。
    int tree_PalletCustom_SA_Max = 3;//セカンドアクションの最大値。
    int tree_PalletCustom_SA_Min = 0;//セカンドアクションの最小値。
    */

    //エクストラツリー数値。
    int lowerTree_Paragraph_Int;//下部バーの選択を行う為に使用される。
    int lowerTree_Paragraph_Max;//下部バー選択の最大値。
    int lowerTree_Paragraph_Min;//下部バー選択の最小値。




    //ウィンドウ起動すいっち。
    // public bool charaCustomSW;//キャラカスタマイズWindowに移動しているかを二極値で表している。
    // public bool palletCustomSW;//パレットカスタマイズに移動しているかを二極値で表している。
    public bool readySW;//準備完了に移動しているかを二極値で表している。

    public List<GameObject> pick_Cursor_sprite;//カーソルスプライトを切り替える。アタッチ必須。

    public Image colorPanel_Image;//イメージカラーを変更する際の描写Image。アタッチ必須。
    public List<Color32> colorDate;//イメージカラーを変更する際に参照する場所。

    public Image PCW_WindowBackImage;//ウィンドウの背景イメージ。アタッチ必須。
    public GameObject Ready_ImageGroup;//「準備完了」を意味するイメージ。
    public Image Ready_Image;//「準備完了」を意味するイメージ。

    //効果音
    AudioSource ppid_AS;
    public AudioClip cursorMoveSE;
    public AudioClip enterSE;
    public AudioClip cancelSE;

    void Start()
    {
        playingData = GetComponent<PlayingData>();
        ppid_AS = GetComponent<AudioSource>();
        PlayerInput pi = GetComponent<PlayerInput>();//プレイヤーインプットを取得。

        pi.currentActionMap = pi.actions.actionMaps[1];//取得したPlayerInputのアクションマップを1番目(CharaSetting)に変更。
        tree_CharaCustomInt_VC_Int = playingData.playerNumber;//プレイヤーナンバーをカラーCustomのIntに合わせる。

        inputValueRefresh();

        WindowbackUpdate();
        TreeVisRefresh();//表示内容の更新を行う。
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

    public void inputValueRefresh()//呼び出すことですべてのUI設置値を初期化させる。(InGameManagerがメニューに戻ってきた際に呼び出す。)
    {
        readySW = false;
        Ready_ImageGroup.SetActive(false);
        treeParagraphInt = 0;

        TreeVisRefresh();//表示内容の更新を行う。
    }

    public void UpPoint(InputAction.CallbackContext context)//上に移動。
    {
        if (context.performed)
        {
            if (treeParagraphInt < treeParagraphInt_Max && readySW == false)
            {
                ppid_AS.PlayOneShot(cursorMoveSE);//効果音を鳴らす。
                treeParagraphInt += 1;
                Debug.Log(treeParagraphInt);
                TreeVisRefresh();//表示内容の更新を行う。
            }
            /*
            if (charaCustomSW == true)
            {
                if (tree_CharaCustom_Main_Int < tree_CharaCustomInt_Main_Max)
                {
                    tree_CharaCustom_Main_Int += 1;
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            else if (palletCustomSW == true)
            {
                if(tree_PalletCustom_Main_Int < tree_PalletCustom_Main_Max)
                {
                    tree_PalletCustom_Main_Int += 1;
                }
            }
            else if (readySW == true)//Ready状態の場合。
            {

            }
            else//何も選択している判定が無い場合。(メインツリーを選択状態。)
            {
                if (treeParagraphInt < treeParagraphInt_Max)
                {
                    treeParagraphInt += 1;
                    Debug.Log(treeParagraphInt);
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            */
        }
    }

    public void DownPoint(InputAction.CallbackContext context)//下に移動。
    {
        if (context.performed) 
        {
            if (treeParagraphInt > treeParagraphInt_Min && readySW == false)
            {
                ppid_AS.PlayOneShot(cursorMoveSE);//効果音を鳴らす。
                treeParagraphInt -= 1;
                Debug.Log(treeParagraphInt);
                TreeVisRefresh();//表示内容の更新を行う。
            }
            /*
            if(charaCustomSW == true)
            {
                if (tree_CharaCustom_Main_Int > tree_CharaCustomInt_Main_Min)
                {
                    tree_CharaCustom_Main_Int -= 1;
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            else if(palletCustomSW == true)
            {
                if(tree_PalletCustom_Main_Int > tree_PalletCustom_Main_Min)
                {
                    tree_PalletCustom_Main_Int -= 1;
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            else if(readySW == true)//Ready状態の場合。
            {

            }
            else//何も選択している判定が無い場合。(メインツリーを選択状態。)
            {
                if (treeParagraphInt > treeParagraphInt_Min)
                {
                    treeParagraphInt -= 1;
                    Debug.Log(treeParagraphInt);
                    ColorVisRefresh();//カラー表示内容の更新を行う。
                }
            }
            */
        }
    }

    public void RightInout(InputAction.CallbackContext context)//右入力。
    {
        if (context.performed)
        {
            if (treeParagraphInt == 0)
            {
                if (tree_CharaCustomInt_VC_Int < tree_CharaCustomInt_VC_Max)
                {
                    ppid_AS.PlayOneShot(cursorMoveSE);//効果音を鳴らす。
                    tree_CharaCustomInt_VC_Int += 1;
                    ColorVisRefresh();//カラー表示内容の更新を行う。
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
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            else if (palletCustomSW == true)
            {
                if(tree_PalletCustom_SA_Int < tree_PalletCustom_SA_Max)
                {
                    tree_PalletCustom_SA_Int += 1;
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            */
        }
    }
    public void LeftInput(InputAction.CallbackContext context)//左入力。
    {
        if (context.performed)
        {
            if (treeParagraphInt == 0)
            {
                if (tree_CharaCustomInt_VC_Int > tree_CharaCustomInt_VC_Min)
                {
                    ppid_AS.PlayOneShot(cursorMoveSE);//効果音を鳴らす。
                    tree_CharaCustomInt_VC_Int -= 1;
                    ColorVisRefresh();//カラー表示内容の更新を行う。
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
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            else if (palletCustomSW == true)
            {
                if (tree_PalletCustom_SA_Int > tree_PalletCustom_SA_Min)
                {
                    tree_PalletCustom_SA_Int -= 1;
                    TreeVisRefresh();//表示内容の更新を行う。
                }
            }
            */
        }
    }

    public void InputEnter(InputAction.CallbackContext context)//決定ボタンの入力。
    {
        if (context.performed)
        {
            if(treeParagraphInt == 0)//イメージカラーを決定入力する。
            {
                ppid_AS.PlayOneShot(enterSE);//効果音を鳴らす。
                WindowbackUpdate();
            }
            else if(treeParagraphInt == -1)
            {
                ppid_AS.PlayOneShot(enterSE);//効果音を鳴らす。
                SetEnter();
            }
            /*
            if(charaCustomSW == true)//キャラカスタムを選択している判定がある場合。
            {
                //もし決定ボタンで設定するようにするならばここの処理で変数を定義させる処理を行う必要がある。
            }
            else if(palletCustomSW == true)//パレットカスタムを選択している判定がある場合。
            {
                //もし決定ボタンで設定するようにするならばここの処理で変数を定義させる処理を行う必要がある。
            }
            else//何も選択している判定が無い場合。
            {
                if (treeParagraphInt == 0)
                {
                    charaCustomSW = true;//キャラカスタムを選択している。
                }
                else if (treeParagraphInt == -1)
                {
                    palletCustomSW = true;//パレットカスタムを選択している。
                }
                else if (treeParagraphInt == -2)
                {
                    readySW = true;
                }
            }
            */
        }
    }

    public void InputCancel(InputAction.CallbackContext context)//拒否ボタンの入力。
    {
        if (context.performed)
        {
            if(PID_InputSafety == false)
            {
                if (treeParagraphInt == -1)
                {
                    ppid_AS.PlayOneShot(cancelSE);//効果音を鳴らす。
                    readySW = false;
                    Ready_ImageGroup.SetActive(false);
                }
            }
        }
    }

    void TreeVisRefresh()//メインツリーの表示内容の更新を行う。
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

    void WindowbackUpdate()//色の適応。
    {
        Color32 color_D = colorDate[tree_CharaCustomInt_VC_Int];

        
        playingData.playerVisualColor = color_D;
        color_D.a = 200;
        PCW_WindowBackImage.color = color_D;
        color_D.a = 150;
        Ready_Image.color = color_D;
    }
    void ColorVisRefresh()//サンプルイメージカラーの表示内容の更新を行う。
    {
        if(colorPanel_Image.color != colorDate[tree_CharaCustomInt_VC_Int])
        {
            colorPanel_Image.color = colorDate[tree_CharaCustomInt_VC_Int];
        }
    }

    void SetEnter()//準備が終わり、装備が決定した。
    {
        playingData.teamNumber = playingData.playerNumber;//[テスト機能]自身の番号をチーム番号に張りつける。
        Color32 color_D = colorDate[tree_CharaCustomInt_VC_Int];
        pDM.playerCursorMaterials[playingData.playerNumber].color = color_D;
        pDM.playerLockOnLineMaterial[playingData.playerNumber].color = color_D;
        pDM.playerOverRayMaterial[playingData.playerNumber].color = color_D;

        pDM.playableChara_UI_PalletFrame_Image[playingData.playerNumber].color = color_D;
        readySW = true;
        pDM.ReadySW_detection();//PDMの全員がReadyを押せたかを検知する。();//PDMの全員がReadyを押せたかを検知する。
        Ready_ImageGroup.SetActive(true);
    }
}
