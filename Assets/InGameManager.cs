using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class InGameManager : MonoBehaviour
{
    public SceneTransitionManager sceneTM;
    public PlayableDateManager playableDM;
    
    public GameObject battleAnimationUI_Obj;//スタート時に再生するアニメーションのオブジェクトを格納。
    Animator battleAnima;//animatorを取得、格納。

    public GameObject PCW_Group;//プレイヤーカスタムウィンドウのグループ。
    public GameObject GSW_Group;//汎用セレクトウィンドウのグループ。
    public GameObject MenuUI_ExtrasGroup;//menuUI中の特殊なUIグループ。
    public GameObject PalletUI_Group;//プレイヤーのインゲームウィンドウのグループ。
    CanvasGroup PalletGroup_CanvasGroup;
    public GameObject PtJ_Group;//ジョインスプライトのグループ。

    //Scene入室後に処理を行う。
    public SceneProfileDate currentSceneDate;//現在のシーンの要素。

    //switch文に使用するstring型。各ゲームモードの処理に使用される。
    [SerializeField]string gameModeRoute;
    [SerializeField]bool inBattleSW;//戦闘中扱いかの二極値。(trueが戦闘中。)

    //ゲームモード追加モジュール。
    public bool shieldBuild_ModeSW = true;//シールド戦。二極値。

    public StatusDate mode_conflict_offsetStatusDate;//conflict時のオフセット(初期)ステータス。

    public List<GameObject> playableObject;//プレイヤーのプレイアブルオブジェクトを格納する。
    public List<StatusManager> playableStatusManager;//プレイヤーのステータスマネージャーを格納する。

    public List<Transform> playerSpawnPoint;//プレイヤーのスポーン地点。
    public Vector3 exclusionSpawnPoint;//倒された後に移動する場所。

    public GameObject[] beingPlayerObj = new GameObject[4];//生存中のプレイヤーオブジェクトを格納する為の配列。4つ枠を生成。
    public List<GameObject> eliminatePlayerObj;//倒されたプレイヤーオブジェクトを格納。
    GameObject depositRoundWinnerObj;//ラウンドに勝利したプレイヤーオブジェクトを一時格納する。
    public ParticleSystem eliminateEffect;//撃破された際に発生するパーティクル。

    public List<Sprite> numberSprites;//ラウンド終了時にに勝利したプレイヤーの数字を表示する際に使用する。
    public Image UI_winerNumberSprite;//ラウンド終了時に表示するImageUI。

    public Image GSW_BackCover_Image;//選択中、後ろを見えなくするカバーイメージ。

    bool slowTimeSW;//現在がスロー状態かを二極値で表す。
    float slowTimeScale = 0.1f;//どれくらい遅くなるかを設定する。デフォルトで0.2f;
    float finishSlowTime;

    //効果音等
    AudioSource inGameManagerAS;

    public AudioClip battleReadySE;//戦闘が開始する前の効果音。(AreYouReady?の辺りで流れる。)
    public AudioClip battleStartSE;//戦闘開始時に再生する効果音。(BattleStartの辺りで流れる。)
    public AudioClip battleEndE;//戦闘が終了した際に再生される音効果音。

    //--------

    //メニューシンプルビジュアル
    //SimpleGamemodeSettingManager sgsManager;
    void Start()
    {
        exclusionSpawnPoint = new Vector3(100, 0, 100);
        if (sceneTM == null) sceneTM = GetComponent<SceneTransitionManager>();
        if (playableDM == null) playableDM = GetComponent<PlayableDateManager>();
        battleAnima = battleAnimationUI_Obj.GetComponent<Animator>();
        inGameManagerAS = GetComponent<AudioSource>();
        PalletGroup_CanvasGroup = PalletUI_Group.GetComponent<CanvasGroup>();

        //sgsManager = GetComponent<SimpleGamemodeSettingManager>();

        //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("マウスカーソルを非表示にしました。");
    }

    void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)//ゲームの再起動。
        {
            Debug.Log("再起動する");

            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
            Application.Quit();
        }

        if (inBattleSW == true)
        {
            switch (gameModeRoute)
            {
                //コンフリクト系モードの処理-------------
                case "Conflict_Classic_inBattleProcess":
                    Conflict_Classic_inBattleProcess();
                    break;
                case "Conflict_EndRoundProcess":
                    Conflict_EndRoundProcess();
                    break;
                case "WhatNextGame":
                    WhatNextGame();
                    break;
            }
        }
        else
        {
            switch (gameModeRoute)
            {
                //ゲームモード追加モジュールの処理-------------
                case "ShieldBuild_PickStandby"://選択待ちの状態。
                    ShieldBuild_PickStandby();
                    break;
            }
        }
        

        if (slowTimeSW == true)
        {
            if (finishSlowTime <= 1.5f)
            {
                SlowTimeEffect_in();
                finishSlowTime += 1 * Time.unscaledDeltaTime;

            }
            else if (finishSlowTime >= 1.5f)
            {
                SlowTimeEffect_out();
                slowTimeSW = false;
            }
        }
        else if(slowTimeSW == false && finishSlowTime != 0)
        {
            finishSlowTime = 0f;
        }
    }
    public void SceneEntryProcess()//移行時に送られてくる信号で処理を開始。
    {
        gameModeRoute = null;
        DetectionCurrentMode();//ゲームモードを検出。
    }

    public void DetectionCurrentMode()//ゲームモードを検出。
    {
        Debug.Log("ゲームモードを検出します");
        inBattleSW = false;//一度BattleSWを無効化する。

        if (currentSceneDate.sceneGameMode == "Menu")
        {
            // 入室上限を元に戻す。
            playableDM.maxEntryJoinPlayerInt = 4;

            PalletUI_Group.SetActive(false);
            GSW_Group.SetActive(false);
            GSW_BackCover_Image.enabled = false;
            MenuFunctionRefresh();//メニューに遷移接続した際に機能を復旧させる。
        }
        else//それ以外の場合、
        {
            // 一度入室制限を掛ける。
            playableDM.maxEntryJoinPlayerInt = playableDM.joinPlayerInt;

            // 各種UIを一度非表示にする。
            PCW_Group.SetActive(false);
            GSW_Group.SetActive(false);
            GSW_BackCover_Image.enabled = false;
            MenuUI_ExtrasGroup.SetActive(false);

            PalletUI_Group.SetActive(true);
            PtJ_Group.SetActive(false);
            for (int c = 0; c < playableDM.playableChara_OBJ.Count; c++)
            {
                playableDM.playableChara_PM[c].CameraSearch();
                PlayerInput pI = playableDM.playableChara_OBJ[c].GetComponent<PlayerInput>();
                pI.currentActionMap = pI.actions.actionMaps[0];//取得したPlayerInputのアクションマップを0番目(Player)に変更。
            }

        }

        if (currentSceneDate.sceneGameMode == "Conflict_Classic")//ゲームモードが対戦モードである場合。
        {
            PalletGroup_AlphaHide();//プレイヤーパレット群のアルファ値を一度 ゼロにする。

            if (shieldBuild_ModeSW == true)//シールド戦がオンになっている場合、
            {
                //ゲームを開始する前にシールドデッキを組ませる。
                ShieldBuild_SetUp();

                PlayerSpawnPoint_Snap();
            }
            else//シールド戦がオンになっていない場合、
            {
                //そのままバトルフェイズに移行させる。
                for (int i = 0; i < playableDM.playableChara_OBJ.Count; i++)
                {
                    playableDM.playeable_PD[i].onHandDeckDate.Clear();
                }

                Conflict_Classic_SetUp();//セットアップの処理を行う。
                PlayerSpawnPoint_Snap();
                BeforeStartingAnima();// Animationを再生させ、終点のイベントにスタートさせる。
            }
        }
    }

    void MenuFunctionRefresh()//メインメニューへ戻る際の操作。
    {
        for (int c = 0; c < playableDM.joinPlayerInt; c++)
        {
            PCW_Group.SetActive(true);
            PtJ_Group.SetActive(true);
            MenuUI_ExtrasGroup.SetActive(true);

            PlayerPreparationInputDate ppID = playableDM.playableChara_PID[c];//取得。
            ppID.inputValueRefresh();//数値の初期化を行う。
            ppID.PID_InputSafety = false;

            PlayerInput pI = playableDM.playableChara_OBJ[c].GetComponent<PlayerInput>();
            pI.currentActionMap = pI.actions.actionMaps[1];//取得したPlayerInputのアクションマップを1番目(UI)に変更。

            Animator animator = playableDM.playableChara_UI_PCW[c].GetComponent<Animator>();
            animator.SetTrigger("OpenWindow_Trigger");
        }
    }

    void PlayerSpawnPoint_Snap()//プレイヤースポーン地点に移動させる。
    {
        //各プレイヤーの設定が終わった後にスポーン位置を変更。
        GameObject[] spawnPointObj = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");//ステージ上のスポーン地点を探す。
        for (int i = 0; i < spawnPointObj.Length; i++)//見つけたスポーン地点となるオブジェクトをTransformに変換し格納。
        {
            playerSpawnPoint.Add(spawnPointObj[i].GetComponent<Transform>());
        }

        int joinPlayerCount = playableDM.playableChara_OBJ.Count;
        List<GameObject> playerObj = playableDM.playableChara_OBJ;

        for (int c = 0; c < joinPlayerCount; c++)//スポーン地点をランダムに振り分ける。この場合設定されたスポーン地点の頭から使用される。
        {
            playerObj[c].transform.position = spawnPointObj[c].transform.position;
            playerObj[c].transform.rotation = spawnPointObj[c].transform.rotation;
        }
    }

    void BeforeStartingAnima()//戦闘開始前のアニメーションを再生。(開始前カウントダウンなど)
    {
        battleAnimationUI_Obj.SetActive(true);

        int BSC_T_ParamHash = Animator.StringToHash("BattleStartCutIn_Trigger");
        battleAnima.SetTrigger(BSC_T_ParamHash);

    }

    public void BattleReadySE_Play()//ゲーム開始前のサウンド再生を行う為のメゾット。
    {
        inGameManagerAS.PlayOneShot(battleReadySE);//ここでseを流す。
    }
    public void BattleStartSE_Play()//開始時のサウンド再生を行う為のメゾット。
    {
        inGameManagerAS.PlayOneShot(battleStartSE);//ここで開始のSEを再生させる。
    }
    public void BattleEndSE_Play()//戦闘が終了した際にサウンド再生を行う為のメゾット。
    {
        inGameManagerAS.PlayOneShot(battleEndE);
    }

    public void StartBattlePhase()////UIを再生し、終了を検知したらバトルフェイズを開始させる場合に読み込ませる。
    {
        for (int i = 0;i < playableDM.joinPlayerInt;i++)
        {
            Debug.Log("バトルフェイズ移行"+i); 
            //StartusManagerのSM_ProcessSafetyをオフにする。
            playableDM.playableChara_SM[i].SM_ProcessSafety = false;
            //PlayerMovingのスクリプトエナブルをオンにする。この時にカメラも渡す。
            PlayerMoving pm = playableDM.playableChara_PM[i];
            pm.enabled = true;
            GameObject mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");//
            pm.visCamera = mainCamObj.GetComponent<Camera>();
            playableDM.playableChara_PM[i].enabled = true;
            //HandCardManagerのHCM_ProcessSafetyをオフにする。
            playableDM.playableChara_HCM[i].HCM_ProcessSafety = false;

            StartCoroutine(PalletGroup_AlphaFadeIn());

            int playerTeam = playableObject[i].GetComponent<PlayingData>().teamNumber;
            if (playerTeam == 0)
            {
                playableObject[i].tag = "Player_1";
                playableObject[i].layer = 6;
            }
            else if (playerTeam == 1)
            {
                playableObject[i].tag = "Player_2";
                playableObject[i].layer = 7;
            }
            else if(playerTeam == 2)
            {
                playableObject[i].tag = "Player_3";
                playableObject[i].layer = 8;
            }
            else if (playerTeam == 3)
            {
                playableObject[i].tag = "Player_4";
                playableObject[i].layer = 9;
            }
            else
            {
                //playableDM.playableChara_OBJ[i].layer = LayerMask.GetMask(new string[] { "Player_1" });
                Debug.Log("プレイヤーのPlayingDataにチームが指定されていません。");
            }

            playableStatusManager[i].this_Eliminated = false;//あらかじめ撃破状態を解除させる。

        }

        inBattleSW = true;//戦闘中にする。
        gameModeRoute = "Conflict_Classic_inBattleProcess";
    }

    ////各ゲームモード処理。
    
    //モード_Conflict_Classic ------------------------
    void Conflict_Classic_SetUp()//コンフリクト・クラシックのセットアップ処理。終わると、次の処理に移行。
    {
        playableObject = playableDM.playableChara_OBJ;//プレイアブルデータマネ―ジャーからプレイアブルOBJを格納する。
        playableStatusManager = playableDM.playableChara_SM;//プレイアブルデータマネ―ジャーからプレイアブルSMを格納する。

        for (int i = 0; i < playableDM.joinPlayerInt; i++)
        {
            HandCardManager hcm = playableDM.playableChara_HCM[i];//HandCardManagerを格納。
            //hcm.SetUp();
            hcm.switchRoot = "SetUp";

            //アクションマップの変更
            PlayerInput pI = playableDM.playableChara_OBJ[i].GetComponent<PlayerInput>();
            pI.currentActionMap = pI.actions.actionMaps[0];//取得したPlayerInputのアクションマップを1番目(Player)に変更。

            StatusManager sm = playableDM.playableChara_SM[i];
            sm.maxHitPoint = mode_conflict_offsetStatusDate.maxHitPoint;//最大体力の設定。
            sm.hitPoint = mode_conflict_offsetStatusDate.maxHitPoint;//現在体力値を最大体力の数値で代入させる。
            sm.maxManaPoint = mode_conflict_offsetStatusDate.maxManaPoint;//最大マナ数の設定。
            sm.manaPoint = 0;
            sm.manaWantTime = mode_conflict_offsetStatusDate.manaChargeWantTime;//マナチャージに必要な時間の設定。
            sm.manaProgressTime = 0;
            sm.maxSpeed = mode_conflict_offsetStatusDate.maxSpeed;//最大移動速度の設定。
            sm.moveSpeed = mode_conflict_offsetStatusDate.moveSpeed;//移動時の速度設定。

            beingPlayerObj[i] = playableObject[i];//プレイアブルオブジェクトから生存中のプレイヤーに格納する。
        }
    }

    void Conflict_Classic_inBattleProcess()//自分以外全員敵のフリーフォーオール。最後まで生存している者が勝利。
    {
        if (beingPlayerObj[0] != null)
        {
            if (playableStatusManager[0].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[0].enabled = true;//撃破Coverを表示にする。
                eliminatePlayerObj.Add(beingPlayerObj[0]);
                beingPlayerObj[0] = null;

                PlayableInvalidation(0);//対象のプレイアブルを無効化。
                FinishFlagConfirmation();
            }
        }

        if (beingPlayerObj[1] != null)
        {
            if (playableStatusManager[1].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[1].enabled = true;//撃破Coverを表示にする。
                eliminatePlayerObj.Add(beingPlayerObj[1]);
                beingPlayerObj[1] = null;

                PlayableInvalidation(1);//対象のプレイアブルを無効化。
                FinishFlagConfirmation();

            }
        }

        if (beingPlayerObj[2] != null)
        {
            if (playableStatusManager[2].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[2].enabled = true;//撃破Coverを表示にする。
                eliminatePlayerObj.Add(beingPlayerObj[2]);
                beingPlayerObj[2] = null;

                PlayableInvalidation(2);//対象のプレイアブルを無効化。
                FinishFlagConfirmation();
            }
        }

        if (beingPlayerObj[3] != null)
        {
            if (playableStatusManager[3].this_Eliminated == true)
            {
                playableDM.playableCharaUI_DestroyCover[3].enabled = true;//撃破Coverを表示にする。
                eliminatePlayerObj.Add(beingPlayerObj[3]);
                beingPlayerObj[3] = null;

                PlayableInvalidation(3);//対象のプレイアブルを無効化。
                FinishFlagConfirmation();
            }
        }
    }

    void PlayableInvalidation(int player)//指定した番号のプレイアブルの無効化。
    {
        //ここで戦闘時に使用した機能を完全に初期化させる必要がある。
        //StartusManagerのSM_ProcessSafetyをオンにする。
        GameObject paOb = playableDM.playableChara_OBJ[player];

        //プレイアブルの表示の無効化。
        paOb.GetComponent<Collider>().enabled = false;//コライダーの無効化。
        paOb.transform.GetChild(0).gameObject.SetActive(false);//モデル等見た目群を無力化。
        paOb.transform.GetChild(1).gameObject.SetActive(false);//カーソル群を無力化。

        Instantiate(eliminateEffect, paOb.transform.position, transform.rotation);//撃破された際に描写されるエフェクトを生成する。

        playableDM.playableChara_SM[player].SM_ProcessSafety = true;
        playableDM.playableChara_SM[player].this_Eliminated = false;//撃破状態扱いかをオフにする。
                                                               //PlayerMovingのスクリプトenableをオフにする。この時にカメラも渡す。
        PlayerMoving pm = playableDM.playableChara_PM[player];
        pm.enabled = false;
        GameObject mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");//ここはあらかじめエラーを防ぐためにカメラを取らせている。
        pm.visCamera = mainCamObj.GetComponent<Camera>();
        playableDM.playableChara_PM[player].enabled = false;

        HandCardManager hcm = playableDM.playableChara_HCM[player];

        //HandCardManagerのHCM_ProcessSafetyをオンにする。
        hcm.HCM_ProcessSafety = true;
        //HandCardManagerのその他もろもろ。
        hcm.lockOnTarget = null;
        hcm.lineRenderer.enabled = false;
        //HandCardManagerのswichRoot処理値をセットアップに変更する。
        hcm.switchRoot = "SetUp";
        hcm.HundWindowRefresh();//表記の更新。
    }

    void FinishFlagConfirmation()//ラウンド完了のフラグを満たしているかの確認。
    {
        int aliveObj = 0;
        for (int ic = 0;ic < beingPlayerObj.Length;ic++)
        {
            if(beingPlayerObj[ic] != null)
            {
                aliveObj++;
                depositRoundWinnerObj = beingPlayerObj[ic];
            }
        }

        if(aliveObj >= 1 && aliveObj == 1)//もし生き残ったプレイヤーが一人になっていた場合。
        {
            slowTimeSW = true;

            StartCoroutine(PalletGroup_AlphaFadeOut());

            //Animation再生。
            battleAnimationUI_Obj.SetActive(true);
            //int BEC_T_ParamHash = Animator.StringToHash("Break_BattleEndCutIn_Trigger");
            battleAnima.SetTrigger("Break_BattleEndCutIn_Trigger");
            Conflict_Classic_RoundFinish(depositRoundWinnerObj);
        }
    }

    

    void Conflict_Classic_RoundFinish(GameObject winnerObj)//ラウンド終了処理。
    {
        string winnerName = null;
        if(winnerObj.GetComponent<PlayingData>() != null)
        {
            PlayingData winerPD = winnerObj.GetComponent<PlayingData>();
            if(winerPD.playerNumber == 0)
            {
                winnerName = "P1";
                UI_winerNumberSprite.sprite = numberSprites[0];
            }
            else if(winerPD.playerNumber == 1)
            {
                winnerName = "P2";
                UI_winerNumberSprite.sprite = numberSprites[1];
            }
            else if (winerPD.playerNumber == 2)
            {
                winnerName = "P3";
                UI_winerNumberSprite.sprite = numberSprites[2];
            }
            else if (winerPD.playerNumber == 3)
            {
                winnerName = "P4";
                UI_winerNumberSprite.sprite = numberSprites[3];
            }
            else
            {
                winnerName = "404";
            }

            Debug.Log("勝者" + winnerName);

            Invoke("Conflict_EndRoundProcess",10f);
        }
    }

    public void Conflict_EndRoundProcess()//ラウンド終了時に発生させる処理。
    {
        CancelInvoke();

        if(shieldBuild_ModeSW == true)
        {
            for (int i = 0; playableDM.joinPlayerInt > i; i++)
            {
                playableDM.PlayeableChara_GSW[i].selectEndSW = false;
            }
        }

        for (int i = 0; i < playableDM.joinPlayerInt; i++)
        {
            //ここで戦闘時に使用した機能を完全に初期化させる必要がある。
            Debug.Log("セーフモード移行" + i);

            GameObject paOb = playableDM.playableChara_OBJ[i];

            //StartusManagerのSM_ProcessSafetyをオンにする。
            playableDM.playableChara_SM[i].SM_ProcessSafety = true;
            playableDM.playableChara_SM[i].this_Eliminated = false;//撃破状態扱いかをオフにする。
            //PlayerMovingのスクリプトエナブルをオフにする。この時にカメラも渡す。
            PlayerMoving pm = playableDM.playableChara_PM[i];
            pm.enabled = false;
            GameObject mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");//ここはあらかじめエラーを防ぐためにカメラを取らせている。
            pm.visCamera = mainCamObj.GetComponent<Camera>();
            playableDM.playableChara_PM[i].enabled = false;
            playableDM.playableCharaUI_DestroyCover[i].enabled = false;//撃破Coverを非表示にする。

            //プレイアブルの表示処理を有効化させる。
            paOb.GetComponent<Collider>().enabled = true;//コライダーの有効化。
            paOb.transform.GetChild(0).gameObject.SetActive(true);//モデル等見た目群を有効化。
            paOb.transform.GetChild(1).gameObject.SetActive(true);//カーソル群を有効化。

            HandCardManager hcm = playableDM.playableChara_HCM[i];
            //HandCardManagerのswichRoot処理値をセットアップに変更する。
            hcm.switchRoot = "SetUp";
            //HandCardManagerのHCM_ProcessSafetyをオンにする。
            hcm.HCM_ProcessSafety = true;
            //HandCardManagerのその他もろもろ。
            hcm.lockOnTarget = null;
            hcm.lineRenderer.enabled = false;
            hcm.HundWindowRefresh();//表記の更新。
        }

        gameModeRoute = "WhatNextGame";
    }
    ////モードコンポーネント処理。

    //追加モジュール_ShieldBuild ------------------------
    void ShieldBuild_SetUp()//
    {
        GSW_Group.SetActive(true);

        GSW_BackCover_Image.enabled = true;//背景カバーをオンにする。

        for (int c = 0; c < playableDM.playableChara_OBJ.Count; c++)
        {
            //actionmapの変更。
            PlayerInput pI = playableDM.playableChara_OBJ[c].GetComponent<PlayerInput>();
            pI.currentActionMap = pI.actions.actionMaps[2];//取得したPlayerInputのアクションマップを3番目(Player)に変更。

            if(playableDM.playeable_PD[c].onHandDeckDate.Count != 0)
            {
                playableDM.playeable_PD[c].onHandDeckDate.Clear();//デッキデータを削除。
            }

            //Pickの申請。
            playableDM.playeableChara_Adopt_Card[c].AddCard_Conflict_Shield(3,20);//(giveValue,rollcount)//3枚中一枚を20回Pickさせる。
            //Debug.Log("カード選択申請");
        }
        gameModeRoute = "ShieldBuild_PickStandby";

    }
    void ShieldBuild_PickStandby()
    {
        //Debug.Log("選択検証中..............。");

        int joinPlayerInt = playableDM.joinPlayerInt;
        int pickPlayerCount = 0;

        for(int i = 0;joinPlayerInt > i;i++)
        {
            if (playableDM.PlayeableChara_GSW[i].selectEndSW == true)
            {
                pickPlayerCount++;
            }
        }
        
        if (joinPlayerInt <= pickPlayerCount)
        {
            //全員が選択し終わった。
            Debug.Log("全員選択終わりました。");
            for (int i = 0; joinPlayerInt > i; i++)
            {
                playableDM.PlayeableChara_GSW[i].FallWindow();//アニメーションの再生を行う。
            }

            GSW_BackCover_Image.enabled = false;//背景カバーをオフにする。

            Conflict_Classic_SetUp();//セットアップの処理を行う。
            BeforeStartingAnima();//Animationを再生させ、終点のイベントにスタートさせる。
            gameModeRoute = null;
        }
        
    }

    //------------------------------

    public void WhatNextGame()//ゲームがFinishした後、次に移行する要素。
    {
        //現在のゲームモードを比較し、そこから新しいラウンド、ステージ、メインメニューへの帰還への処理を行う。

        sceneTM.loadStageNamber = 0;
        sceneTM.StartLoad();
        gameModeRoute = null;
    }

    //演出用
    void SlowTimeEffect_in()//時間がゆっくりになる。　//これは決着がついた際に使用されることを想定している。
    {
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
    void SlowTimeEffect_out()//時間が元通りになる。　//これは決着がついた際に使用されることを想定している。
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    ////プレイヤーパレット群の表示・非表示の演出用の関数たち。
    //
    void PalletGroup_AlphaHide()//プレイヤーパレットのグループ群をアルファ値かを0にする。
    {
        PalletGroup_CanvasGroup.alpha = 0;
    }

    IEnumerator PalletGroup_AlphaFadeIn()//プレイヤーパレットのグループ群を徐々にフェードアウトさせる。
    {
        PalletGroup_CanvasGroup.alpha = 0;
        float downTime = 0;

        while (downTime <= 1)
        {
            yield return new WaitForSecondsRealtime(0.02f);// 0.02秒待ちます。
            
            downTime += 0.1f;
            PalletGroup_CanvasGroup.alpha = downTime;
        }
        yield break;
    }

    IEnumerator PalletGroup_AlphaFadeOut()//プレイヤーパレットのグループ群を徐々にフェードインする。
    {
        PalletGroup_CanvasGroup.alpha = 1;
        float downTime = 1;

        while (downTime >= 0)
        {
            yield return new WaitForSecondsRealtime(0.02f);// 0.02秒待ちます。

            downTime -= 0.1f;
            PalletGroup_CanvasGroup.alpha = downTime;
        }
        yield break;
    }
}
