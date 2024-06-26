using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Straight : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。


    [Header("エフェクト")]//---
    #region
    [Tooltip("射撃時に 描写されるエフェクトです。")]
    public ParticleSystem shotEffect;// 射撃時のエフェクト

    [Tooltip("対象に ヒットした際に 描写されるエフェクトです。")]
    public ParticleSystem targetHitEffect;// 対象へのヒットエフェクト

    [Tooltip("何かしらに ヒットした際に 描写されるエフェクトです。")]
    public ParticleSystem hitEffect;// 他オブジェクトへのヒットエフェクト
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("射撃時に 再生させるサウンドです。")]
    public AudioClip shotSE;// 射撃時の効果音。

    [Tooltip("何かしらにヒットした時に 再生されるサウンドです。")]
    public AudioClip hitSE;// ヒット時の効果音。
    #endregion

    ////--------------------------------------------------
    
    SpellPrefabManager spm;

    LineRenderer tr;

    GameObject ownerObj;//所有者オブジェクト。
    string ownerTag;//所有者のタグ

    int damage;//対象に与えるダメージ値 変数。

    bool shotSW;//射撃したかの二極値。

    ////--------------------------------------------------
    
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        tr = GetComponent<LineRenderer>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//所有者となるオブジェクトを格納する。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = spm.ownerObject.tag;//所有者のタグを格納する。

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
                string hitObjectTag = hit.transform.tag;//ヒット対象のタグを検出。
                StatusManager hitObjectStatusM = null;

                if (hit.transform.GetComponent<StatusManager>() != null) hitObjectStatusM = hit.transform.GetComponent<StatusManager>();// StatusManager SCを持っていたらそのままキャッシュ。

                tr.SetPosition(0, gameObject.transform.position);
                tr.SetPosition(1, hit.point);
                tr.enabled = true;

                if (hitObjectStatusM != null && hitObjectTag != ownerTag)
                {
                    AudioSource hitAS = hit.transform.GetComponent<AudioSource>();
                    hitAS.PlayOneShot(hitSE);

                    Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//当たったエフェクト
                    hitObjectStatusM.HP_Inflict_Damage(damage);//40ダメージを発生させる。
                }
                else Instantiate(hitEffect, hit.point, Quaternion.identity);//エフェクトを生成。

                shotSW = true;

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
