using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_PushLater : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。


    [Header("モデル オブジェクト")]//---
    [Tooltip("モデルの制御に使用されます。")]
    public GameObject modelObject;//このスペルのモデル。


    [Header("エフェクト")]//---
    #region
    [Tooltip("射撃時に 描写されるエフェクトです。")]
    public ParticleSystem shotEffect;// 射撃時のエフェクト。

    [Tooltip("マズルフラッシュの 描写エフェクト")]
    public ParticleSystem muzzleFlashEffect;// マズルフラッシュエフェクト。

    [Tooltip("標的に ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem targetHitEffect;// 対象へのヒットエフェクト。

    [Tooltip("何かしらに ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem hitEffect;// 対象以外のオブジェクトへのヒットエフェクト

    [Tooltip("起爆時に 描写されるエフェクトです。")]
    public ParticleSystem windEffect;// 起爆時のエフェクト

    [Tooltip("力を加える方向を 描写するエフェクトです。")]
    public ParticleSystem pushDirectionalEffect;// ヒット後に描写される指方向エフェクト。
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("射撃時に 再生されるサウンドです。")]
    public AudioClip shotSE;//射撃時に発生する音。

    [Tooltip("何かしらに ヒットした時に 再生されるサウンドです。")]
    public AudioClip hitSE;//当たった際の音。

    [Tooltip("起爆時に 再生されるサウンドです。")]
    public AudioClip blastSE;//爆発時に鳴らす音。
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    Rigidbody rb;
    AudioSource audioSource;

    GameObject ownerObj;// 所有者。
    string ownerTag;// 所有者のタグ
    AudioSource ownerAS;// 所有者オブジェクトのAudioSource

    int damage;// このオブジェクトのダメージ値
    float speed;// このオブジェクトの移動スピード値


    string rootString;

    GameObject hitObj;
    StatusManager hitSM;
    AudioSource otherAS;

    float processTime;//生成されてからの経過時間

    bool hitSW;// 当たっているかどうか。
    float stickProcessTime;// 当たってからの経過時間を格納する為の変数。
    float goalTime;// 終点となる時間。

    ////--------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        spm = GetComponent<SpellPrefabManager>();
        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//所有者のタグをこのオブジェクトに渡す。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;
        ownerAS = ownerObj.GetComponent<AudioSource>();

        ownerAS.PlayOneShot(shotSE);

        speed = 28.0f;
        goalTime = 2.0f;//二秒後に起動させる。

        rootString = "ShellMode";//シェルモードに移行。

        Instantiate(muzzleFlashEffect, transform.position, transform.rotation);// マズルフラッシュのエフェクト生成。
        Instantiate(shotEffect, transform.position, Quaternion.identity);// 波紋型のエフェクト生成。
    }

    void Update()
    {
        switch(rootString)
        {
            case "ShellMode": //発射された際の状態。敵にヒットすると次の処理に移行する。
                ShellMode();
                break;
            case "Stick_at": //対象にくっつく処理を行う。
                Stick_at();
                break;
            case "Stick_TimeProcess": //くっ付いてからの時間経過処理。
                Stick_TimeProcess();
                break;
            case "Blast_Impact": //爆発の処理。
                Blast_Impact();
                break;
        }
    }

    void FixedUpdate()
    {
        if(hitSW == false)//当たるまで進み続ける。
        {
            rb.velocity = (transform.forward * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string hitObjectTag = other.tag;

        if(hitSW == false)
        {
            if (hitObjectTag != ownerTag && hitObjectTag != "Search")
            {
                hitObj = other.gameObject;

                if (other.transform.GetComponent<StatusManager>() != null)
                {
                    gameObject.transform.position = other.transform.position;//ぶつかったオブジェクトの位置に移動させる。

                    gameObject.transform.parent = other.transform;//当たったオブジェクトの子に入れる。
                    hitSM = other.GetComponent<StatusManager>();

                    otherAS = other.GetComponent<AudioSource>();
                    otherAS.PlayOneShot(hitSE);

                    hitSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                    hitSM.St_Inflict_Shock(0.1f, 1);//ショック状態レベル1を0.1秒与える。

                    Instantiate(targetHitEffect, hitObj.transform.position, Quaternion.identity);

                    Instantiate(pushDirectionalEffect, gameObject.transform.position, transform.rotation,transform);

                    HitToEnd();
                }
                else if (hitObjectTag == "Player_1" || hitObjectTag == "Player_2" || hitObjectTag == "Player_3" || hitObjectTag == "Player_4")// プレイヤー識別のタグだった場合
                {
                    //何も起こしません。
                }
                else//静的なオブジェクトにヒットした場合。
                {
                    Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
                    audioSource.PlayOneShot(hitSE);

                    hitSW = true;

                    Destroy(gameObject);
                }
                
                void HitToEnd()//何かしらに当たった後の移行処理
                {
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true;//重力を無効化する。
                    modelObject.SetActive(false);//モデル表示を無効化する。

                    Stick_at();
                }
            }
        }
        
    }

    void ShellMode()
    {
        //破壊までの時間。
        processTime += 1 * Time.deltaTime;
        if(processTime >= 3.0f)
        {
            Destroy(gameObject);
        }
    }

    void Stick_at()//くっ付くいた時の処理。終わった後に時間処理に移行。
    {
        hitSW = true;

        if (hitObj.GetComponent<Rigidbody>() != null)//相手が重力に影響を受ける場合、その方向へ力を加える。
        {
            hitObj.GetComponent<Rigidbody>().AddForce(transform.forward * 40, ForceMode.Impulse);//相手を吹き飛ばす。40
        }
        
        stickProcessTime = 0;//処理時間を０にする。
        rootString = "Stick_TimeProcess";
    }

    void Stick_TimeProcess()//時間処理。一定時間立つと爆発処理に移行。
    {
        stickProcessTime += 1 * Time.deltaTime;
        if(stickProcessTime >= goalTime)
        {
            rootString = "Blast_Impact";//処理を変更。
        }

        if (hitSM.hitPoint <= 0) Destroy(gameObject);// もし、時間内に当たった対象の体力が0になったら、
    }

    void Blast_Impact()//爆発処理。
    {
        Instantiate(windEffect, gameObject.transform.position, Quaternion.identity);

        if (hitObj.GetComponent<Rigidbody>() != null)//相手が重力に影響を受ける場合、その方向へ力を加える。
        {
            otherAS.PlayOneShot(blastSE);

            hitSM.St_Inflict_Shock(0.2f, 2);
            hitObj.GetComponent<Rigidbody>().AddForce(transform.forward * 300, ForceMode.Impulse);//相手を吹き飛ばす。200
        }
        else
        {
            audioSource.PlayOneShot(blastSE);
        }

        Destroy(gameObject);
    }
}
