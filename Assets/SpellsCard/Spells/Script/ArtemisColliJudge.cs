using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtemisColliJudge : MonoBehaviour
{
    public SpellData spellDate;

    int damage;

    public GameObject meinObj;//オブジェクトの本体(親)をアタッチする。
    public string ownerTag;
    public int ownerLayer;

    //エフェクト
    public ParticleSystem hitEffect;
    public ParticleSystem hitDamageEffect;
    //効果音
    public AudioClip hitSE;
    private void Start()
    {
        ownerTag = meinObj.GetComponent<Spell_Artemis>().ownerObj.tag;
        ownerLayer = meinObj.GetComponent<Spell_Artemis>().ownerObj.layer;
        gameObject.tag = ownerTag;
        //gameObject.layer = ownerLayer;
        damage = spellDate.primaryDamage;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != ownerTag && other.tag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                otherSM.St_Inflict_Shock(0.2f, 1);//ショック状態レベル1を0.2秒与える。
                //otherSM.St_Inflict_Invincible(0.4f);//0.2秒の無敵時間を付与する。

                AudioSource otherAS = other.transform.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(hitDamageEffect, other.transform.position, Quaternion.identity);
            }
            else Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(meinObj.gameObject);
        }
    }
}
