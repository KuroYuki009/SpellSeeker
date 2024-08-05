using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayableDateManager : MonoBehaviour
{
    //このスクリプトはプレイヤーの操作キャラとそのキャラのUIを同時に格納し、管理する為のスクリプトになります。
    //またどのシーンにもこのスクリプトを保持する必要があります。

    PlayerInputManager playerInputManager;
    WindowManager windowManager;//CustomWindowのスナップを行ったり制御を行う。
    SceneTransitionManager scene_TM;//シーン遷移に使用される専用のスクリプト。


    public int joinPlayerInt;//ゲームに参加しているプレイヤー人数。
    public int maxEntryJoinPlayerInt;//現在、最大何人まで入れるか。(これにより参加可能人数を制限する。またこれは自然数で表す。)


    string unPlayableSearchTag = "UnknownPlayer";//入室した未加入プレイヤーを探す際に使用するタグ。
    string joinPlayableTag = "JoinPlayer";//加入したプレイヤーに使用するタグ。

    [Space]

    public GameObject playersPrefab_Group;//生成したプレイアブルプレハブをまとめるグループオブジェクト

    [HideInInspector] public List<GameObject> playableChara_OBJ;//操作キャラ本体。操作キャラなる プレイアブルオブジェクト そのものを指す。

    ////--------------------------------------------------

    //// 各オブジェクト群 ///---
    #region
    [HideInInspector] public GameObject playable_CanvasOBJ;// 各プレイヤーが所有する キャンバスオブジェクト が格納されています。

    [HideInInspector] public List<GameObject> playableChara_UI_PCW;// 各プレイヤーが所有する UI系の PlayerCustomWindow が格納されています。

    [HideInInspector] public List<GameObject> playableChara_UI_GSW;// 各プレイヤーが所有する UI系の GeneralSelectWindow が格納されています。

    [HideInInspector] public List<GameObject> playableChara_UI_Pallet;// 各プレイヤーが所有する パレットUI が格納されています。

    [HideInInspector] public List<Image> playableChara_UI_PalletFrame_Image;// パレットUI のフレームが格納されています。

    [HideInInspector] public List<Image> playableCharaUI_DestroyCover;// パレットUI に紐図けられている 撃破カバー が格納されています。
    #endregion


    //// 各プレイヤー に 付与されたスクリプト群 ///---
    #region
    [HideInInspector] public List<PlayingData> playeable_PD;// 各プレイヤーに付与されているスクリプト体 PlayingData が格納されています。

    [HideInInspector] public List<StatusManager> playableChara_SM;// 各プレイヤーに付与されているスクリプト体 StatusManager が格納されています。

    [HideInInspector] public List<PlayerMoving> playableChara_PM;// 各プレイヤーに付与されているスクリプト体 PlayerMoving が格納されています。

    [HideInInspector] public List<HandCardManager> playableChara_HCM;// 各プレイヤーに付与されているスクリプト体 HandCardManager が格納されています。

    [HideInInspector] public List<PlayerUIManager> playableChara_UI_Manager;// 各プレイヤーに付与されているスクリプト体 PlayerUIManager が格納されています。

    [HideInInspector] public List<PlayerPreparationInputDate> playableChara_PID;// 各プレイヤーに付与されているスクリプト体 PlayerPreparationInputDate が格納されています。

    [HideInInspector] public List<PlayerGSWInput> PlayeableChara_GSW;// 各プレイヤーに付与されているスクリプト体 PlayerGeneralSelectWindow が格納されています。

    [HideInInspector] public List<PlayerAdopt_Card> playeableChara_Adopt_Card;//  各プレイヤーに付与されているスクリプト体 PlayerGeneralSelectWindow が格納されています。
    #endregion


    //各要素の処理を行うScriptをアタッチする必要がある。

    [Header("カラーリング・マテリアル")]
    #region
    [Tooltip("プレイヤーが使用可能なプリセットのカラーリング値。")]
    public List<Color32> presetPlayerColor;//プリセットのプレイヤーのカラーリング群。インスペクターから設定可能。

    [Tooltip("プレイヤーに付与される色判別用のマテリアル。")]
    public List<Material> playerCursorMaterials;//プレイヤーに付与するマテリアル。インスペクターからアタッチ必須。

    [Tooltip("プレイヤーの使用する ロックオンライン のマテリアル。")]
    public List<Material> playerLockOnLineMaterial;//プレイヤーの使用するロックオンラインのマテリアル。インスペクターからアタッチ必須。

    [Tooltip("プレイヤーがカメラから見えない 遮蔽物 に隠れた際に 描画されるマテリアル。")]
    public List<Material> playerOverRayMaterial;//プレイヤーの透過時のマテリアル。インスペクターからアタッチ必須。
    #endregion

    ////--------------------------------------------------

    [HideInInspector] public int stageNumberInt;//ロードさせるシーン番号。

    ////--------------------------------------------------

    void Start()
    {
        maxEntryJoinPlayerInt = 4;
        stageNumberInt = 1;
        playerInputManager = GetComponent<PlayerInputManager>();
        windowManager = GetComponent<WindowManager>();
        scene_TM = GetComponent<SceneTransitionManager>();
        stageNumberInt = 1;
    }

    public void OnPlayerJoined(PlayerInput playerInput)//対応したボタンを押して入室した場合。
    {
        print($"プレイヤー#{playerInput.user.index}が入室！");

        int i = playerInput.user.index;
        joinPlayerInt++;

        GameObject joinPlayables = GameObject.FindWithTag(unPlayableSearchTag);//未登録プレイヤーを専用のタグで検知、取得する。

        if(maxEntryJoinPlayerInt >= (i + 1))
        {
            joinPlayables.tag = joinPlayableTag;
            joinPlayables.transform.parent = playersPrefab_Group.gameObject.transform;
            PlayablePrefabManager pm = joinPlayables.GetComponent<PlayablePrefabManager>();//入室したプレイヤーからPlayablePrefabManagerを取得する。


            pm.PalletFrame_NumberTag_Text.text = joinPlayerInt.ToString();//プレイヤーの番号を入れる。
            pm.PCW_NumberTag_Image[playerInput.user.index].enabled = true;//プレイヤーの番号通りのImageObjectを表示する。
            pm.playable_CursorRing.GetComponent<SpriteRenderer>().material = playerCursorMaterials[playerInput.user.index];
            pm.playable_DireArrow.GetComponent<SpriteRenderer>().material = playerCursorMaterials[playerInput.user.index];
            pm.playableObject.GetComponent<LineRenderer>().material = playerLockOnLineMaterial[playerInput.user.index];


            pm.playableObject.GetComponent<PlayerPreparationInputDate>().pDM = GetComponent<PlayableDateManager>();
            PlayingData pd = pm.playableObject.GetComponent<PlayingData>();
            pd.playerNumber = playerInput.user.index;//入室番号をプレイヤーNumberに割り当てる。
            pd.playerVisualColor = presetPlayerColor[playerInput.user.index];

            playableChara_OBJ.Add(pm.playableObject);//操作キャラを取得。
            playable_CanvasOBJ = pm.canvasOBJ;
            playableChara_UI_PCW.Add(pm.ui_CustomWindow);//Ui_PlayerCastomWindowを取得。
            playableChara_UI_GSW.Add(pm.ui_GeneralSelectWindow);//ui_GeneralSelectWindowを取得。

            playableChara_UI_Pallet.Add(pm.ui_Pallet);//UI_Palletを取得。
            playableChara_UI_PalletFrame_Image.Add(pm.ui_Pallet_Frame);
            playableCharaUI_DestroyCover.Add(pm.ui_DestroyCover);
            playableChara_SM.Add(pm.playable_SM);//StatusManagerを取得。
            playeable_PD.Add(pm.playable_PD);//PlayingDataを取得。
            playableChara_PM.Add(pm.playable_PM);//PlayerMovingを取得。
            playableChara_HCM.Add(pm.playable_HCM);//HandCardManagerを取得。
            playableChara_UI_Manager.Add(pm.playable_UI_Manager);//StatusUIManagerを取得。
            playableChara_PID.Add(pm.playable_PID);//PlayerPreparationInputDateを取得。
            PlayeableChara_GSW.Add(pm.playable_GSW);//PlayerGSWInputを取得。
            playeableChara_Adopt_Card.Add(pm.playeable_Adopt_Card);//PlayerAdopt_Cardを取得。


            windowManager.WindowSetUp(i);//Canvasに入ってるUI群を一つのキャンバスに格納する。


            joinPlayables = null;
            pm = null;
        }
        else//違った場合。
        {
            joinPlayerInt--;

            Destroy(joinPlayables);// 入った相手を破棄する。

            Debug.Log("入室上限を超えました。");
        }
    }

    // プレイヤー退室時に受け取る通知
    public void OnPlayerLeft(PlayerInput playerInput)//削除(無効化)された場合。
    {
        print($"プレイヤー#{playerInput.user.index}が退室！");
    }

    public void ReadySW_detection()//全員のReadySWがtreuになっているかを調べ、条件を満たしていたら遷移Managerのロード関数を呼び出す。
    {
        if(joinPlayerInt >= 2)
        {
            int t_Count = 0;

            for (int i = 0; i < playableChara_PID.Count; i++)
            {
                if (playableChara_PID[i].readySW == true)
                {
                    t_Count++;
                }
            }

            if (t_Count == playableChara_PID.Count)
            {
                Debug.Log("準備完了");
                windowManager.Snap_Corner();//パレットをコーナーにスナップする。(デュエル時には左右設置。)

                for (int i = 0; i < playableChara_PID.Count; i++)
                {
                    playableChara_PID[i].PID_InputSafety = true;//PIDすべてのセルフティをオンにする。
                }

                scene_TM.loadStageNamber = stageNumberInt;//遷移先のナンバーを渡す。
                scene_TM.StartLoad();//シーン遷移を開始する。
            }
        }
    }

    

    
}
