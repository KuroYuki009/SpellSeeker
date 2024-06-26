using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Rubato : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。

    
    [Header("エフェクト")]//---
    #region
    [Tooltip("発射時に 描写されるエフェクトです。")]
    public ParticleSystem shotEffect;// 射撃時のエフェクト。

    [Tooltip("準備完了時に 描写されるエフェクトです。")]
    public ParticleSystem standbyEffect;// 準備完了時のエフェクト。加速時に発生させる。

    [Tooltip("標的に ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem targetHitEffect;// 対象へのヒットエフェクト。

    [Tooltip("何かしらに ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem hitEffect;// 対象以外のオブジェクトへのヒットエフェクト
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("発射時に 再生されるサウンドです。")]
    public AudioClip shotSE;

    [Tooltip("何かしらに ヒットした時に 再生されるサウンドです。")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ

    Rigidbody rb;
    AudioSource audioSource;

    int damage;// このオブジェクトのダメージ値。
    float speed;// このオブジェクトの移動スピード値。


    bool highSpeedSW;// 高速速度に移行したかの二極値。

    float elapsedTime;//経過時間を保管する為の入れ物。

    ////--------------------------------------------------

    void Start()
    {
        speed = 10f;
        rb = GetComponent<Rigidbody>();
        spm = GetComponent<SpellPrefabManager>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//所有者のタグをこのオブジェクトに渡す。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(shotSE);

        Instantiate(shotEffect, gameObject.transform.position, Quaternion.identity);
    }

    
    void Update()
    {
        elapsedTime += 1 * Time.deltaTime;

        if (elapsedTime >= 10.0f)
            Destroy(gameObject);
        else if (elapsedTime >= 2.0f && highSpeedSW == false)
        {
            Instantiate(shotEffect, gameObject.transform.position, Quaternion.identity);
            audioSource.pitch = 1.6f;
            audioSource.PlayOneShot(shotSE);
            speed = 14.0f;
            highSpeedSW = true;
        }
        else if (elapsedTime >= 0.1f && elapsedTime <= 2f) speed = 0.8f;

    }

    void SpellCast()//スペル使用を検知。させたい。ナ....
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;//前進させる。
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
                //otherSM.St_Inflict_Invincible(0.4f);//0.2秒の無敵時間を付与する。

                AudioSource otherAS = other.GetComponent<AudioSource>();
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
