using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class HandCardManager : MonoBehaviour
{
    //このスクリプトのセルフティ(falseで解除状態)
    public bool HCM_ProcessSafety;

    ////他スクリプト関連。
    PlayingData playingData;//スクリプト「playingData」格納用
    PlayerMoving playerMoving;//スクリプト「playerMoving」格納用
    StatusManager statusManager;//スクリプト「StatusManager」格納用。

    ////スペルデッキ関連。
    public DeckData deckInst;

    //ハンドケースのアクションアニメーション関係。(UIのPallet内に含まれるゲームオブジェクトActionAnimaをアタッチする。)
    public Animator handAnimator;//ハンドケースのアニメーション。
    public Text cooltime_Text;//クールタイムのテキスト。
    public Image cooltime_FillImage;//クールタイムの塗りつぶし。

    //ハンドケースのキャストパネルのアニメーション関係。(UIのPallet内に含まれるゲームオブジェクトCastPanelAnimaをアタッチする。)
    public Animator castPanelAnimator;

    ////インスペクターでのアタッチ必須項目
    public List<SpellData> deckCards;//デッキカード
    [SerializeField] List<SpellData> depositDeck;//シャフルする際に入れていくことが出来る。
    public List<SpellData> handCards;//手札

    public Text castCardNameText;//キャストカーソルに入っているカードの名前の表示する。
    public Text castCardDamageText;//キャストカーソルに入っているカードのダメージ値の表示。

    public List<PickCard> pickWindow;// ハンドケースの描写用GUIImage。
    public SpellData noneCardData;// 非表示用のカードデータ。

    public Image CastCardCover;//カードが使用できない場合に表示するカバー画像。

    public Animator manaCostWindow_0_Animator;//最前列のカードのマナ表示ウィンドウのAnimatorコンポーネント。

    public List<Text> manaCostWindow;//ハンドケースの各カードのコストを描写する。

    public Text deckCountText;//デッキに含まれるカード残数

    //
    public GameObject instPos;//プレハブの生成地点として使用する座標オブジェクト。

    ////分岐用
    public string switchRoot;
    [SerializeField]bool standbySW;//準備完了か否か。

    //
    public int deckCount;//処理回数カウント用
    int handCard_max = 5;//手札上限

    //クールタイム
    public float drawCT;//進行型クールタイム。
    public float drawCT_max;//ドローに発生するクールタイムの長さ。

    public float deckReloadCT;//進行型クールタイム。
    public float deckReloadCT_max;//デッキリロードに発生するクールタイムの長さ。(basisTimeとsumTimeの合計値)
    public float deckReloadCT_basisTime;//デッキリロードに発生するクールタイムの基礎数値。
    public float deckReloadCT_sumTime;//使用したカードに合わせ増減する時間値をあらかじめ格納する。必要な時に呼びだし、数値を空にする。

    //入力関係


    //ロックオンシステム系
    public GameObject lockOnTarget;//ロックオンしたオブジェクトを格納。
    StatusManager TargetSM;//ロックオンしたオブジェクトのStatusManager。
    public LineRenderer lineRenderer;
    float lockOnDistance;//ロックオン可能距離。
    bool canlockOnSW;//ロックオンできるかを二極値で表す。
    float cantLockOnTime;//ロックオン時クールタイム。
    float lockOnOutTime;//ロックオン維持時間。ロックオン中に壁越しになった際の解除までの時間。

    float lineSize;

    float lockOnLineInstPoint;//ロックオン時に描写する線の表示する高さ。

    //サウンド系

    AudioSource audioSource;

    public AudioClip ac_HandCase_Reload_Start;
    public AudioClip ac_HandCase_Reload_Charge;
    public AudioClip ac_HandCase_Reload_End;

    public AudioClip cardFoldSE;

    public AudioClip lockOn_InSE;
    public AudioClip lockOn_OutSE;

    void Start()
    {
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

                case "HandCard_Cast"://使用
                    HandCard_Cast();
                    break;
                case "HandCard_Trash"://破棄
                    HandCard_Trash();
                    break;

                //

                case "SetUpDeckCharge":// プレイヤーがビルドしたデッキデータをデッキに入れる処理。
                    Debug.Log("デッキチャージ");
                    break;
                case "DeckShuffle":// デッキのシャッフルを行う処理
                    deckCount = deckCards.Count;//一時デッキから枚数カウントを取得。
                    DeckShuffle();
                    if (deckCount <= 0) switchRoot = "MixingDeckCharge";
                    break;
                case "MixingDeckCharge":// シャッフルしたデッキを本デッキに補充を行う処理
                    deckCards = new List<SpellData>(depositDeck);
                    depositDeck.Clear();
                    switchRoot = "CardDraw";
                    break;
                case "CardDraw":// 本デッキからカードを取り出し、手札に入れる。
                    HandDraw();
                    break;
                case "HundFalld"://手札をすべて破棄し、クールタイムを発生させた後にCardDrowに移行する。
                    HundFalld();
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

            LockOnProcessing();//ロックオン時の処理を行う。
        }
    }

    public void SetUp()//Startで処理する内容。
    {
        Debug.Log("セットアップしてます");
        drawCT_max = 2;
        deckReloadCT_basisTime = 5;
        //初期化。
        deckReloadCT_sumTime = 0;
        cooltime_Text.enabled = false;
        cooltime_FillImage.enabled = false;
        lockOnLineInstPoint = (gameObject.transform.localScale.y * -0.9f) + gameObject.transform.position.y;//ロックオン時の描写線の高さを設定。
        //

        statusManager = GetComponent<StatusManager>();

        playingData = GetComponent<PlayingData>();

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

        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();//コンポ―ネントを取得。
        lineRenderer.enabled = false;//コンポーネントの無効化。

        audioSource = GetComponent<AudioSource>();

        depositDeck.Clear();
        handCards.Clear();

        HundWindowRefresh();//表記の更新。

        switchRoot = "DeckShuffle";
    }

    void Standby()//プレイヤーの入力を受け付ける。
    {
        if (standbySW == false) standbySW = true;//スタンバイ状態になる。

        //ピックウィンドウ上のカードが消費コストを満たしている場合カバー画像を非表示にする。
        if (statusManager.manaPoint >= handCards[0].manaCost && CastCardCover.enabled == true) CastCardCover.enabled = false;
        else if (statusManager.manaPoint < handCards[0].manaCost && CastCardCover.enabled == false) CastCardCover.enabled = true;

        if (handCards[0] == noneCardData)//ハンドケース内のカード数が0場合。
        {
            standbySW = false;//スタンバイ状態を無効化する。
            switchRoot = "HundFalld";//手札の手前がnoneCardDateだった場合、ドローを行う。
        }

        /*
        //旧式入力。
        if (Input.GetKeyDown(KeyCode.Space)) switchRoot = "HundFalld";
        if(Input.GetMouseButtonDown(0)) switchRoot = "HandCard_Cast"; 
        if (Input.GetMouseButtonDown(1)) switchRoot = "HandCard_Trash";
        */
    }


    void HandCard_Cast()//カード使用
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

    void HandCard_Trash()//カード破棄
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

    void DeckShuffle()//デッキシャッフル
    {
        int rundamInt = Random.Range(0, deckCount);

        if(deckCount > 0)
        {
            depositDeck.Add(deckCards[rundamInt]);
            deckCards.RemoveAt(rundamInt);
        }
    }
    

    public void HundWindowRefresh()//手札の再読み込み。これにより今持っている手札の画像が更新される。
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            pickWindow[i].cardDate = handCards[i];
            pickWindow[i].CardRefresh();
            if(handCards[i] == noneCardData)
            {
                manaCostWindow[i].enabled = false;
            }
            else
            {
                if (manaCostWindow[i].enabled == false) manaCostWindow[i].enabled = true;
                manaCostWindow[i].text = string.Format("{00}", handCards[i].manaCost);
            }

            if(i == 0)//最初の処理限定。
            {
                if (handCards[0] == noneCardData)
                {
                    castCardNameText.text = " ";
                    castCardDamageText.text = " ";
                }
                else
                {
                    castCardNameText.text = handCards[0].spellName;
                    if (handCards[0].damageString != null)
                        castCardDamageText.text = handCards[0].damageString + handCards[0].attackOftenString;
                }
            }
        }
        
        deckCountText.text = string.Format("{00}", deckCards.Count);//デッキ残量表示を更新する。

        switchRoot = default;
    }

    void HandDraw()//デッキから手札にカードデータを渡す。
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

    void HundFalld()//手札を破棄する処理。またそこから山札(デッキ)からカードをドローする処理につなげる。
    {
        if (deckCards.Count > 0)//デッキ内にカードが有り、ドローが可能な場合。
        {
            handCards.Clear();

            audioSource.PlayOneShot(ac_HandCase_Reload_Start);//SEを再生する。

            audioSource.loop = true;//ループをオンにする。
            audioSource.clip = ac_HandCase_Reload_Charge;//bgm設定。
            audioSource.Play();//bgm再生。

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
            handCards.Clear();

            audioSource.PlayOneShot(ac_HandCase_Reload_Start);//SEを再生する。

            audioSource.loop = true;//ループをオンにする。
            audioSource.clip = ac_HandCase_Reload_Charge;//bgm設定。
            audioSource.Play();//bgm再生。

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

            deckReloadCT_max = deckReloadCT_basisTime + deckReloadCT_sumTime;//所要クールタイムの数値を計算。(基礎値と乗算値を合わせ。所要時間に入れる。)
            deckReloadCT_sumTime = 0;//sumTimeをゼロにする。
            deckReloadCT = deckReloadCT_max;//デッキリロードに必要なクールタイムを再設定。
            handAnimator.SetBool("DeckReloadBool", true);
            switchRoot = "DeckReloadingTime";//デッキリロードの処理を行う。
        }
        else
        {
            Debug.Log("NullDeck");
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

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//ループを無効。
            audioSource.Stop();//オーディオを無効化する。
            audioSource.clip = null;//設定音声を無効化。

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SEを再生する。

            handAnimator.SetBool("DrawBool", false);
            switchRoot = "CardDraw";
            
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

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//ループを無効。
            audioSource.Stop();//オーディオを無効化する。
            audioSource.clip = null;//設定音声を無効化。

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SEを再生する。

            Debug.Log("デッキ補充");
            handAnimator.SetBool("DeckReloadBool", false);
            switchRoot = "DeckShuffle";//デッキリロードの処理を行う。
        }
    }

    ////新式入力。---------------------------------------------

    public void CardCast(InputAction.CallbackContext context)//カード使用
    {
        if(HCM_ProcessSafety == false)
        {
            if (context.performed)
            {
                if (statusManager.shockSt == false)
                {
                    if (standbySW == true)
                    {
                        if (context.phase == InputActionPhase.Performed)
                        {
                            if (statusManager.manaPoint >= handCards[0].manaCost)
                            {
                                statusManager.Mana_Inflict_Expense(handCards[0].manaCost);//コストを消費する。
                                HandCard_Cast();//処理を開始。
                                Debug.Log("カード使用");
                                standbySW = false;
                            }
                            else
                            {
                                Debug.Log(gameObject + ">> スペル使用に必要なマナが足りない。");
                                manaCostWindow_0_Animator.SetTrigger("CostNonEnough_Trigger");
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void CardTrash(InputAction.CallbackContext context)//カード破棄
    {
        if (HCM_ProcessSafety == false)
        {
            if (context.performed)
            {
                if (statusManager.shockSt == false)
                {
                    if (standbySW == true)
                    {
                        if (context.phase == InputActionPhase.Performed)
                        {
                            HandCard_Trash();//処理を開始。
                            Debug.Log("カード破棄");
                            standbySW = false;
                        }
                    }
                }
            }
        }
    }

    public void LockOnSearch(InputAction.CallbackContext context)//ロックオンを行う。
    {
        if (HCM_ProcessSafety == false)
        {
            if (context.performed)
            {
                if (lockOnTarget == null && canlockOnSW == true)
                {
                    lockOnDistance = 30.0f;//テストでロックオンできる距離を設定。

                    Vector3 ori = gameObject.transform.position;
                    Ray ray = new Ray(ori, transform.forward);
                    int layerMask = ~LayerMask.GetMask(new string[] { "Default", "StructureObject", "InstObject", "Search",gameObject.tag});//Layer判定から除外する。タグ名とレイヤー名が同じである事が必須。
                    RaycastHit hit;

                    if (Physics.BoxCast(transform.position, new Vector3(1.5f, 1.5f, 1.5f), transform.forward, out hit, Quaternion.identity, lockOnDistance, layerMask))
                    {
                        if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != gameObject.tag)//スクリプト「StatusManager」が付いていて自身と違うタグが付いている場合に反応する。
                        {
                            Debug.Log("ロックオン！");

                            audioSource.PlayOneShot(lockOn_InSE);//効果音を鳴らす。

                            lockOnTarget = hit.transform.gameObject;
                            if (playerMoving == null) playerMoving = GetComponent<PlayerMoving>();
                            playerMoving.lockOnTargetObj = lockOnTarget;
                            cantLockOnTime = 0.2f;//クールタイムを設定。
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

        if(lockOnTarget != null)//ターゲットが格納されている場合は。
        {
            LockOnLineChain();//ロックオンしているオブジェクトの間に線を生成。
            GameObject target = lockOnTarget;
            Vector3 pos = target.transform.position - transform.position;

            Ray ray = new Ray(transform.position, pos);//コリジョンした敵の座標をレイ目標位置に設定。

            int layerMask = LayerMask.GetMask(new string[] { "Default","StructureObject", "InstObject",target.tag}); //Layer判定を設定する。タグ名とレイヤー名が同じである事が必須。
            RaycastHit hit;

            //Debug.DrawRay(ray.origin, ray.direction * lockOnDistance, Color.red, 0.2f);//テスト機能。飛ばしたRayの軌道を見る。

            if (Physics.Raycast(ray, out hit, lockOnDistance,layerMask))
            {

                if (hit.collider.gameObject != target)//もしロックオンしているオブジェクトが壁越しになると。
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
            }

            if (TargetSM.hitPoint <= 0) LockOnOut();//もしロックオンしている対象のHPが0の場合、ロックオンをオフにする。
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
        cantLockOnTime = 1f;//クールタイムを設定。
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
    
}
