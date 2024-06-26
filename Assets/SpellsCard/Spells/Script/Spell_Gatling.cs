using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Gatling : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。

    
    [Header("発射するオブジェクト")]//---
    #region
    [Tooltip("マガジンサイズ。このスペルが弾を発射できる数")]
    public int magazineSizeInt;//発射する弾数。または残弾数

    [Tooltip("発射するオブジェクトを設定出来ます。")]
    public GameObject instBulletObject;//発射する弾となるプレハブオブジェクト。

    [Tooltip("発射する為の始点を設定できます。")]
    public List<Transform> instBarrelTF;// 弾の発射に使用する始点をオブジェクトの座標から利用する。
    #endregion


    [Header("エフェクト")]//---
    #region
    [Tooltip("射撃時に 描写されるエフェクトです。")]
    public ParticleSystem shotEffect;// 射撃時のエフェクト。
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("射撃時に 再生されるサウンドです。")]
    public AudioClip shotSE;// 射撃サウンド。
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ
    [HideInInspector] public StatusManager ownerSM;//所有者のステータスマネージャーをキャッシュ。これは発射される弾に渡される。
    GameObject hcmInstPos;// HandCardManagerのスペル始点部分。

    AudioSource audioSource;


    string swirchRoute;// switch文に使用。

    int useBarrelInt;// 発射の始点を段階的に切り替える際に使用する カウント値。

    float intervalMax;
    float intervalTime;

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        audioSource = GetComponent<AudioSource>();
        
        ownerObj = spm.ownerObject;
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        gameObject.tag = ownerTag;
        gameObject.layer = ownerObj.layer;

        if(ownerObj.GetComponent<HandCardManager>() != null)//ハンドカードマネージャーを持っている場合、
        {
            hcmInstPos = ownerObj.GetComponent<HandCardManager>().instPos;// スペルの発射始点を取得。

            Transform sameObject = null;
            sameObject = hcmInstPos.transform.Find(this.gameObject.name);// 現在、発動中の所持型同一スペルがあるかを探す。

            if (sameObject == null)// 何もなかった場合、
            {
                gameObject.transform.parent = hcmInstPos.transform;// スペル発射始点の子に入れる。
            }

            else if(sameObject.name == gameObject.name) // 同一名のスペルがあった場合には
            {
                sameObject.GetComponent<Spell_Gatling>().magazineSizeInt += 12;// 発動中のマシンガンの残り残弾数にプラス12発追加する。
                Destroy(gameObject);// そして "このスペル" を破棄する。
            }
        }

        ownerSM = ownerObj.GetComponent<StatusManager>();// 所有者のステータスマネージャーを取得。

        intervalMax = 0.08f;// 射撃間のインターバル時間を設定。

        swirchRoute = "Fire";
    }


    void Update()
    {
        switch(swirchRoute)
        {
            case "FireInterval":
                FireInterval();
                break;

            case "Fire":
                Fire();
                break;
        }
        if (ownerSM.shockSt == true)//所有者がショック状態だった場合、
        {
            //この攻撃を完全に無力化する。
            Destroy(gameObject);
        }
    }

    void FireInterval()// 一発撃つごとに発生するインターバル処理
    {
        intervalTime -= 1 * Time.deltaTime;
        if(intervalTime <= 0)
        {
            swirchRoute = "Fire";
        }
    }

    void Fire()
    {
        if(useBarrelInt >= instBarrelTF.Count)//Barrel数より使用済みBarrelの数が上回った場合、
        {
            useBarrelInt = 0;//カウント回数をリセットする。
        }


        GameObject instbullet = Instantiate(instBulletObject, instBarrelTF[useBarrelInt].transform.position, instBarrelTF[useBarrelInt].transform.rotation);
        GatlingBullet gbLocal = instbullet.GetComponent<GatlingBullet>();
        instbullet.tag = ownerTag;
        instbullet.layer = ownerObj.layer;

        gbLocal.ownerObj = ownerObj;
        gbLocal.ownerTag = ownerTag;

        gbLocal.damage = spellDate.primaryDamage;

        audioSource.PlayOneShot(shotSE);// 射撃 効果音再生

        Instantiate(shotEffect, transform.position, transform.rotation,ownerObj.transform);// 射撃 エフェクト生成

        useBarrelInt++;// 射撃始点 カウント値を増加
        magazineSizeInt--;// 残弾数から弾を現象させる。
        
        if (magazineSizeInt <= 0)//残り残弾数が0をになっている場合、
        {
            swirchRoute = null;//処理を停止。
            Destroy(gameObject);//破棄する。
        }
        else//そうでなければ処理を続行する。
        {
            intervalTime = 0;
            intervalTime += intervalMax;
            swirchRoute = "FireInterval";
        }
    }
}
