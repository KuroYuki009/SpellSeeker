using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class HandCardManager : MonoBehaviour
{
    [Header("デッキデータ")]//---
    #region
    [Tooltip("編集済みのデッキデータを指す。")]
    public DeckData deckInst;// 編集済みのデッキデータ。

    [Tooltip("現在のデッキデータ。")]
    public List<SpellData> deckCards;//現在のデッキデータ。
    [HideInInspector]public List<SpellData> depositDeck;//シャフルする際に一度デッキデータをキャッシュする為の格納形。

    [Tooltip("現在の手札にあるカード")]
    public List<SpellData> handCards;//手札
    #endregion


    [Header("始点")]//---
    #region
    [Tooltip("カードを使用した時に 生成を行う為の始点となる座標を オブジェクトで指定する。")]
    public GameObject instPos;//プレハブの生成地点として使用する座標オブジェクト。
    #endregion


    [Header("ハンドケース アニメ関係")]//---
    #region
    [Tooltip("ハンドケースの アニメ制御に使用される アニメーターです。")]
    public Animator handAnimator;//ハンドケースのアニメーション。(UIのPallet内に含まれるゲームオブジェクトActionAnima)

    [Tooltip("リロード・ドロー時に 表示させる 残りクールタイムのテキスト。")]
    public Text cooltime_Text;//クールタイムのテキスト。

    [Tooltip("リロード・ドロー時に 表示させる 視覚的な残り時間の演出イメージ画像。")]
    public Image cooltime_FillImage;//クールタイムの塗りつぶし。

    [Space]

    [Tooltip("キャストパネルのアニメ制御に使用される アニメーターです。")]
    public Animator castPanelAnimator;//ハンドケースのキャストパネルのアニメ制御アニメーター。(UIのPallet内に含まれるゲームオブジェクトCastPanelAnima)

    [Tooltip("キャストパネルに入っているカードの 名前 を表示するテキストです。")]
    public Text castCardNameText;//キャストパネルに入っているカードの名前の表示する。

    [Tooltip("キャストパネルに入っているカードの ダメージ値 を表示するテキストです。")]
    public Text castCardDamageText;//キャストパネルに入っているカードのダメージ値の表示。

    [Space]

    [Tooltip("各マグパネルの GUIImage制御に使用される スクリプト。")]
    public List<PickCard> pickWindow;// ハンドケースの描写用GUIImage。

    [Tooltip("カードマグが空の際に 表示させる Noneカードデータ")]
    public SpellData noneCardData;// 非表示用のカードデータ。

    [Tooltip("キャストパネル内のカードが利用できない際に表示させる被せカバー画像。")]
    public Image CastCardCover;//カードが使用できない場合に表示するカバー画像。

    [Tooltip("キャストパネル下部マナ表示の アニメ制御に使用される アニメーターです。")]
    public Animator manaCostWindow_0_Animator;//キャストパネル下部のマナ表示ウィンドウのAnimatorコンポーネント。

    [Tooltip("各マグパネルに 存在してる各カードのコストを表示させる為の テキスト群です。")]
    public List<Text> manaCostText;//ハンドケースの各カードのコストを描写する。

    [Tooltip("デッキの 残りカード枚数を 表示するテキストです。")]
    public Text deckCountText;//デッキに含まれるカード残数
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("ハンドケースの リロード・ドロー時に 最初の入りで 再生されるサウンドです。")]
    public AudioClip ac_HandCase_Reload_Start;

    [Tooltip("ハンドケースの リロード・ドロー時に 待機している間 再生されるループサウンドです。")]
    public AudioClip ac_HandCase_Reload_Charge;

    [Tooltip("ハンドケースの リロード・ドロー時に 最後の閉めで 再生されるサウンドです。")]
    public AudioClip ac_HandCase_Reload_End;


    [Tooltip("カードの破棄時に 再生されるサウンドです。")]
    public AudioClip cardFoldSE;


    [Tooltip("ロックオンの 捕捉時に 再生されるサウンドです。")]
    public AudioClip lockOn_InSE;

    [Tooltip("ロックオンの 解除時に 再生されるサウンドです。")]
    public AudioClip lockOn_OutSE;
    #endregion

    ////--------------------------------------------------

    StatusManager statusManager;//スクリプト「StatusManager」格納用。
    PlayingData playingData;//スクリプト「playingData」格納用
    PlayerMoving playerMoving;//スクリプト「playerMoving」格納用

    [HideInInspector] public LineRenderer lineRenderer;
    AudioSource audioSource;


    [HideInInspector] public string switchRoot;

    //このスクリプトのセルフティ(falseで解除状態)
    [HideInInspector] public bool HCM_ProcessSafety;
    
    bool standbySW;//準備完了か否か。

    [HideInInspector] public int deckCount;//現在のデッキカード枚数を格納する変数。
    int handCard_max = 5;//手札上限。現状この数値を変更する事はできません。


    //クールタイム関係
    #region
    [HideInInspector] public float drawCT;//現在のクールタイム値。
    [HideInInspector] public float drawCT_max;//ドローに発生するクールタイムの長さ。

    [HideInInspector] public float deckReloadCT;//現在のクールタイム値。
    [HideInInspector] public float deckReloadCT_max;//デッキリロードに発生するクールタイムの長さ。basisTimeとsumTimeを合わせた合計値をこれに格納して使用する算段。
    [HideInInspector] public float deckReloadCT_basisTime;//デッキリロードに発生するクールタイムの基礎数値。
    [HideInInspector] public float deckReloadCT_sumTime;//使用したカードに合わせ増減する時間値をあらかじめ格納する。
    #endregion


    //ロックオンシステム系
    #region
    [HideInInspector] public GameObject lockOnTarget;//ロックオンしたオブジェクトを格納。
    StatusManager TargetSM;//ロックオンしたオブジェクトのStatusManager。

    bool canlockOnSW;//ロックオンできるかを二極値で表す。
    float lockOnDistance;//ロックオン可能距離。
    float cantLockOnTime;//ロックオン時クールタイム。
    float lockOnOutTime;//ロックオン維持時間。ロックオン中に壁越しになった際の解除までの時間。

    float lineSize;//現在のロックオンラインの横幅。
    float lockOnLineInstPoint;//ロックオン時に描写する線の表示する高さ。
    #endregion


    ////--------------------------------------------------

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        playingData = GetComponent<PlayingData>();
        playerMoving = GetComponent<PlayerMoving>();
        audioSource = GetComponent<AudioSource>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();//コンポ―ネントを取得。

        switchRoot = "SetUp";
    }

    void Update()
    {
        if(HCM_ProcessSafety == false)
        {
            switch (switchRoot)
            {

                case "SetUp"://処理を行う前に発生させる処理。
                    SetUp();
                    break;


                case "HandCard_Cast":// カード使用
                    HandCard_Cast_Process();
                    break;

                case "HandCard_Trash":// カード破棄
                    HandCard_Trash_Process();
                    break;

            

                case "SetUpDeckCharge":// プレイヤーがビルドしたデッキデータをデッキに入れる処理。
                    Debug.Log("デッキチャージ");
                    break;

                case "DeckShuffle":// デッキのシャッフルを行う処理
                    DeckShuffle();// デッキシャッフルを開始する。
                    break;

                case "MixingDeckCharge":// シャッフルしたデッキを本デッキに補充を行う処理
                    deckCards = new List<SpellData>(depositDeck);
                    depositDeck.Clear();
                    switchRoot = "CardDraw";
                    break;

                case "CardDraw":// 本デッキからカードを取り出し、手札に入れる。
                    HandDraw();
                    break;

                case "HundFalldToLoad"://手札をすべて破棄し、クールタイムを発生させた後にCardDrowに移行する。
                    HundFalldToLoad();
                    break;


                case "DrawCoolTime"://ドロー時に発生する時間経過処理。
                    DrowingTime();
                    break;

                case "DeckReloadingTime"://デッキリロード時に発生する時間経過処理。
                    DeckReloadingTime();
                    break;

                default:
                    Standby();
                    break;
            }

            LockOnProcessing();//ロックオンの処理を行う。
        }
    }

    public void SetUp()// Startで処理する内容。
    {
        Debug.Log("セットアップしてます");
        drawCT_max = 1.2f;　　　　　　// ドローに必要な時間を1.2秒に設定。
        deckReloadCT_basisTime = 3.0f;// リロードに必要な時間を3.0秒に設定。

        lockOnDistance = 30.0f;// ロックオンできる距離を設定。

        //初期化。
        deckReloadCT_sumTime = 0;
        cooltime_Text.enabled = false;
        cooltime_FillImage.enabled = false;
        lockOnLineInstPoint = (gameObject.transform.localScale.y * -0.9f) + gameObject.transform.position.y;//ロックオン時の描写線の高さを設定。
 
        //デッキデータを入れる。
        if (playingData.onHandDeckDate.Count == 0)
        {
            //
            deckCards = new List<SpellData>(deckInst.spells);
        }
        else
        {
            deckCards = new List<SpellData>(playingData.onHandDeckDate);
        }

        lineRenderer.enabled = false;//コンポーネントの無効化。

        depositDeck.Clear();
        handCards.Clear();

        HundWindowRefresh();//表記の更新。

        switchRoot = "DeckShuffle";
    }
    

    void Standby()// プレイヤーの入力を受け付ける。
    {
        if (standbySW == false) standbySW = true;//スタンバイ状態になる。

        // ピックウィンドウ上のカードが消費コストを満たしている場合カバー画像を非表示にする。
        if (statusManager.manaPoint >= handCards[0].manaCost && CastCardCover.enabled == true) CastCardCover.enabled = false;
        else if (statusManager.manaPoint < handCards[0].manaCost && CastCardCover.enabled == false) CastCardCover.enabled = true;

        if (handCards[0] == noneCardData)//ハンドケース内のカード数が0場合。
        {
            standbySW = false;// スタンバイ状態を無効化する。
            switchRoot = "HundFalldToLoad";// 手札の手前がnoneCardDateだった場合、ドローを行う。
        }
    }


    void DeckShuffle()//デッキシャッフル
    {
        deckCount = deckCards.Count;// 一時デッキから 枚数カウントを取得。

        while (deckCount > 0)// シャッフル用のListが空になるまで処理を行う。
        {
            int rundamInt = Random.Range(0, deckCount);// 疑似乱数で 数字を割り出す。最大値は 元のデッキに含まれるカード枚数。

            depositDeck.Add(deckCards[rundamInt]);//---// 割り出した数字の要素に含まれるカードを シャッフル用のListから インゲーム用のListに渡す。
            deckCards.RemoveAt(rundamInt);//-----------// そして、シャッフル用のリストからカードを除外する。

            deckCount = deckCards.Count;// 枚数カウントを更新。
        }

        if (deckCount <= 0) switchRoot = "MixingDeckCharge";// もしカウント値が0になるまでシャッフル処理が終わったら、次の処理"MixingDeckCharge"に移行する。
    }
    

    public void HundWindowRefresh()// 手札の再読み込み。これにより今持っている手札の情報が更新される。
    {
        for (int i = 0; i < handCards.Count; i++)// 手札の枚数分 処理を走らせる。
        {
            pickWindow[i].cardDate = handCards[i];// カードの イメージ画像を 表示させるウィンドウ側に データを渡し、
            pickWindow[i].CardRefresh();//--------// ウィンドウ側の 表示更新関数を起動する。

            if (handCards[i] == noneCardData)//---// もし、このカードが "何でもない" カードだった場合、
            {
                manaCostText[i].enabled = false;  // イメージ画像下部に表示してあるマナコストの テキストを非表示にする。
            }
            else//--------------------------------// そうで無ければ、カードのマナコスト値を テキストに表示させる。
            {
                manaCostText[i].enabled = true;   
                manaCostText[i].text =　　　　　　
                    string.Format("{00}", handCards[i].manaCost);
            }

            if(i == 0)//--------------------------// 手札の中で 一番頭にあるカードであれば、
            {
                if (handCards[0] == noneCardData) 
                {
                    castCardNameText.text = " ";
                    castCardDamageText.text = " ";
                }
                else//----------------------------// 名前とダメージ値を 参照し、それぞれを テキストに表示させる。
                {
                    castCardNameText.text = handCards[0].spellName;
                    if (handCards[0].damageString != null)
                        castCardDamageText.text = handCards[0].damageString + handCards[0].attackOftenString;
                }
            }
        }
        
        deckCountText.text = string.Format("{00}", deckCards.Count);//デッキ残量表示を更新する。

        switchRoot = default;// この処理を終了させ、待機状態に戻る。
    }


    void HandDraw()// デッキから手札にカードデータを渡す。
    {

        for(int i = 0; i < handCard_max; i++)
        {
            if(deckCards.Count <= 0)
            {
                handCards.Add(noneCardData);
            }
            else
            {
                handCards.Add(deckCards[0]);
                deckCards.RemoveAt(0);
            }
        }
        HundWindowRefresh();
    }


    void HundFalldToLoad()// 手札を破棄する処理。またそこから山札(デッキ)からカードをドローする処理につなげる。
    {

        handCards.Clear();// 手札のカードをすべて破棄。

        audioSource.PlayOneShot(ac_HandCase_Reload_Start);//StartSEを再生する。

        audioSource.loop = true;//-------------------// ループをオン。
        audioSource.clip = ac_HandCase_Reload_Charge;// 音声を設定。
        audioSource.Play();//------------------------// 再生。

        if (deckCards.Count > 0)// デッキ内にカードが有り、ドローが可能な場合。
        {
            for (int i = 0; i < handCard_max; i++)
            {
                pickWindow[i].cardDate = noneCardData;
                pickWindow[i].CardRefresh();
                //Debug.Log("手札描写適応！");
            }

            drawCT = drawCT_max;//ドローに必要なクールタイムを再設定。

            handAnimator.SetBool("DrawBool", true);
            switchRoot = "DrawCoolTime";//ドローの処理を行う。
        }
        else if(deckCards.Count <= 0)//デッキが0になっている場合に取得する。
        {
            //デッキデータを入れる。
            if (playingData.onHandDeckDate.Count == 0)
            {
                deckCards = new List<SpellData>(deckInst.spells);
            }
            else
            {
                deckCards = new List<SpellData>(playingData.onHandDeckDate);
            }

            deckReloadCT_max = deckReloadCT_basisTime + deckReloadCT_sumTime;//所要クールタイムの数値を計算。(基礎値と乗算値を合わせ。所要時間に入れる。)
            deckReloadCT_sumTime = 0;//sumTimeをゼロにする。
            deckReloadCT = deckReloadCT_max;//デッキリロードに必要なクールタイムを再設定。

            handAnimator.SetBool("DeckReloadBool", true);
            switchRoot = "DeckReloadingTime";//デッキリロードの処理を行う。
        }
        else
        {
            switchRoot = default;
        }
    }


    void DrowingTime()//手札がドローされるまでの時間処理。
    {
        if(drawCT >= 0)
        {
            drawCT -= 1 * Time.deltaTime;

            if(cooltime_FillImage.enabled == false) cooltime_FillImage.enabled = true;
            if (cooltime_Text.enabled == false) cooltime_Text.enabled = true;


            cooltime_Text.text = string.Format("{00}", drawCT);//
            cooltime_FillImage.fillAmount = drawCT / drawCT_max;
        }
        else
        {
            Debug.Log("ドロークールタイム終了");
            switchRoot = "CardDraw";

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//ループを無効。
            audioSource.Stop();//オーディオを無効化する。
            audioSource.clip = null;//設定音声を無効化。

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SEを再生する。

            handAnimator.SetBool("DrawBool", false);

            //switchRoot = "CardDraw";
        }
    }


    void DeckReloadingTime()//山札がシャッフルされるまでの時間処理。
    {
        if(deckReloadCT >= 0)
        {

            deckReloadCT -= 1 * Time.deltaTime;

            if (cooltime_FillImage.enabled == false) cooltime_FillImage.enabled = true;
            if (cooltime_Text.enabled == false) cooltime_Text.enabled = true;

            cooltime_Text.text = string.Format("{00}", deckReloadCT);//
            cooltime_FillImage.fillAmount = deckReloadCT / deckReloadCT_max;
        }
        else
        {
            Debug.Log("リロードクールタイム終了");
            switchRoot = "DeckShuffle";//デッキリロードの処理を行う。

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//ループを無効。
            audioSource.Stop();//オーディオを無効化する。
            audioSource.clip = null;//設定音声を無効化。

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SEを再生する。

            Debug.Log("デッキ補充");
            handAnimator.SetBool("DeckReloadBool", false);
            //switchRoot = "DeckShuffle";//デッキリロードの処理を行う。
        }
    }


    ////--------------------------------------------------

    //// カードの使用 ////--------------------------------
    
    public void CardCast(InputAction.CallbackContext context)// カード使用の入力受付
    {
        if (HCM_ProcessSafety == false)
        {
            if (statusManager.shockSt == false)// ステータス ショック状態かどうか
            {
                if (standbySW == true)
                {
                    if (context.phase == InputActionPhase.Performed)// ボタンを押したときに一度だけ実行する。
                    {
                        if (statusManager.manaPoint >= handCards[0].manaCost)
                        {
                            statusManager.Mana_Inflict_Expense(handCards[0].manaCost);//コストを消費する。
                            HandCard_Cast_Process();//処理を開始。

                            // Debug.Log("カード使用");

                            standbySW = false;
                        }
                        else
                        {
                            // Debug.Log(gameObject + ">> スペル使用に必要なマナが足りない。");
                            manaCostWindow_0_Animator.SetTrigger("CostNonEnough_Trigger");
                        }
                    }
                }
            }
        }

    }

    void HandCard_Cast_Process()// カード使用の処理。
    {
        if (handCards.Count >= 0)
        {
            standbySW = false;//standby状態を無効化する。

            castPanelAnimator.SetTrigger("CastTrigger");//アニメーションを再生。

            Vector3 vc3InstPos =
                instPos.transform.position;//InstObjをVector3に変換、格納。

            GameObject instPrefab =
                Instantiate(handCards[0].spellPrefab, vc3InstPos, instPos.transform.rotation);//カードデータを元にプレハブを生成する。

            instPrefab.GetComponent<SpellPrefabManager>().ownerObject = gameObject;//スペルプレハブマネージャーにオブジェクト情報を渡す。

            //使用したカードを減らす処理。
            handCards.RemoveAt(0);
            pickWindow[handCards.Count - 1].CardRefresh();
            handCards.Add(noneCardData);

            HundWindowRefresh();

        }
    }


    ////--------------------------------------------------

    //// カードの破棄 ////--------------------------------

    public void CardTrash(InputAction.CallbackContext context)// カード破棄の入力受付
    {
        if (HCM_ProcessSafety == false)
        {
            if (statusManager.shockSt == false)// ステータス ショック状態かどうか。
            {
                if (standbySW == true)
                {
                    if (context.phase == InputActionPhase.Performed)// ボタンを押したときに一度だけ実行する。
                    {
                        HandCard_Trash_Process();//処理を開始。

                        //Debug.Log("カード破棄");

                        standbySW = false;
                    }
                }
            }
        }
    }

    void HandCard_Trash_Process()// カード破棄の処理
    {
        if (handCards.Count >= 0)
        {
            castPanelAnimator.SetTrigger("TrashTrigger");//アニメーションを再生。
            audioSource.PlayOneShot(cardFoldSE);//音を鳴らす。
            statusManager.Mana_Inflict_Revenue(handCards[0].trashMana);//マナポイントの取得。

            //使用したカードを減らす処理。
            handCards.RemoveAt(0);
            pickWindow[handCards.Count - 1].CardRefresh();
            handCards.Add(noneCardData);

            HundWindowRefresh();

        }
    }


    ////--------------------------------------------------

    //// ロックオンアクション ////------------------------

    public void LockOnSearch(InputAction.CallbackContext context)//ロックオンを行う。
    {
        if (HCM_ProcessSafety == false)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                if (lockOnTarget == null && canlockOnSW == true)
                {

                    Vector3 ori = gameObject.transform.position;
                    Ray ray = new Ray(ori, transform.forward);

                    int layerMask = ~LayerMask.GetMask(new string[] { "Default", "StructureObject", "InstObject", "Search",gameObject.tag});//非対象のLayerを除外する。
                    RaycastHit hit;

                    if (Physics.BoxCast(transform.position, new Vector3(1.5f, 1.5f, 1.5f), transform.forward, out hit, Quaternion.identity, lockOnDistance, layerMask))
                    {
                        if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != gameObject.tag)//スクリプト「StatusManager」が付いていて、自身と違うタグが付いている場合に反応する。
                        {
                            Debug.Log("ロックオン！");

                            audioSource.PlayOneShot(lockOn_InSE);//　効果音を鳴らす。

                            lockOnTarget = hit.transform.gameObject;// 標的の オブジェクト情報を 格納する。

                            playerMoving.lockOnTargetObj = lockOnTarget;// PlayerMovingにTargetを格納。

                            cantLockOnTime = 0.2f;//クールタイムを設定。通常より短期間のクールタイム。

                            canlockOnSW = false;//ロックオンを不可能にする。

                            if (TargetSM == null) TargetSM = lockOnTarget.GetComponent<StatusManager>();//ターゲットを取得。
                            
                        }
                        //string name = hit.collider.gameObject.name;//ヒットしたオブジェクトの名前を格納。
                        //Debug.Log(name);//ヒットしたオブジェクトの名前をログに出す。

                    }
                }
                else if (lockOnTarget != null && canlockOnSW == true)
                {
                    LockOnOut(); //ロックオンが解除された際の処理を走らせる。
                }
            }
        }
    }

    //--------------------------------------------------------------

    void LockOnProcessing()
    {
        if(canlockOnSW == false)//ロックオンできない状態である場合、
        {
            //canTimeの数値を経過低下させる。
            if (cantLockOnTime >= 0)
            {
                cantLockOnTime -= 1 * Time.deltaTime;
            }
            else if (cantLockOnTime <= 0.0f && cantLockOnTime != 0)
            {
                cantLockOnTime = 0.0f;
                canlockOnSW = true;
            }
        }

        //// 敵が壁越しにいるかの確認を行う。
        
        if(lockOnTarget != null)// ターゲットが格納されている場合、
        {
            LockOnLineChain();// ロックオンしているオブジェクトとの間に線を描写させる。

            Vector3 pos = lockOnTarget.transform.position - transform.position;

            Ray ray = new Ray(transform.position, pos);// 敵の座標を目標位置に設定。

            int layerMask = LayerMask.GetMask(new string[] { "Default","StructureObject", "InstObject",lockOnTarget.tag}); // Layer判定を設定する。タグ名とレイヤー名が同じである事が必須。
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, lockOnDistance,layerMask))
            {

                if (hit.collider.gameObject != lockOnTarget)//もしロックオンしているオブジェクトが壁越しになると。
                {
                    //ロックオン解除までの時間経過、処理を行う。

                    if (lockOnOutTime >= 0)
                    {
                        lockOnOutTime -= 1 * Time.deltaTime;
                    }
                    else if (lockOnOutTime <= 0)
                    {
                        LockOnOut(); //ロックオンが解除された際の処理を走らせる。
                    }
                }
                else if (lockOnOutTime <= 0.5f)//もし、ロックオンしているオブジェクトのレイヤーを検出している間は。
                {
                    //ロックオン解除までの時間をリセット。
                    lockOnOutTime += 0.1f;
                }

                if (TargetSM.hitPoint <= 0) LockOnOut();//もしロックオンしている対象のHPが0の場合、ロックオンをオフにする。
            }
        }
    }

    void LockOnOut()//ロックオンが解除された際のの処理。
    {
        Debug.Log("ロックオン解除！");

        audioSource.PlayOneShot(lockOn_OutSE);//効果音を鳴らす。

        lineRenderer.enabled = false;//コンポーネントを無効化させる。
        lineSize = 0f;
        lockOnTarget = null;
        TargetSM = null;
        if (playerMoving == null) playerMoving = GetComponent<PlayerMoving>();
        playerMoving.lockOnTargetObj = lockOnTarget;

        cantLockOnTime = 0.5f;//クールタイムを設定。

        canlockOnSW = false;//ロックオンを不可能にする。
    }

    void LockOnLineChain()//ロックオン中のオブジェクト間の線を表示させる。
    {
        lineRenderer.enabled = true;
        Vector3 p_vc3 = new Vector3(gameObject.transform.position.x, lockOnLineInstPoint, gameObject.transform.position.z);
        Vector3 t_vc3 = new Vector3(lockOnTarget.transform.position.x, lockOnLineInstPoint, lockOnTarget.transform.position.z);

        
        lineRenderer.SetPosition(0, p_vc3);
        lineRenderer.SetPosition(1, t_vc3);

        if (lineSize <= 0.5f)
        {
            lineSize += 5 * Time.deltaTime;
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;
        }
    }

    ////--------------------------------------------------
}
