using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerGSWInput : MonoBehaviour
{
    PlayingData playingData;//
    PlayerAdopt_Card playerAdopt_Card;

    Animator animator_GSW;

    public bool selectEndSW;//選択が完了しているかどうか。InGameManager側の判定に使用する。

    public string rootswitch;//switch文の条件分岐き使用します。

    public string loadType;//読み込むタイプを選択。("SpellDate"であればカード系の読み込みを行う。)

    public List<SpellData> pick_SpellDates;//外部から送られてくる選択できるデータ。SpellData用(ScriptableObjectを使用する)
    int canPickInt;//選択できる要素の数。(数に合わせて選択枠の配置を変更。)

    //ダイナミック選択時カーソル移動に使用します。
    InputAction selectMove;

    public GameObject dynamic_pickCursorObj;//カーソルとなるUIをアタッチ。
    RectTransform dynamic_Cursor_rtf;//カーソルとなるUIをアタッチ。
    Image dynamic_Cursor_img;

    [SerializeField]int nowSelectChoiceInt;//現在どれにカーソルを合わせているかを取得する為のint値。

    public SpellData[] spellCardDates = new SpellData[4];//スペルデータを格納する為に使用する。

    ////UI

    //選択枠 (0_上、1_下、2_左、3_右、になるように。また参照時、これを基準にする。)//アタッチ必須。
    public List<GameObject> selectPickCursorWindowObject;//要素を選択する際のイメージ画像表示させる枠組みオブジェクト。
    public List<Image> selectPickCursorWindowImage;//要素を選択する際のイメージ画像表示させる枠組みのImageコンポーネント。
    public  List<Image> selectPickImageWindow;//要素を選択する際のイメージ画像表示させる場所。
    public bool[] canSelectPickWindowSW = new bool[4];//選択枠が選択できるかどうかのbool。
    public List<GameObject>canSelectPickWindowObj;//現在選択できる枠数。
    public List<Image> canSelectPickImage;//現在選択できる枠数のイメージ画像表示部分。
    public List<SpellData> canSelectPickDate_SpellDate;//現在選択できるデータ。SpellDate用。

    public GameObject object_GSW;//GeneralSelectWindowそのもの。
    public GameObject object_GSWStandBy;//GSWのスタンバイ状態に表示するオブジェクト。

    public Color32 pick_NoneColorString;

    //back_
    public Image backGroundNumber;//選択状態に表示する番号イメージ。
    public Image standbyBackNumber;//他選択待機状態に表示する番号イメージ。
    public List<Sprite> backNumber_Sprite;//プレイヤーの番号スプライトをアタッチする必要がある。

    //generalSelectTipsPanel
    public GameObject generalSelectTipsPanelObj;//汎用選択Tipsパネルオブジェクト。

    //card_Manual
    public GameObject card_ManualPanelObj;//カード説明パネルオブジェクト。

    public Image card_imageSprite;//データのイメージ画像を表示する。
    public Text card_dateTitleText;//データの名前を表記するテキスト。
    public Text card_manualTipsText;//データの説明文に当たる文章を表示するテキスト。
    public Text card_commentOutText;//データのどうでもいい文章を表示するテキスト。

    public Text card_spellCostText;//スペル使用に必要なマナコストを表示するテキスト。
    public Text card_spellDamageText;//スペルのダメージ値を表示するテキスト。
    public Text card_spellDamageOftenText;//スペルの攻撃数を表示するテキスト。

    //効果音
    AudioSource audioSource;
    public AudioClip selectSE;
    public AudioClip enterSE;

    void Start()
    {
        playingData = GetComponent<PlayingData>();
        playerAdopt_Card = GetComponent<PlayerAdopt_Card>();
        animator_GSW = object_GSW.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        dynamic_Cursor_rtf = dynamic_pickCursorObj.GetComponent<RectTransform>();
        dynamic_Cursor_img = dynamic_pickCursorObj.GetComponent<Image>();

        PlayerInput playerInput = GetComponent<PlayerInput>();//本体以外からの入力で動く為一時コメントアウト。
        selectMove = playerInput.actions["SelectMove"];//指定する入力

        backGroundNumber.sprite = backNumber_Sprite[playingData.playerNumber];//選択状態ジに表示する番号を変更。
        standbyBackNumber.sprite = backNumber_Sprite[playingData.playerNumber];//他選択待機状態時に表示する番号を変更。
    }

    
    void Update()
    {
        switch(rootswitch)
        {
            case "SetUpLoad"://セットアップ。
                SetUpLoad();
                break;
            case "ChoiceSelect"://選択中。
                InputCursor_Dynamic();
                break;
            case "PickSelect"://選択完了。
                if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
                {
                    playerAdopt_Card.Select_PickCallBack();
                }
                break;

        }
    }

    public void SetUpLoad()//データの読み込みから選択待機状態への移行まで行う。
    {
        if(selectEndSW == true)
        {
            selectEndSW = false;
        }

        object_GSW.SetActive(true);

        animator_GSW.SetTrigger("Blink_SelectWindow_Trigger");//選択枠のちらつきアニメーションを再生。

        PlayerInput pI = GetComponent<PlayerInput>();
        pI.currentActionMap = pI.actions.actionMaps[2];//取得したPlayerInputのアクションマップを3番目(Player)に変更。

        dynamic_Cursor_img.color = playingData.playerVisualColor;//CursorColorを設定。

        //どのような情報が送られてくるかを取得する。String型loadType変数。
        //何枚からの選択ができるかの確認。
        if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
        {
            canPickInt = pick_SpellDates.Count;//
        }

        for (int i = 0; 4 > i; i++)//一度選択可能枠をなくす。
        {
            canSelectPickWindowSW[i] = false;//すべての入力可能部分を無力化。
            spellCardDates[i] = null;//spellCardDates内のデータをすべて消す。
            selectPickCursorWindowObject[i].SetActive(false);//すべて非表示。
        }
        if (canSelectPickDate_SpellDate != null) canSelectPickDate_SpellDate.Clear();//削除
            //枚数ごとによる選択枠配置の変更。
        if (canPickInt == 1)//もし選択可能数が1の場合。
        {
            selectPickCursorWindowObject[0].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[0]);//上部を追加。
            canSelectPickImage.Add(selectPickImageWindow[0]);
            canSelectPickWindowSW[0] = true;
            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[0] = pick_SpellDates[0];
            }
        }
        else if(canPickInt == 2)//もし選択可能数が2の場合。
        {
            selectPickCursorWindowObject[2].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[2]);//左部を追加。
            canSelectPickImage.Add(selectPickImageWindow[2]);
            canSelectPickWindowSW[2] = true;
            selectPickCursorWindowObject[3].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[3]);//右部を追加。
            canSelectPickImage.Add(selectPickImageWindow[3]);
            canSelectPickWindowSW[3] = true;

            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[2] = pick_SpellDates[0];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[1]);
                spellCardDates[3] = pick_SpellDates[1];
            }
        }
        else if(canPickInt == 3)//もし選択可能数3がの場合。
        {
            selectPickCursorWindowObject[2].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[2]);//左部を追加。
            canSelectPickImage.Add(selectPickImageWindow[2]);
            canSelectPickWindowSW[2] = true;
            selectPickCursorWindowObject[0].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[0]);//上部部を追加。
            canSelectPickImage.Add(selectPickImageWindow[0]);
            canSelectPickWindowSW[0] = true;
            selectPickCursorWindowObject[3].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[3]);//右部を追加。
            canSelectPickImage.Add(selectPickImageWindow[3]);
            canSelectPickWindowSW[3] = true;

            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[2] = pick_SpellDates[0];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[1]);
                spellCardDates[0] = pick_SpellDates[1];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[2]);
                spellCardDates[3] = pick_SpellDates[2];
            }
        }
        else if(canPickInt == 4)//もし選択可能数が4の場合。
        {
            selectPickCursorWindowObject[0].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[0]);//上部を追加。
            canSelectPickImage.Add(selectPickImageWindow[0]);
            canSelectPickWindowSW[0] = true;
            selectPickCursorWindowObject[1].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[1]);//下部を追加。
            canSelectPickImage.Add(selectPickImageWindow[1]);
            canSelectPickWindowSW[1] = true;
            selectPickCursorWindowObject[2].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[2]);//左部を追加。
            canSelectPickImage.Add(selectPickImageWindow[2]);
            canSelectPickWindowSW[2] = true;
            selectPickCursorWindowObject[3].SetActive(true);//表示。
            canSelectPickWindowObj.Add(selectPickCursorWindowObject[3]);//右部を追加。
            canSelectPickImage.Add(selectPickImageWindow[3]);
            canSelectPickWindowSW[3] = true;

            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                canSelectPickDate_SpellDate.Add(pick_SpellDates[0]);
                spellCardDates[0] = pick_SpellDates[0];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[1]);
                spellCardDates[1] = pick_SpellDates[1];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[2]);
                spellCardDates[2] = pick_SpellDates[2];
                canSelectPickDate_SpellDate.Add(pick_SpellDates[3]);
                spellCardDates[3] = pick_SpellDates[3];
            }
        }

        for (int i = 0; canPickInt > i; i++)//画像情報を更新。
        {
            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
                canSelectPickImage[i].sprite = canSelectPickDate_SpellDate[i].imageSprite;//イメージ画像を更新する。
        }

        rootswitch = "ChoiceSelect";
    }

    ////現在必要性を確認中。SetUpで同時処理させる予定。
    /*
    void Refresh()//各種格納変数を初期化等を行う。
    {
        for (int i = 0; canPickInt > i; i++)//画像情報を更新。
        {
            if(loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
                canSelectPickImage[i].sprite = canSelectPickDate_SpellDate[i].imageSprite;//イメージ画像を更新する。
        }
    }
    */
    

    public void InputEnter(InputAction.CallbackContext context)//決定ボタン入力。
    {
        if(rootswitch == "ChoiceSelect")//現在の選択されている処理が"ChoiceSelect"であれば、
        {
            PickSelect();
        }
    }

    void InputCursor_Dynamic()//ダイナミック選択時カーソル移動。(一定の軸まで移動させると各方向にスナップされます。)
    {
        Vector2 moveVC2 = selectMove.ReadValue<Vector2>();
        Vector3 moveInput = new Vector3(moveVC2.x, moveVC2.y, 0);

        if (moveInput.y >= 0.75f && canSelectPickWindowSW[0] == true)//1
        {
            selectPickCursorWindowImage[0].color = playingData.playerVisualColor;
            selectPickCursorWindowImage[1].color = pick_NoneColorString;
            selectPickCursorWindowImage[2].color = pick_NoneColorString;
            selectPickCursorWindowImage[3].color = pick_NoneColorString;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(0, 35f, 0);

            if(nowSelectChoiceInt != 1)
            {
                audioSource.PlayOneShot(selectSE);//効果音を鳴らす。
                nowSelectChoiceInt = 1;
            }
            
            PickInfoDateRefresh(0);
            // Debug.Log("上");
        }
        else if (moveInput.y <= -0.75f && canSelectPickWindowSW[1] == true)//2
        {
            selectPickCursorWindowImage[0].color = pick_NoneColorString;
            selectPickCursorWindowImage[1].color = playingData.playerVisualColor;
            selectPickCursorWindowImage[2].color = pick_NoneColorString;
            selectPickCursorWindowImage[3].color = pick_NoneColorString;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(0, -35f, 0);

            if (nowSelectChoiceInt != 2)
            {
                audioSource.PlayOneShot(selectSE);//効果音を鳴らす。
                nowSelectChoiceInt = 2;
            }

            PickInfoDateRefresh(1);
            // Debug.Log("下");
        }

        else if (moveInput.x <= -0.75f && canSelectPickWindowSW[2] == true)//3
        {
            selectPickCursorWindowImage[0].color = pick_NoneColorString;
            selectPickCursorWindowImage[1].color = pick_NoneColorString;
            selectPickCursorWindowImage[2].color = playingData.playerVisualColor;
            selectPickCursorWindowImage[3].color = pick_NoneColorString;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(-35f, 0, 0);

            if (nowSelectChoiceInt != 3)
            {
                audioSource.PlayOneShot(selectSE);//効果音を鳴らす。
                nowSelectChoiceInt = 3;
            }

            PickInfoDateRefresh(2);
            // Debug.Log("左");
        }
        else if (moveInput.x >= 0.75f && canSelectPickWindowSW[3] == true)//4
        {
            selectPickCursorWindowImage[0].color = pick_NoneColorString;
            selectPickCursorWindowImage[1].color = pick_NoneColorString;
            selectPickCursorWindowImage[2].color = pick_NoneColorString;
            selectPickCursorWindowImage[3].color = playingData.playerVisualColor;

            dynamic_Cursor_rtf.anchoredPosition = new Vector3(35, 0, 0);

            if (nowSelectChoiceInt != 4)
            {
                audioSource.PlayOneShot(selectSE);//効果音を鳴らす。
                nowSelectChoiceInt = 4;
            }

            PickInfoDateRefresh(3);
            // Debug.Log("右");
        }
        else
        {
            dynamic_Cursor_rtf.anchoredPosition = new Vector3(0, 0, 0) + new Vector3(35 * moveInput.x, 35 * moveInput.y, 0);//スナップされない状態での操作。
            if(nowSelectChoiceInt != 0)
            {
                selectPickCursorWindowImage[0].color = pick_NoneColorString;
                selectPickCursorWindowImage[1].color = pick_NoneColorString;
                selectPickCursorWindowImage[2].color = pick_NoneColorString;
                selectPickCursorWindowImage[3].color = pick_NoneColorString;

                nowSelectChoiceInt = 0;
            }
            if (moveInput == new Vector3(0, 0, 0))//もし、何も入力されていない場合。
            {
                //ここですべての説明パネルを無効化する。
                card_ManualPanelObj.SetActive(false);//カード説明パネルを起動。
            }
        }
    }

    void PickInfoDateRefresh(int i)//選択中の要素の情報(Tips)を読み込む。
    {
        if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
        {
            if (spellCardDates[i] != null)
            {
                card_ManualPanelObj.SetActive(true);//カード説明パネルを起動。

                card_imageSprite.sprite = spellCardDates[i].imageSprite;//イメージ画像を適応。
                card_dateTitleText.text = spellCardDates[i].spellName;//スペル名を入れる。
                card_manualTipsText.text = spellCardDates[i].manualText;//説明文を入れる。
                card_commentOutText.text = spellCardDates[i].commentText;//コメントアウトを入れる。

                card_spellCostText.text = spellCardDates[i].manaCostString;//string型マナコスト値を入れる。
                card_spellDamageText.text = spellCardDates[i].damageString;//string型ダメージ値を入れる。
                card_spellDamageOftenText.text = spellCardDates[i].attackOftenString;//string型攻撃回数を入れる。
            }
            else
            {
                //Debug.Log("何も格納されていない")
            }
        }

        

    }

    void PickSelect()//選んだ数字を格納する
    {
        if(nowSelectChoiceInt == 1)//１(上)を選択
        {
            Debug.Log("上を選択。");
            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                //ここにデッキに加える、
                playingData.onHandDeckDate.Add(spellCardDates[0]);
            }

            audioSource.PlayOneShot(enterSE);//効果音を鳴らす。

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 2)//２(下)を選択
        {
            Debug.Log("下を選択。");
            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                //ここにデッキに加える、
                playingData.onHandDeckDate.Add(spellCardDates[1]);
            }

            audioSource.PlayOneShot(enterSE);//効果音を鳴らす。

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 3)//３(左)を選択
        {
            Debug.Log("左を選択。");
            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                //ここにデッキに加える、
                playingData.onHandDeckDate.Add(spellCardDates[2]);
            }

            audioSource.PlayOneShot(enterSE);//効果音を鳴らす。

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 4)//４(右)を選択
        {
            Debug.Log("右を選択。");
            if (loadType == "SpellDate")//もし呼び出しタイプが"SpellDate"であれば、
            {
                //ここにデッキに加える、
                playingData.onHandDeckDate.Add(spellCardDates[3]);
            }

            audioSource.PlayOneShot(enterSE);//効果音を鳴らす。

            rootswitch = "PickSelect";
        }
        else if(nowSelectChoiceInt == 0)//未選択。
        {
            Debug.Log("未選択");
        }
    }

    public void SelectEnd()//選択終了。
    {
        animator_GSW.SetTrigger("Fade_StandbyWindow_Trigger");
        selectEndSW = true;//選択を終了しているとする。(true化)
    }

    public void FallWindow()
    {
        animator_GSW.SetTrigger("FallWindow_Trigger");//アニメーション再生。
    }
    public void HUD_Hidden()//HUDを非表示にする。
    {
        object_GSW.SetActive(false);
    }
}
