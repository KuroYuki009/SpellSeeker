using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Slash : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;//所有者オブジェクト
    string ownerTag;//所有者のタグ

    Collider cd;
    AudioSource ownerAS;

    float aliveTime;//生存時間。
    bool oneHit;//一度当たったかの判定。

    int damage;

    //public GameObject shotEffect;//射撃時のエフェクト
    public GameObject hitEffect;//対象へのヒットエフェクト

    //効果音
    public AudioClip shotSE;//生成時に発生する音。
    public AudioClip hitSE;//当たった際の音。
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        cd = GetComponent<Collider>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject; //所有者となるオブジェクトを格納する。
        ownerTag = spm.ownerObject.tag;
        gameObject.tag = ownerTag;
        ownerAS = ownerObj.GetComponent<AudioSource>();
        //gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。遮蔽に入った際に専用色で描写される

        ownerAS.PlayOneShot(shotSE);

        aliveTime = 0.15f;
    }

    
    void Update()
    {
        aliveTime -= 1 * Time.deltaTime;

        if (aliveTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneHit == false)//一度ヒットしている場合無効化する。
        {
            if(other.tag != ownerTag && other.tag != "Search") //所有者以外にヒット。
            {
                if (other.transform.GetComponent<StatusManager>() != null)
                {
                    AudioSource hitAS = other.GetComponent<AudioSource>();//当たった物からオーディオソースを取得。
                    hitAS.PlayOneShot(hitSE);//再生。

                    Instantiate(hitEffect, other.transform.position, Quaternion.identity);

                    var otherSM = other.GetComponent<StatusManager>();//一度取得し、格納する。
                    otherSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                    otherSM.St_Inflict_Shock(0.2f, 1);//ショック状態レベル1を0.2秒与える。
                    //otherSM.St_Inflict_Invincible(0.4f);//0.8秒の無敵時間を付与する。

                    if (other.GetComponent<Rigidbody>() != null)//相手が重力に影響を受ける場合、その方向へ力を加える。
                    {
                        other.GetComponent<Rigidbody>().AddForce(transform.forward*50, ForceMode.Impulse);//相手を吹き飛ばす。
                    }
                }
                oneHit = true;
            }
        }
    }
}
