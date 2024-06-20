using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_DoubleTap : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;
    string ownerTag;

    public LineRenderer lr;

    public ParticleSystem shotEffect;//射撃時のエフェクト
    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト

    int damage;

    int tapCount;//一度に何回射撃を行うか。
    //int magCount;//残り何回射撃を行うか。
    bool shotTrigger;//射撃済みかどうかを検出する。

    GameObject instPos;//ownerオブジェクトが持っているinstPosを取得、格納する。

    float elapsedTime;//経過時間
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        lr = GetComponent<LineRenderer>();
        
        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//所有者となるオブジェクトを格納する。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = spm.ownerObject.tag;//所有者のタグを格納する。
        instPos = ownerObj.GetComponent<HandCardManager>().instPos;

        LineView();

        tapCount = 2;
    }

    void Update()
    {
        if (tapCount != 0)
        {
            if (shotTrigger == false)
            {
                Debug.Log("タップショット！！");
                Shot();
            }
            else if (shotTrigger == true)//トリガーを引かれた後。またタップカウントが0ではない場合。
            {
                elapsedTime += 1 * Time.deltaTime;
                if (elapsedTime >= 0.3f)
                {

                    elapsedTime = 0;
                    shotTrigger = false;
                }
                else if (elapsedTime >= 0.1f)
                {
                    //LineHide(); 
                }
            }
            else Destroy(gameObject, 0.1f);
        } 
    }

    void Shot()
    {
        
        Instantiate(shotEffect, instPos.transform.position, Quaternion.identity);

        Vector3 ori = instPos.transform.position;
        Ray ray = new Ray(ori, transform.forward);


        //int outMask = LayerMask.GetMask(new string[] { " "});//レイヤー" "

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f))
        {
            

            lr.SetPosition(0, instPos.transform.position);
            lr.SetPosition(1, hit.point);
            

            if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != ownerTag)
            {
                Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//当たったエフェクト
                StatusManager sm = hit.transform.GetComponent<StatusManager>();
                sm.HP_Inflict_Damage(damage);//ダメージを発生させる。
                if(sm.hitPoint <= 0 || sm.hitPoint == 0 || sm.gameObject == null)//当たった対象の体力が0、もしくは消滅済みの場合
                {
                    //なにか効果を書いてどうぞ
                }

            }
            else
                Instantiate(hitEffect, hit.point, Quaternion.identity);//エフェクトを生成。
        }


        //Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 1.0f);//飛ばしたRayの軌道を見る。
         
        tapCount -= 1;
        shotTrigger = true;
    }

    void LineHide()
    {
        lr.startWidth = 0.0f;
        lr.endWidth = 0.0f;
    }
    void LineView()
    {
        lr.startWidth = 0.15f;
        lr.endWidth = 0.15f;
    }
}
