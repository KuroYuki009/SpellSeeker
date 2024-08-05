using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_SawDisk : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。


    [Header("エフェクト")]//---
    #region
    [Tooltip("発射時に 描写されるエフェクトです。")]
    public GameObject shotEffect;//射撃時のエフェクト

    [Tooltip("何かしらに ヒットした際に 描写されるエフェクトです。")]
    public GameObject hitEffect;//対象へのヒットエフェクト
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("使用時に 再生されるサウンドです。")]
    public AudioClip shotSE;//生成時に発生する音。

    [Tooltip("標的に ヒットした時に 再生されるサウンドです。")]
    public AudioClip hitSE;//当たった際の音。

    [Tooltip("何かしらに ヒットした際に 再生されるサウンドです。")]
    public AudioClip colliSE;//何かしらにぶつかった音。
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    Rigidbody rb;
    AudioSource audioSource;

    GameObject ownerObj;// 所有者オブジェクト
    string ownerTag;// 所有者のタグ
    AudioSource ownerAS;// 所有者オブジェクトのAudioSource

    int health;// このオブジェクトの体力値。
    int damage;// このオブジェクトのダメージ値。
    float speed;//このオブジェクトの移動スピード値。


    float aliveTime;// 生存時間。

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        health = 200;
        damage = spellDate.primaryDamage;
        speed = 12.0f;

        ownerObj = spm.ownerObject; //所有者となるオブジェクトを格納する。
        ownerTag = spm.ownerObject.tag;

        gameObject.tag = ownerTag;
        gameObject.layer = ownerObj.layer;

        ownerAS = ownerObj.GetComponent<AudioSource>();
        //gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。遮蔽に入った際に専用色で描写される

        ownerAS.PlayOneShot(shotSE);

        rb.velocity = transform.forward * speed;//前進させる。
    }


    void Update()
    {
        aliveTime += 1 * Time.deltaTime;

        if (aliveTime >= 7f || health <= 0)
        {
            Destroy(gameObject);
        }

        SpeedKeeper();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool seSW = false;
        if (collision.gameObject.tag != ownerTag && collision.gameObject.tag != "Search") //所有者以外にヒット。
        {
            if (collision.transform.GetComponent<StatusManager>() != null)
            {
                AudioSource hitAS = collision.gameObject.GetComponent<AudioSource>();//当たった物からオーディオソースを取得。

                if (collision.gameObject.tag == "Installation" && seSW == false)
                {
                    hitAS.PlayOneShot(colliSE);//再生。
                    seSW = true;
                }
                else if(seSW == false)
                {
                    hitAS.PlayOneShot(hitSE);//再生。
                    seSW = true;
                }

                Instantiate(hitEffect, collision.transform.position, Quaternion.identity);

                var otherSM = collision.gameObject.GetComponent<StatusManager>();//一度取得し、格納する。
                otherSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                //otherSM.St_Inflict_Shock(0.2f, 1);//ショック状態レベル1を0.2秒与える。
                //otherSM.St_Inflict_Invincible(0.2f);//0.2秒の無敵時間を付与する。

                health -= 10;
            }
            health -= 10;
        }

        if (collision.gameObject.tag != "Ground" && seSW == false) 
        {
            audioSource.PlayOneShot(colliSE);
            health -= 10;
            aliveTime = 0;//破壊猶予時間をリセット。
            seSW = true;
        }
    }

    void SpeedKeeper()
    {
        Vector3 vt3 = rb.velocity;
        if (rb.velocity.magnitude != speed)
        {
            rb.velocity = vt3.normalized * speed;
        }
    }
}
