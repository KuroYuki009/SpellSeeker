using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShootNote : MonoBehaviour
{
    public GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ

    public GameObject modelObject;//子付けされているモデルオブジェクト。

    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト。
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト

    public int damage;//対象に与えるダメージ。発射側からダメージを代入してもらう。
    float speed;//このオブジェクトの移動スピード

    Rigidbody rb;

    AudioSource audioSource;
    public AudioClip shotSE;
    public AudioClip hitSE;
    void Start()
    {
        if (ownerObj == null) return;
        if (modelObject != null) modelObject.GetComponent<LayerConverter>().parentDate = gameObject;
        rb = GetComponent<Rigidbody>();
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;
        speed = 10;//スピードを入れる。

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(shotSE);
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
                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                other.transform.GetComponent<StatusManager>().HP_Inflict_Damage(damage);//ダメージを発生させる。

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
