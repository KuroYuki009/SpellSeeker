using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Gatling : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ

    public StatusManager ownerSM;

    string swirchRoute;//条件分 用。

    int magazineSizeInt;//発射する数。

    float intervalMax;
    float intervalTime;

    public GameObject instBulletObject;

    public List<Transform> instBarrelTF;
    int useBarrelInt;

    //エフェクト類
    public ParticleSystem shotEffect;

    //効果音類
    AudioSource audioSource;
    public AudioClip shotSE;

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        audioSource = GetComponent<AudioSource>();
        
        ownerObj = spm.ownerObject;
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        gameObject.tag = ownerTag;
        gameObject.layer = ownerObj.layer;

        if(ownerObj.GetComponent<HandCardManager>() != null)//ハンドカードマネージャーを持っている場合、
        gameObject.transform.parent = ownerObj.GetComponent<HandCardManager>().instPos.transform;//HCMの射撃TFの子に設定する。

        ownerSM = ownerObj.GetComponent<StatusManager>();//ショック状態かのステータスを取得。

        magazineSizeInt = 16;
        intervalMax = 0.08f;

        swirchRoute = "Fire";
    }


    void Update()
    {
        switch(swirchRoute)
        {
            case "FireInterval":
                FireInterval();
                break;

            case "Fire":
                Fire();
                break;
        }
        if (ownerSM.shockSt == true)//所有者がショック状態だった場合、
        {
            //この攻撃を完全に無力化する。
            Destroy(gameObject);
        }
    }

    void FireInterval()
    {
        intervalTime -= 1 * Time.deltaTime;
        if(intervalTime <= 0)
        {
            swirchRoute = "Fire";
        }
    }

    void Fire()
    {
        if(useBarrelInt >= instBarrelTF.Count)//Barrel数より使用済みBarrelの数が上回った場合、
        {
            useBarrelInt = 0;//カウント回数をリセットする。
        }


        GameObject instbullet = Instantiate(instBulletObject, instBarrelTF[useBarrelInt].transform.position, instBarrelTF[useBarrelInt].transform.rotation);
        GatlingBullet gbLocal = instbullet.GetComponent<GatlingBullet>();
        instbullet.tag = ownerTag;
        instbullet.layer = ownerObj.layer;

        gbLocal.ownerObj = ownerObj;
        gbLocal.ownerTag = ownerTag;

        gbLocal.damage = spellDate.primaryDamage;

        Instantiate(shotEffect, transform.position, transform.rotation,ownerObj.transform);//エフェクト生成

        audioSource.PlayOneShot(shotSE);

        useBarrelInt++;
        magazineSizeInt--;

        if (magazineSizeInt <= 0)//残り発射回数が0をになっている場合、
        {
            Destroy(gameObject, 2);//二秒後に本体を破壊。
            swirchRoute = null;//処理を停止。
        }
        else//そうでなければ処理を続行する。
        {
            intervalTime = 0;
            intervalTime += intervalMax;
            swirchRoute = "FireInterval";
        }
    }
}
