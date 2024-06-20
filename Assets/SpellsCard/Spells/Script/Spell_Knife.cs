using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Knife : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ

    public ParticleSystem shotEffect;//射撃時のエフェクト。
    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト。
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト

    Rigidbody rb;

    //テスト用のスペルです。
    int damage;
    public float speed;

    public AudioClip shotSE;
    public AudioClip hitSE;
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

        speed = 22.0f;
        Instantiate(shotEffect, transform.position, Quaternion.identity);
    }

    void FixedUpdate()
    {
        rb.velocity = (transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != ownerTag && other.tag != "Search")
        {
            /*
            if(other.transform.GetComponent<AudioSource>() != null)
            {
                AudioSource otherAS = other.transform.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);
            }
            */
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
            }
            else Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
