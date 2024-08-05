using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_SpreadShoot : MonoBehaviour
{
    [Header("スペルデータ")]//---
    public SpellData spellDate;// 各種データ参照に使用される。


    [Header("発射するオブジェクト")]//---
    #region
    [Tooltip("発射するオブジェクトを設定出来ます。")]
    public GameObject shootObject;//発射されるオブジェクトを設定。

    [Tooltip("発射する為の始点を設定できます。")]
    public List<Transform> instPos;//射出に使用する位置を定義。
    #endregion


    [Header("エフェクト")]//---
    #region
    [Tooltip("射撃時に 描写されるエフェクトです。")]
    public GameObject shotEffect;//射撃時のエフェクト。

    [Tooltip("準備完了時に 描写されるエフェクトです。")]
    public GameObject standbyEffect;//準備完了時のエフェクト。射出前に発生させる。
    #endregion


    [Header("サウンド")]//---
    #region
    [Tooltip("チャージ中に 再生されるサウンドです。")]
    public AudioClip chargeSE;
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;//所有者からデータを引用する為に。

    AudioSource audioSource;

    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ

    int damage;//対象に与えるダメージ


    string swirchRoute;//処理を分ける為に使用しているSwitch文を制御する為。
    float elapsedTime;//経過時間を保管する為の入れ物。

    int shotCount = 0;//何回発射したかをカウントする。

    GameObject C_Effect;//生成したエフェクトを一時的に格納。

    ////--------------------------------------------------

    void Start()
    {
        damage = spellDate.primaryDamage;

        spm = GetComponent<SpellPrefabManager>();
        ownerObj = spm.ownerObject;//所有者のタグをこのオブジェクトに渡す。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = chargeSE;
        audioSource.Play();

        C_Effect = Instantiate(shotEffect, gameObject.transform.position, Quaternion.identity);//チャージエフェクトを生成。

        swirchRoute = "Charge";
    }

    
    void Update()
    {
        switch(swirchRoute)
        {
            case "Charge":
                elapsedTime += 1 * Time.deltaTime;
                if (elapsedTime >= 0.4f)
                {
                    Destroy(C_Effect);
                    swirchRoute = "SpShoot";
                }
                break;
            case "SpShoot":
                SpShoot();
                if (shotCount == 5 || shotCount >= 5) Destroy(gameObject);//五回生成したらこのオブジェクトを破壊。
                break;
        }
    }

    void SpShoot()
    {
        Instantiate(standbyEffect, instPos[shotCount].transform.position, Quaternion.identity);//射出場所にエフェクト生成。

        GameObject shotNote = Instantiate(shootObject, instPos[shotCount].transform.position, instPos[shotCount].transform.rotation);//車室場所から弾を発射。
        shotNote.GetComponent<SpreadShootNote>().damage = damage;//発射物に付いているScript内部の変数「damage」にこちらのダメージを入れる。
        shotNote.GetComponent<SpreadShootNote>().ownerObj = ownerObj;//発射物に付いているScript内部の変数「ownerObj」にこちらの所有者情報を入れる
        shotCount++;
    }
}
