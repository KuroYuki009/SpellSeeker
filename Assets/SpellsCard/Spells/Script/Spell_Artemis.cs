using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Artemis : MonoBehaviour
{
    [Header("スペルデータ")]
    public SpellData spellDate;// 各種データ参照に使用される。


    [Header("エフェクト")]
    #region 
    [Tooltip("生成時に 描写するエフェクトです。")]
    public ParticleSystem GeneEffect;//生成時のエフェクト。

    [Tooltip("射撃時に 描写するエフェクトです。")]
    public ParticleSystem shotEffect;//射撃時のエフェクト。

    [Tooltip("標的を 発見した時に 描写するエフェクトです。")]
    public ParticleSystem standbyEffect;//標的の発見時のエフェクト。

    [Tooltip("標的に ヒットした時に 描写するエフェクトです。")]
    public ParticleSystem targetHitEffect;//対象へのヒットエフェクト。

    [Tooltip("何かしらに ヒットした時に描写するエフェクトです。")]
    public ParticleSystem hitEffect;//他オブジェクトへのヒットエフェクト。
    #endregion


    [Header("サウンド")]
    #region 
    [Tooltip("生成時に 再生させるサウンドです。")]
    public AudioClip instSE;

    [Tooltip("飛行モード時に 再生させるサウンドです。")]
    public AudioClip flyingSE;

    [Tooltip("標的を 発見した時に 再生させるサウンドです。")]
    public AudioClip lockonSE;

    [Tooltip("射撃時に 再生させるサウンドです。")]
    public AudioClip shotSE;

    [Tooltip("標的に ヒットした際に 再生させるサウンドです。")]
    public AudioClip hitSE;
    #endregion


    [Header("モデル アニメ")]
    [Tooltip("モデルのアニメ制御に使用されます。")]
    public Animator animator;//モデルのアニメ制御に使用する。

    [Header("発射座標")]
    [Tooltip("定義されたオブジェクトの座標が レーザーの始点になります。")]
    public Transform laserShootPos;//発射する場所をオブジェクトを使用して設定する。

    ////--------------------------------------------------
    
    SpellPrefabManager spm;

    Rigidbody rb;
    LineRenderer lineRenderer;
    AudioSource audioSource;

    [HideInInspector] public GameObject ownerObj;// 所有者オブジェクト
    string ownerTag;// 所有者のタグ

    int damage;// 対象に与えるダメージ値。
    float objSpeed;// このオブジェクトの移動スピード値。


    string switchRoute;// switch文に使用する変数。

    GameObject targetObj;// ターゲットとなるオブジェクト情報を格納。

    float elapsedTime;// 経過時間を格納するための入れ物。
    float exTime;// 射撃前の待機時間を格納するための入れ物。

    bool moveSW;// 前進するかどうかの二極値。
    [HideInInspector] bool searchSW;// 索敵中かどうかの二極値。
    bool rotationSW = true;// 回転

    Quaternion lookRotation;

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//所有者となるオブジェクトを格納する。
        ownerTag = spm.ownerObject.tag;//所有者のタグを格納する。
        //gameObject.tag = ownerTag;

        audioSource.PlayOneShot(instSE);
        audioSource.loop = true;
        audioSource.clip = flyingSE;
        audioSource.Play();

        //Instantiate(shotEffect, transform.position, Quaternion.identity);//スペル使用時のエフェクトを生成。
        lineRenderer.enabled = false;//コンポーネントを有効化。
        objSpeed = 5.0f;
        switchRoute = "Generate";
    }

    void Update()
    {
        switch (switchRoute)
        {
            case "Generate"://生成
                Generate();
                break;
            case "Aviation"://飛行
                Aviation();
                break;
            case "Discover"://発見
                Discocer();
                break;
            case "Fire"://射撃
                Fire();
                break;
            case "FadeOut"://後処理
                FadeOut();
                break;
        }
    }
    void Generate()//生成時の挙動。
    {
        var ownerSM = ownerObj.GetComponent<StatusManager>();
        ownerSM.St_Inflict_NoMove(0.2f);//所有者に移動不可を付与する。
        

        Instantiate(GeneEffect, gameObject.transform.position, Quaternion.identity);//エフェクト生成
        switchRoute = "Aviation";
    }

    void Aviation()//飛行時。索敵時。
    {
        if (moveSW == false) moveSW = true;

        elapsedTime += 1 * Time.deltaTime;//時間を経過させる。

        if (elapsedTime >= 4.0f)//四秒経過するとオブジェクトを削除
        {
            Destroy(gameObject);
        }
        else if(elapsedTime >= 0.4f)//0.4秒経過すると索敵を開始する。
        {
            searchSW = true;
        }

        if(targetObj != null)//ターゲットが設定された場合、処理を発見に変更する。
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.PlayOneShot(lockonSE);
            switchRoute = "Discover";
        }
    }

    void Discocer()//標的を発見時。
    {
        rb.velocity = Vector3.zero;
        exTime += 1 * Time.deltaTime;//射撃までの遅延時間を経過させる。

        if (exTime >= 0.0f)//敵の向きへ回転させるアニメーションを取る
        {
            if (rotationSW == true)
            {
                
                Instantiate(standbyEffect, gameObject.transform.position, Quaternion.identity);
                lookRotation =
                        Quaternion.LookRotation(targetObj.transform.position - transform.position, transform.forward);//敵が自分から見てどの方角にいるかを索敵する。
                rotationSW = false;

            }
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.4f);//回転する。
            animator.SetBool("LockOn", true);//AnimationのBool値.LockOnをtrueに。
        }

        if (exTime >= 0.6f)//ターゲットの方を確実に向き、射撃の処理に移行する。
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 2.0f);
            switchRoute = "Fire";
        }
    }

    void Fire()//射撃動作時。
    {
        audioSource.PlayOneShot(shotSE);

        Vector3 ori = gameObject.transform.position;
        Ray ray = new Ray(ori, transform.forward);

        int layerMask = ~LayerMask.GetMask(new string[] {ownerTag});

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f,layerMask))
        {

            Instantiate(shotEffect, laserShootPos.transform.position, Quaternion.identity);//エフェクト生成
            
            lineRenderer.enabled = true;//ラインレンダリングコンポーネントを有効化。

            lineRenderer.SetPosition(0, laserShootPos.transform.position);
            lineRenderer.SetPosition(1, hit.point);


            if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != ownerTag)
            {
                AudioSource hitAS = hit.transform.GetComponent<AudioSource>();
                hitAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//当たったエフェクト
                var hitSM = hit.transform.GetComponent<StatusManager>();
                hitSM.HP_Inflict_Damage(damage);//ダメージを発生させる。
                hitSM.St_Inflict_Shock(0.2f, 1);//ショック状態レベル1を0.2秒与える。
                //hitSM.St_Inflict_Invincible(0.4f);//0.4秒の無敵時間を付与する。
            }
            else
                Instantiate(hitEffect, hit.point, Quaternion.identity);//エフェクトを生成。

            //string name = hit.collider.gameObject.name;//ヒットしたオブジェクトの名前を格納。
            //Debug.Log(name);//ヒットしたオブジェクトの名前をログに出す。
        }

        switchRoute = "FadeOut";
        //Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 1.0f);//飛ばしたRayの軌道を見る。

    }

    void FadeOut()//行動後後処理。
    {
        if(lineRenderer.startWidth >= 0 || lineRenderer.endWidth >= 0)
        {
            lineRenderer.startWidth -= 0.1f;
            lineRenderer.endWidth -= 0.1f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(moveSW == true)
        {
            rb.velocity = transform.forward * objSpeed;//前進させる。
        }
    }

    private void OnTriggerStay(Collider other)
    {
        string hitObjectTag = other.tag;
        if (searchSW == true && (hitObjectTag != "Search" && hitObjectTag != "Untagged" && hitObjectTag != "Structure" && hitObjectTag != "Installation" && hitObjectTag != "Ground" && hitObjectTag != ownerTag))
        {
            if(other.GetComponent<StatusManager>() != null)
            {
                GameObject target = other.gameObject;

                Vector3 pos = target.transform.position - transform.position;

                Ray ray = new Ray(transform.position, pos);//コリジョンした敵の座標をレイ目標位置に設定。

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20f))
                {
                    if (hit.collider.gameObject == target)
                    {
                        targetObj = target.gameObject;
                        moveSW = false;
                        searchSW = false;
                    }
                }
            }

            //Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 1.0f);//飛ばしたRayの軌道を見る。
        }
    }
}
