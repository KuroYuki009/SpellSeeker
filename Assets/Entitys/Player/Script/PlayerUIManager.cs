using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUIManager : MonoBehaviour
{
    ////基本。//----
    public GameObject ownerPlayerOBJ;//所有者となるプレイヤーオブジェクト。
    public StatusManager ownerPlayerSM;//所有者のステータスマネージャー。
    public PlayerMoving ownerPlayerMV;//所有者のプレイヤームービング。

    public GameObject manaUI_OBJ;
    Animator manaAnima;
    int depositManapoint;//一時的に数値を保管し、判定に使用。

    ////プレイアブル下部に描写するUIに関する。//------
    /*public GameObject playerUnderUI;//プレイヤーオブジェクト下部に表示するUI。
    Camera visCamera;//カメラの指定。
    Transform p_tf;//プレイヤーの座標。
    RectTransform uu_tf;//下部UIのRect座標。
    Vector3 offsetPos = new Vector3(0,-1.5f,0);*/

    ////プレイヤーのステータスUIに関する。//-----
    public CanvasGroup playerUIGroup;//ステータスUI等をまとてある親に付いているコンポーネント。
    float guiAlpha;

    ////ステータスUI各種に関する。//------
    float healthDeposit;//体力を一時格納する。UI等の更新の判別に使用する。
    public Image healthFillSprite;
    public Text healthText;

    public Image manaFillSprite;
    public Text manaText;

    //脆弱状態
    public Image healthCrackCoverImage;
    //float vulnerableDeposit;
    

    //セカンドアクション「シフトダッシュ」はPlayerMovingスクリプトに存在します。
    public Image shiftDashFillSprite;
    public Text shiftDashCooltimeText;
    ////処理に関する。
    public string windowSwitch;//switch文の分岐に使用。

    //アニメーション類

    public bool healthDangerSW;
    public GameObject healthDangerObj;//
    public Animator healthDangerAnimator;//体力値警告のアニメーション。

    public Animator healthDamageImpactAnimator;//被ダメージ時の体力値アニメーション。

    void Start()
    {
        if (ownerPlayerSM == null) ownerPlayerSM = ownerPlayerOBJ.GetComponent<StatusManager>();
        if (ownerPlayerMV == null) ownerPlayerMV = ownerPlayerOBJ.GetComponent<PlayerMoving>();
        guiAlpha = 1.0f;

        manaAnima = manaUI_OBJ.GetComponent<Animator>();

        /*if (visCamera == null) visCamera = Camera.main;//メインカメラを入れる。
        if(p_tf == null) p_tf = ownerPlayerOBJ.GetComponent<Transform>();//プレイヤーの座標を取得。
        if (uu_tf == null) uu_tf = playerUnderUI.GetComponent<RectTransform>(); //下部UIの座標を取得。*/
    }

    
    void Update()
    {
        switch (windowSwitch)//ウィンドウの透過状態、ハイライトの切り替え。
        {
            case "HideUIGroup":
                HideUIGroup();
                break;
            case "AppearUIGroup":
                AppearUIGroup();
                break;
        }
        
        if(healthDeposit != ownerPlayerSM.hitPoint)//もし一時格納体力変数とプレイヤーの現在の体力の数値が違う場合、
        {
            //Debug.Log("体力を更新。");
            HitPointUILoad();//情報更新の処理を行う。
            healthDeposit = ownerPlayerSM.hitPoint;//格納用変数に格納。

        }

        if (ownerPlayerSM.vulnerableSt == true)//もし脆弱状態の二極値がtrueになっていた場合。
        {
            healthCrackCoverImage.enabled = true;
        }
        else if(healthCrackCoverImage.enabled == true)
        {
            healthCrackCoverImage.enabled = false;
        }

        ManaUILoad();//プレイヤーのマナに合わせ、text、Spriteを変更する。
        SecondActionUILoad();//セカンドアクションの数値に合わせtext、spriteを変更する。
    }

    /*
    void chaseUnderUI()//[廃止]下部UIをプレイヤーに追尾させる。
    {
        uu_tf.position = RectTransformUtility.WorldToScreenPoint(visCamera, p_tf.position + offsetPos);
    }
    */


    public void HitPointUILoad()//プレイヤーの体力に合わせ。Text、Spriteを変更する。
    {
        float health = ownerPlayerSM.hitPoint;//intからfloatに変換。
        float maxHealth = ownerPlayerSM.maxHitPoint;//intからfloatに変換。
        healthText.text = string.Format("{00}", health);//体力のテキストを更新する。
        healthFillSprite.fillAmount = health / maxHealth;

        healthDamageImpactAnimator.SetTrigger("DamageImpact_Trigger");//被ダメ時にアニメーションを再生する。
        
        if(healthDangerSW != false && maxHealth / 5 <= health)
        {
            healthDangerObj.SetActive(false);
            healthDangerAnimator.SetBool("HealthWarning_Bool", false);
            healthDangerSW = false;
        }
        else if (maxHealth / 5 >= health)//最大体力の5分の１より既存体力が少ない場合。
        {
            healthDangerObj.SetActive(true);
            healthDangerAnimator.SetBool("HealthWarning_Bool", true);
            healthDangerSW = true;
        }
    }

    public void ManaUILoad()//プレイヤーのマナに合わせ、text、Spriteを変更する。
    {
        float mana = ownerPlayerSM.manaPoint;
        float wantTime = ownerPlayerSM.manaWantTime;//必要時間を取得。
        float progressTime = ownerPlayerSM.manaProgressTime;//経過時間を取得。
        if (mana == ownerPlayerSM.maxManaPoint && manaFillSprite.fillAmount != 1f)
        {
            manaText.text = string.Format("{00}", mana);//マナのテキストを更新する。
            manaFillSprite.fillAmount = 1f;
        }
        else if(mana != ownerPlayerSM.maxManaPoint)
        {

            manaText.text = string.Format("{00}", mana);//マナのテキストを更新する。
            manaFillSprite.fillAmount = progressTime / wantTime;
        }

        if(ownerPlayerSM.manaPoint != depositManapoint)
        {
            manaAnima.SetTrigger("ManaFlash_Trigger");
            depositManapoint = ownerPlayerSM.manaPoint;
        }
    }

    void SecondActionUILoad()//
    {
        float cooltime = ownerPlayerMV.sdAtCoolTime;//クールタイム。
        float maxCooltime = ownerPlayerMV.sdAtMaxCoolTime;//代入用クールタイム。
        //bool actionSW = ownerPlayerMV.sdActionSW;//Actionの使用可能か。

        if (cooltime <= 0 && shiftDashCooltimeText.enabled == true)
        {
            shiftDashCooltimeText.enabled = false;//テキストを隠す
        }
        else if (cooltime >= 0 && shiftDashCooltimeText.enabled == false)
        {
            shiftDashCooltimeText.enabled = true;//テキストを表示する。
        }

        if (cooltime >= 0)
        {
            shiftDashCooltimeText.text = string.Format("{00}", cooltime);
            shiftDashFillSprite.fillAmount = 1 - (cooltime / maxCooltime);
        }
        else if (shiftDashFillSprite.fillAmount != 1) shiftDashFillSprite.fillAmount = 1;
    }

    void HideUIGroup()
    {
        if(guiAlpha >= 0.5f)
        {
            guiAlpha -= 5 * Time.deltaTime;
            playerUIGroup.alpha = guiAlpha;
        }
        else
        {
            guiAlpha = 0.5f;
            playerUIGroup.alpha = guiAlpha;
            windowSwitch = null;
        }
    }

    void AppearUIGroup()
    {
        if (guiAlpha <= 1f)
        {
            guiAlpha += 5 * Time.deltaTime;
            playerUIGroup.alpha = guiAlpha;
        }
        else
        {
            guiAlpha = 1.0f;
            playerUIGroup.alpha = guiAlpha;
            windowSwitch = null;
        }

    }

}
