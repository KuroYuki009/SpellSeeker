using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Knife : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。


    [Header("エフェクト")]//---
    #region
    [Tooltip("射撃時に 描写されるエフェクトです。")]
    public ParticleSystem shotEffect;//射撃時のエフェクト。

    [Tooltip("標的に ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト。

    [Tooltip("何かしらに ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("射撃時に 再生されるサウンドです。")]
    public AudioClip shotSE;

    [Tooltip("何かしらに ヒットした時に 描写されるエフェクトです。")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    Rigidbody rb;

    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ

    int damage;// このオブジェクトのダメージ値
    float speed;// このオブジェクトの移動スピード

    ////--------------------------------------------------
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spm = GetComponent<SpellPrefabManager>();
        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//所有者のタグをこのオブジェクトに渡す。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        AudioSource ownerAS = ownerObj.GetComponent<AudioSource>();
        ownerAS.PlayOneShot(shotSE);

        speed = 22.0f;// オブジェクトの移動スピード値を 22 に設定。

        Instantiate(shotEffect, transform.position, Quaternion.identity);// 射撃エフェクトを生成。
    }

    void FixedUpdate()
    {
        rb.velocity = (transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        string hitObjectTag = other.tag;

        if (hitObjectTag != ownerTag && hitObjectTag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                otherSM.St_Inflict_Shock(0.2f, 1);//ショック状態レベル1を0.2秒与える。
                otherSM.St_Inflict_Vulnerable(4.0f);//脆弱状態を4秒与える。
                //otherSM.St_Inflict_Invincible(0.4f);//0.2秒の無敵時間を付与する。

                AudioSource otherAS = other.transform.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect, other.transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
            else if (hitObjectTag == "Player_1" || hitObjectTag == "Player_2" || hitObjectTag == "Player_3" || hitObjectTag == "Player_4")// プレイヤー識別のタグだった場合
            {
                //何も起こしません。
            }
            else
            {
                Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }


            
        }
    }
}
