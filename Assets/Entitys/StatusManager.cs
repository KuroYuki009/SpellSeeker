using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusManager : MonoBehaviour
{
    //このスクリプトのセルフティ(falseで解除状態)
    public bool SM_ProcessSafety;

    //// 基本ステータス情報
    public int maxHitPoint;//体力の最大値。
    public int hitPoint;//現在の体力。
    public bool hitPoint_Empty;//体力があるかどうか。
    public bool this_Eliminated;//このステータスの持ち主は完全に倒されている状態か？

    public int maxArmorPoint;//アーマーシールドの最大値。
    public int armorPoint;//現在のアーマーシールド。

    public int maxManaPoint;//マナコストの最大値。
    public int manaPoint;//現在のマナコスト。
    public float manaWantTime;//一回のマナチャージに必要な時間。
    public float manaProgressTime;//一回のマナチャージの経過時間。

    public float maxSpeed;//最大移動速度
    public float moveSpeed;//移動速度
    public float moveMag = 1.0f;//移動速度倍率。

    //public int offensivePoint;//攻撃力//[廃止]単純に必要ない為。
    //public int defensePoint;//防御力//[廃止]単純に必要ない為。

    //エクストラ
    public bool thisCommonEnemy;//このステータスの持ち主は雑魚敵に該当するか？(これがオンになった場合、一部例外を除き外部からの無敵時間が無くなります。)

    ////バフ・デバフ・状態系の変数

    public bool invincibleSt;//無敵状態か？
    public float invincibleTime;//無敵状態時間。

    public bool superArmorSt;//スーパーアーマー状態か。(ダメージによる怯みが無くなる)

    public float slowTime;//移動速度低下時間。

    public bool noMoveSt;//移動不能状態か?。
    public float noMoveTime;//移動不可時間。

    public bool shockSt;//ショック状態か？
    public float shockTime;//ショック時間。

    public bool vulnerableSt;//脆弱状態か?。
    float vulnerableMag = 1.5f;//上昇する被ダメージ倍率
    public float vulnerableTime;//脆弱状態時間。

    //[テスト機能]ヒット値計算_変数
    float takeDamege;
    float totalDamage;

    void Start()
    {
        manaWantTime = 1.5f;
        maxManaPoint = 5;
        maxSpeed = 15;//Default = 15
        moveSpeed = 15;//Default = 15
        hitPoint_Empty = false;
    }
    void Update()
    {
        if(SM_ProcessSafety == false)
        {
            ConditionCheck();//状態の検出。(予定要素 // 状態異常を付与されるタイミングで処理を起動するようにした方がいい。)
            ManaCharge();
        }
    }

    // ヒットポイント(HealthPoint) //-----------------------------------------------------------------------------------
    public void HP_Inflict_Damage(int damageInt)//ヒットポイントに与えられるダメージ値。
    {
        if(invincibleSt == false)
        {
            if (vulnerableSt == true)//脆弱状態である場合、ダメージをvulnerableMagの数値で掛ける。
            {
                int calc = 0;
                float floatCalc = 0;
                floatCalc = (float)damageInt * vulnerableMag;//計算した数値を格納。
                armorPoint -= (int)floatCalc;//アーマーシールド値をダメージ値で削る。
                if (armorPoint < 0)//もしアーマーシールド値が0以下であれば
                {
                    calc += armorPoint * -1;//数値を正当値にし、ローカルcalcに入れる。
                    armorPoint = 0;//アーマー値を0にする。
                    hitPoint -= calc;//ヒットポイントにローカルcalcを入れる。
                }

                if (hitPoint <= 0)
                {
                    hitPoint = 0;
                    hitPoint_Empty = true;
                }

                vulnerableTime = 0f;//脆弱状態の時間をなくす。
            }
            else//脆弱状態で無ければ通常の処理を行う。
            {
                int calc = 0;
                armorPoint -= damageInt;//アーマーシールド値をダメージ値で削る。
                if (armorPoint < 0)//もしアーマーシールド値が0以下であれば
                {
                    calc += armorPoint * -1;//数値を正当値にし、ローカルcalcに入れる。
                    armorPoint = 0;//アーマー値を0にする。
                    hitPoint -= calc;//ヒットポイントにローカルcalcを入れる。
                }

                if (hitPoint <= 0)
                {
                    hitPoint = 0;
                    hitPoint_Empty = true;
                }
            }

            
            //[テスト機能]ヒット値計算_処理---------------------------------
            takeDamege = damageInt;
            totalDamage += takeDamege;
            Debug.Log("Inst : " + takeDamege + " | Total : " + totalDamage);
            //---------------------------------------------------------------

            if(hitPoint_Empty == true)//体力が確実に無くなっている場合、
            {
                if(hitPoint >= 0)hitPoint_Empty = false;
                this_Eliminated = true;
            }
            else if(this_Eliminated == true)
            {
                if(hitPoint >= 0)hitPoint_Empty = false;
            }
        }
    }

    public void HP_Inflict_Recovery(int recoveryInt)//ヒットポイントに与えられる回復値。
    {
        if(maxHitPoint >= hitPoint) hitPoint += recoveryInt;
    }

    // マナ(Mana) //-----------------------------------------------------------------------------------

    public void Mana_Inflict_Expense(int costInt_L)//マナポイントの消費値。
    {
        manaPoint -= costInt_L;
    }

    public void Mana_Inflict_Revenue(int revenueInt_L)//マナポイントの取得値。
    {
        manaPoint += revenueInt_L;
    }

    // 状態(StatusEffect) //-----------------------------------------------------------------------------------
    public void St_Inflict_Invincible(float invincibleTime_L)//敵からの攻撃が当たらない無敵時間。
    {
        if(thisCommonEnemy == false)
        {
            invincibleTime = invincibleTime_L;
        }
    }


    public void St_Inflict_SlowNess(float slowNessTime_L)//移動速度低下の時間。
    {
        if (slowTime <= slowNessTime_L) slowTime = slowNessTime_L;//もし追加しようしているタイムの方が大きい場合、そちらを優先させる。

    }

    public void St_Inflict_NoMove(float nomoveNessTime_L)//移動不可の時間。
    {
        noMoveTime = nomoveNessTime_L;
    }

    public void St_Inflict_Shock(float shockNessTime_L,int level)//ショック状態の時間。
    {
        if (level <= 1)//受けたショック状態のレベルが1より下であり、スーパーアーマーが無い場合は影響を受ける。
        {
            if(superArmorSt == false)
            {
                noMoveTime = shockNessTime_L;
                shockTime = shockNessTime_L;
            }
        }
        else if (level >= 2)//受けたショック状態のレベルが2以上であればスーパーアーマーの効果を無効にする。
        {
            noMoveTime = shockNessTime_L;
            shockTime = shockNessTime_L;
        }
    }

    public void St_Inflict_Vulnerable(float vulnerableTime_L)//脆弱状態の時間。
    {
        //受けた脆弱状態の時間を足す。
        vulnerableTime += vulnerableTime_L;
    }

    void ManaCharge()//時間経過によるマナ回復。
    {
        if (maxManaPoint > manaPoint) manaProgressTime += 1 * Time.deltaTime;
        else if(manaPoint > maxManaPoint) manaPoint = maxManaPoint;

        if (manaProgressTime >= manaWantTime)
        {
            manaPoint++;//マナを1回復。
            manaProgressTime = 0;//時間経過を初期化。
        }
        
    }

    void ConditionCheck()//状態の検出を行う。----------------------------------------------------
    {
        float deltaTime = Time.deltaTime;//デルタタイムをこちらで生成させ、渡す。
        
        if(thisCommonEnemy == false)
        {
            //無敵状態：プレイヤー自身がダメージを一切受け付けなくなる。
            if (invincibleTime >= 0.0f && invincibleTime != 0)
            {
                if (invincibleSt != true) invincibleSt = true;
                invincibleTime -= 1 * deltaTime;
            }
            else if (invincibleSt != false)
            {
                invincibleTime = 0;
                invincibleSt = false;
            }
        }
        

        //移動速度低下：プレイヤーの移動速度が低下する。
        if (slowTime >= 0.0f && slowTime != 0)//slowTimeが0以上の数値になっている場合、移動低下を発生させながら時間を減らす。
        {
            if (moveMag != 0.7f) moveMag = 0.7f;//移動速度低下
            slowTime -= 1 * deltaTime;
        }
        else if (moveMag != 1.0f)
        {
            slowTime = 0;
            moveMag = 1.0f;//移動速度倍率を１に戻す。
        }

        //移動不可能：移動操作が出来なくなる。視点は操作可能。
        if (noMoveTime >= 0.0f && noMoveTime != 0)
        {
            if (noMoveSt != true) noMoveSt = true;
            noMoveTime -= 1 * deltaTime;
        }
        else if (noMoveSt != false)
        {
            noMoveTime = 0;
            noMoveSt = false;
        }

        //ショック状態：のけ反り状態を指す。現在行っている動作を停止、移動・視点・カード選択 使用の操作が出来なくなる。
        if(shockTime >= 0.0f && shockTime != 0)
        {
            if (shockSt != true) shockSt = true;
            shockTime -= 1 * deltaTime;
        }
        else if(shockSt != false)
        {
            shockTime = 0;
            shockSt = false;
        }

        //脆弱状態：次に受けるダメージが1.5倍になる。一定時間立つかダメージを受けると解除される。
        if (vulnerableTime >= 0.0f && vulnerableTime != 0)
        {
            if (vulnerableSt != true) vulnerableSt = true;
            vulnerableTime -= 1 * deltaTime;
        }
        else if (vulnerableSt != false)
        {
            vulnerableTime = 0;
            vulnerableSt = false;
        }
    }
}
