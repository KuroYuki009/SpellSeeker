using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Straight : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;
    string ownerTag;

    public LineRenderer tr;

    public ParticleSystem shotEffect;//射撃時のエフェクト
    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト

    int damage;

    bool shotSW;//撃った後か。

    //
    public AudioClip shotSE;
    public AudioClip hitSE;

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        tr = GetComponent<LineRenderer>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//所有者となるオブジェクトを格納する。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = spm.ownerObject.tag;//所有者のタグを格納する。

        //ownerObj.GetComponent<StatusManager>().St_Inflict_NoMove(0.2f);//所有者に移動不可を付与する。

        Instantiate(shotEffect, transform.position,transform.rotation);

        AudioSource ownerAS = ownerObj.GetComponent<AudioSource>();
        ownerAS.PlayOneShot(shotSE);

        tr.startWidth = 0.4f;
        tr.endWidth = 0.4f;
    }

    private void Update()
    {
        shoting();
    }

    void shoting()
    {
        if (shotSW == false)
        {
            Vector3 ori = gameObject.transform.position;
            Ray ray = new Ray(ori, transform.forward);


            int outMask = ~LayerMask.GetMask(new string[] { "Search",ownerTag });//所有者タグ名と同じレイヤーを除外。

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30f,outMask))
            {
                
                tr.SetPosition(0, gameObject.transform.position);
                tr.SetPosition(1, hit.point);
                tr.enabled = true;

                if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != ownerTag)
                {
                    AudioSource hitAS = hit.transform.GetComponent<AudioSource>();
                    hitAS.PlayOneShot(hitSE);

                    Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//当たったエフェクト
                    hit.transform.GetComponent<StatusManager>().HP_Inflict_Damage(damage);//40ダメージを発生させる。
                }
                else
                {
                    Instantiate(hitEffect, hit.point, Quaternion.identity);//エフェクトを生成。
                }    
                shotSW = true;

                //string name = hit.collider.gameObject.name;//ヒットしたオブジェクトの名前を格納。
                //Debug.Log(name);//ヒットしたオブジェクトの名前をログに出す。
            }
        }
        else if(shotSW == true)
        {
            tr.startWidth -= 0.05f;
            tr.endWidth -= 0.05f;
            if(tr.startWidth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
