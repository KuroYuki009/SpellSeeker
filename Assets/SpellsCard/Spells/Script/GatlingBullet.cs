using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBullet : MonoBehaviour
{
    Rigidbody rb;

    public GameObject ownerObj;//所有者。
    public string ownerTag;

    float speed;//弾の速度。
    public int damage;//威力。

    //float elapsedTime;//経過時間。

    //エフェクト類。
    public ParticleSystem hitEffect;
    public ParticleSystem targetHitEffect;

    //効果音類。
    public AudioClip hitSE;
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
        if (other.tag != ownerTag && other.tag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                //otherSM.St_Inflict_Invincible(0.4f);//0.2秒の無敵時間を付与する。

                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect,transform.position, Quaternion.identity);
            }
            //else Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
