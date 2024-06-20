using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_PushLater : MonoBehaviour
{
    public SpellData spellDate;//アタッチしたスペルデータ。

    Rigidbody rb;
    AudioSource audioSource;

    SpellPrefabManager spm;
    GameObject ownerObj;//所有者。
    string ownerTag;//所有者のタグ
    AudioSource ownerAS;
    public GameObject modelObject;//このスペルのモデル。

    ////エフェクト
    public ParticleSystem shotEffect;//射撃時のエフェクト。
    public ParticleSystem muzzleFlashEffect;//射撃時マズルフラッシュエフェクト。
    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト。
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト
    public ParticleSystem windEffect;//起爆時のエフェクト

    public ParticleSystem pushDirectionalEffect;
    //------------

    int damage;//このスペルのダメージ格納用。

    public float speed;//このスペルの速度格納用。
    bool hitSW;//当たっているかどうか。
    Vector3 hitPos;//当たった位置を格納。レイが当たった位置を格納し必要な際に呼び出す。

    string rootString;

    GameObject hitObj;
    StatusManager hitSM;
    

    float processTime;//生成されてからの経過時間

    float stickProcessTime;//当たってからの経過時間を格納する為の変数。
    float goalTime;

    //効果音
    public AudioClip shotSE;//生成時に発生する音。
    public AudioClip hitSE;//当たった際の音。
    public AudioClip blastSE;//爆発時になる音。
    AudioSource otherAS;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        spm = GetComponent<SpellPrefabManager>();
        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//所有者のタグをこのオブジェクトに渡す。
        gameObject.layer = ownerObj.layer;//所有者のレイヤーをこのオブジェクトに渡す。
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;
        ownerAS = ownerObj.GetComponent<AudioSource>();

        ownerAS.PlayOneShot(shotSE);

        speed = 28.0f;
        goalTime = 3.0f;

        rootString = "ShellMode";//シェルモードに移行。

        Instantiate(muzzleFlashEffect, transform.position, transform.rotation);
        Instantiate(shotEffect, transform.position, Quaternion.identity);
    }

    void Update()
    {
        switch(rootString)
        {
            case "ShellMode": //発射された際の状態。敵にヒットすると次の処理に移行する。
                ShellMode();
                break;
            case "Stick_at": //対象にくっつく処理を行う。
                Stick_at();
                break;
            case "Stick_TimeProcess": //くっ付いてからの時間経過処理。
                Stick_TimeProcess();
                break;
            case "Blast_Impact": //爆発の処理。
                Blast_Impact();
                break;
        }
    }

    void FixedUpdate()
    {
        if(hitSW == false)
        {
            rb.velocity = (transform.forward * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hitSW == false)
        {
            if (other.tag != ownerTag && other.tag != "Search")
            {
                hitObj = other.gameObject;

                if (other.transform.GetComponent<StatusManager>() != null)
                {
                    gameObject.transform.position = other.transform.position;//ぶつかったオブジェクトの位置に移動させる。

                    gameObject.transform.parent = other.transform;//当たったオブジェクトの子に入れる。
                    hitSM = other.GetComponent<StatusManager>();

                    otherAS = other.GetComponent<AudioSource>();
                    otherAS.PlayOneShot(hitSE);

                    hitSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                    hitSM.St_Inflict_Shock(0.1f, 1);//ショック状態レベル1を0.1秒与える。

                    Instantiate(targetHitEffect, hitObj.transform.position, Quaternion.identity);

                    Instantiate(pushDirectionalEffect, gameObject.transform.position, transform.rotation,transform);
                }
                else//静的なオブジェクトにヒットした場合。
                {
                    Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
                    audioSource.PlayOneShot(hitSE);
                }
                
                
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;//重力を無効化する。
                modelObject.SetActive(false);//モデル表示を無効化する。

                Stick_at();
            }
        }
        
    }

    void ShellMode()
    {
        /*
        //レイキャストを使った接着地点取得。
        Vector3 ori = gameObject.transform.position;
        Ray ray = new Ray(ori, transform.forward);


        int outMask = ~LayerMask.GetMask(new string[] { "Search", ownerTag });//所有者タグ名と同じレイヤーを除外。

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f, outMask))
        {
            if (hit.point == null) hitPos = Vector3.zero;
            else if(hitPos != hit.point) hitPos = hit.point;
        }
        */
        //破壊までの時間。
        processTime += 1 * Time.deltaTime;
        if(processTime >= 3)
        {
            Destroy(gameObject);
        }
    }

    void Stick_at()//くっ付くいた時の処理。終わった後に時間処理に移行。
    {
        hitSW = true;

        
        // if(hitPos != Vector3.zero) gameObject.transform.position = hitPos;//レイキャストの当たった位置に移動させる。

        if (hitObj.GetComponent<Rigidbody>() != null)//相手が重力に影響を受ける場合、その方向へ力を加える。
        {
            hitObj.GetComponent<Rigidbody>().AddForce(transform.forward * 40, ForceMode.Impulse);//相手を吹き飛ばす。
        }
        
        stickProcessTime = 0;//処理時間を０にする。
        rootString = "Stick_TimeProcess";
    }

    void Stick_TimeProcess()//時間処理。一定時間立つと爆発処理に移行。
    {
        stickProcessTime += 1 * Time.deltaTime;
        if(stickProcessTime >= goalTime)
        {
            rootString = "Blast_Impact";//処理を変更。
        }
    }

    void Blast_Impact()//爆発処理。
    {
        Instantiate(windEffect, gameObject.transform.position, Quaternion.identity);

        if (hitObj.GetComponent<Rigidbody>() != null)//相手が重力に影響を受ける場合、その方向へ力を加える。
        {
            otherAS.PlayOneShot(blastSE);

            hitSM.St_Inflict_Shock(0.2f, 2);
            hitObj.GetComponent<Rigidbody>().AddForce(transform.forward * 200, ForceMode.Impulse);//相手を吹き飛ばす。
        }
        else
        {
            audioSource.PlayOneShot(blastSE);
        }

        Destroy(gameObject);
    }
}
