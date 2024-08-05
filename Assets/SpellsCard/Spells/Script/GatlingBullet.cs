using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBullet : MonoBehaviour
{
    [Header("エフェクト")]//---
    #region
    [Tooltip("標的に ヒットした時に 描写されるエフェクトです。")]
    public ParticleSystem targetHitEffect;
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("何かしらに ヒットした時に 再生されるサウンドです。")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------
    
    [HideInInspector]public GameObject ownerObj;//所有者。
    [HideInInspector]public string ownerTag;

    Rigidbody rb;


    [HideInInspector]public int damage;// このオブジェクトのダメージ値。
    float speed;// このオブジェクトの移動スピード。

    ////--------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        speed = 24.0f;//スピードを入れる。


        Destroy(gameObject, 5);
    }

    private void FixedUpdate()
    {
        rb.velocity = (transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        string hitObjectTag = other.tag;// 当たったオブジェクトのタグを取得。

        if (hitObjectTag != ownerTag && hitObjectTag != "Search")// 所有者のタグとSearchタグ以外のオブジェクトだった場合、
        {
            if (other.transform.GetComponent<StatusManager>() != null)// 対象がStatusManagerのスクリプトを持っている場合、
            {
                var otherSM = other.GetComponent<StatusManager>();

                otherSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                //otherSM.St_Inflict_Invincible(0.4f);//0.2秒の無敵時間を付与する。

                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect, transform.position, Quaternion.identity);


                Destroy(gameObject);//このオブジェクトを破棄する。
            }
            else if (hitObjectTag =="Player_1" || hitObjectTag == "Player_2" || hitObjectTag == "Player_3" || hitObjectTag == "Player_4")// プレイヤー識別のタグだった場合
            {
                //何も起こしません。
            }
            else Destroy(gameObject);//このオブジェクトを破棄する。
        }
    }
}
