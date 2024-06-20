using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayableDateManager : MonoBehaviour
{
    //このスクリプトはプレイヤーの操作キャラとそのキャラのUIを同時に格納し、管理する為のスクリプトになります。
    //またどのシーンにもこのスクリプトを保持する必要があります。

    PlayerInputManager playerInputManager;/* ボタンを押すとゲームへの参加を行う。
    インゲーム側では無効化する事が推奨。(再接続的に上手くいかない場合がある為。
    無力化中に操作キャラが一度でも無力化すると権利をはく奪される為注意。)　*/
    int playerGrantNumberCount;//プレイヤーに付与するナンバーを格納する。これは処理の中で使用される。
    public int maxEntryJoinPlayerInt;//現在、最大何人まで入れるか。(これにより参加可能人数を制限する。またこれは自然数で表す。)
    public int joinPlayerInt;//ゲームに参加しているプレイヤー人数。

    string unPlayableSearchTag = "UnknownPlayer";//入室した未加入プレイヤーを探す際に使用するタグ。
    string joinPlayableTag = "JoinPlayer";//加入したプレイヤーに使用するタグ。

    public GameObject playersPrefab_Group;//生成したプレイアブルプレハブをまとめるオブジェクト

    //各プレイアブルキャラのスクリプト取得。
    public List<GameObject> playableChara_OBJ;//操作キャラ本体。

    public GameObject playable_CanvasOBJ;

    public List<GameObject> playableChara_UI_PCW;//UI_PlayerCustomWindow

    public List<GameObject> playableChara_UI_GSW;//UI_GeneralSelectWindow

    public List<GameObject> playableChara_UI_Pallet;//UI_Pallet。

    public List<Image> playableChara_UI_PalletFrame_Image;//UI_Palletのフレーム。

    public List<Image> playableCharaUI_DestroyCover;//UI_Destroyのカバーフレーム。

    public List<PlayingData> playeable_PD;//PlayingData。

    public List<StatusManager> playableChara_SM;//ステータスマネージャー。

    public List<PlayerMoving> playableChara_PM;//プレイヤーマネージャー。

    public List<HandCardManager> playableChara_HCM;//ハンドカードマネージャー。

    public List<PlayerUIManager> playableChara_UI_Manager;//プレイヤーUIマネージャー。

    public List<PlayerPreparationInputDate> playableChara_PID;//プレイヤーの設定画面の入力値保管。

    public List<PlayerGSWInput> PlayeableChara_GSW;//汎用選択枠の操作用スクリプト。

    public List<PlayerAdopt_Card> playeableChara_Adopt_Card;//プレイヤーのカードを受け渡すためのスクリプト。

    //各要素の処理を行うScriptをアタッチする必要がある。

    WindowManager windowManager;//CustomWindowのスナップを行ったり制御を行う。

    public List<Color32> presetPlayerColor;//プリセットのプレイヤーのカラーリング群。インスペクターから設定可能。

    public List<Material> playerCursorMaterials;//プレイヤーに付与するマテリアル。インスペクターからアタッチ必須。

    public List<Material> playerLockOnLineMaterial;//プレイヤーの使用するロックオンラインのマテリアル。インスペクターからアタッチ必須。

    public List<Material> playerOverRayMaterial;//プレイヤーの透過時のマテリアル。インスペクターからアタッチ必須。

    int depositJoinPlayer;//一時的に参加中のプレイヤー人数を取得する。これはインゲーム中に飛び入り参加を無効にするために使用される。
    SceneTransitionManager scene_TM;

    //
    public int stageNumberInt = 1;//ロードさせるステージ番号。1でGrid、2でConcrete。など.....という感じになっている。
    void Start()
    {
        maxEntryJoinPlayerInt = 4;
        playerInputManager = GetComponent<PlayerInputManager>();
        windowManager = GetComponent<WindowManager>();
        scene_TM = GetComponent<SceneTransitionManager>();
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

            Destroy(joinPlayables);//サーチアンドデストロイッッ。サーチアンドデストロイッ!!

            Debug.Log("入室上限を超えました。");
        }
    }

    // プレイヤー退室時に受け取る通知(本来使用しない機能の為、内容不明確。)
    public void OnPlayerLeft(PlayerInput playerInput)//削除(無効化)された場合。
    {
        print($"プレイヤー#{playerInput.user.index}が退室！");
    }

    void PlayableDateInit()//プレイアブルデータの初期定義。
    {

    }

    public void ReadySW_detection()//全員のReadySWがtreuになっているかを調べる。
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

                scene_TM.loadStageNamber = stageNumberInt;//ステージGridに遷移させる。
                scene_TM.StartLoad();//シーン遷移を開始する。
            }
        }
    }

    

    
}
