using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_PrototypeManager : MonoBehaviour
{
    ////このボスは専用のステージ上でしか使用できません。

    StatusManager statusManager;
    InGameManager inGameManager;

    Rigidbody rb;
    Collider cd;

    //外部
    PlayableDateManager playableDateManager;
    //

    float maxSpeed;
    float moveSpeed;
    float moveMag = 1.0f;//移動速度倍率。

    public Vector3 moveVc3;

    string actionRoute;//行動分岐処理に使用される。

    ////アクション使用要素類

    //時間計測-------
    float processingTime;//経過時間測定用の変数
    float goalProcessTime;//経過時間の目標値
    string nextProcessString;//次の処理分岐

    //MissilePack_TopAttack-------
    GameObject topAttackMissile_Prefab;//攻撃に使用されるPrefab
    int shootMagSize;
    List<Vector3> mpLookonPlayesPos;
    

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        rb = GetComponent<Rigidbody>();
        cd = GetComponent<Collider>();

        maxSpeed = 28;
        moveSpeed = 28;

        //プレイアブルデータマネージャーを取得します。
        GameObject gmObj = GameObject.FindWithTag("GameManager");//Tag"GameManager"が付いたオブジェクトを探す。
        playableDateManager = gmObj.GetComponent<PlayableDateManager>();//参照したオブジェクトからScriptを取り出す。
        inGameManager = gmObj.GetComponent<InGameManager>();
    }

    void Update()
    {
        switch(actionRoute)
        {
            case "MovingAction" :
                Debug.Log("移動を行います。");
                break;
            case "Attack_VulcanStrafing":
                Debug.Log("機関銃による掃射攻撃を行います。(ロックオンを行い一定時間後に掃射開始する。すこし追尾しながら追いかける。)");
                break;

            //ミサイルポッドによる攻撃を行います。(ジャベリンミサイルのように上に一度弾を打ち上げ、目標に向かって直角に落ちていく。)
            case "SetUp_MissilePack_TopAttack"://最初に起動。
                SetUp_MissilePack_TopAttack();
                break;
            case "Attack_MissilePack_TopAttack"://メイン処理。
                Attack_MissilePack_TopAttack();
                break;
            case "End_MissilePack_TopAttack"://終了処理。
                End_MissilePack_TopAttack();
                break;
            //-------------

            case "Attack_GroundCrackWave":
                Debug.Log("グランドクラックによる波状攻撃を行う。(機体前方にグランドクラックを発生させる。)");
                break;
            case "Attack_JunkFall":
                Debug.Log("物を上から落下させる。(ステージ真上部分を射撃し、警告カーソルと共に物が落ちてくる。)");
                break;
            case "Attack_Artemis_ThirdFixation":
                Debug.Log("3つのアルテミスを使用し攻撃する。(アルテミスを一つずつプレイヤーの位置に飛ばし固定。3つ固定されると最初に固定された物から順番にロックオン動作を行い、レーザーを発射させる。)");
                break;
            case "Attack_RailCannon_OneShot":
                Debug.Log("当たると大ダメージを与え行動不能を一定時間付与する素早い間距離攻撃を行う。(ロックオン後に発射。発射する前にArcBarrelを露出する。)");
                break;
            case "Attack_RailCannon_ShootingArcBlast":
                Debug.Log("多段ダメージに加え高威力、広範囲を攻撃するビームを射出する。(攻撃前にはArcBarrelを露出するチャージ動作を行う。)");
                break;
            case "Attack_RailCannon_RotationArcBlast":
                Debug.Log("広範囲を攻撃するビームを射出しながら回転を行う(必ずステージ中央に移動を行う。攻撃前にはArcBarrelを露出するチャージ動作を行う。)");
                break;


            case "TimeProcess":
                TimeProcess();//経過処理
                break;
        }
    }

    void TimeProcess()
    {
        processingTime += 1 * Time.deltaTime;

        if (processingTime >= goalProcessTime)//目標時間を満たした場合に処理を通す。
        {
            actionRoute = nextProcessString;
        }
    }

    //MissilePack_TopAttack------------ //ミサイルポッドによる攻撃を行います。(ジャベリンミサイルのように上に一度弾を打ち上げ、目標に向かって直角に落ちていく。)

    void SetUp_MissilePack_TopAttack()//最初に起動する処理。
    {
        shootMagSize = 8;//弾数の補充。
        //アニメーションの再生。



        mpLookonPlayesPos.Clear();//初期化。
        for (int i = 0; i < playableDateManager.joinPlayerInt; i++)//入室しているプレイヤーの座標をすべて取得。
        {
            mpLookonPlayesPos.Add(inGameManager.beingPlayerObj[i].transform.position);//座標を入れる。
        }

        //時間を経過させる。
        nextProcessString = "TimeProcess";//時間経過後に処理する内容を設定。
        processingTime = 0f;//時間の初期化。
        goalProcessTime = 1.4f;//目標時間の設定。
        actionRoute = "TimeProcess";//処理を移行する。
    }

    void Attack_MissilePack_TopAttack()//実行する処理。
    {
        for (int pli = 0; pli < mpLookonPlayesPos.Count;pli++)
        {
            Vector3 instPos = new Vector3(mpLookonPlayesPos[pli].x, 50.0f, mpLookonPlayesPos[pli].z);//各プレイヤーの座標を取得。
            GameObject tam = Instantiate(topAttackMissile_Prefab, instPos, Quaternion.Euler(0, 90, 0));//真下に向けて飛んでいく
            shootMagSize--;
            if (shootMagSize <= 0)//弾数が残りゼロになった場合。
            {
                actionRoute = "End_MissilePack_TopAttack";//処理を移行する。
            }
        } 
    }

    void End_MissilePack_TopAttack()//最後に起動する処理。
    {
        //アニメーション等の解除


        actionRoute = default;//処理を待機状態に移行する。
    }
    //---------------------

    void FixedUpdate()
    {
        //落下処理
        rb.AddForce((-transform.up * 25.0f) / Time.deltaTime, ForceMode.Force);

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity = moveVc3 * (moveSpeed * moveMag) + new Vector3(0, 0, 0);
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = moveVc3 * (moveSpeed * moveMag) + new Vector3(0, 0, 0);
        }
    }
}
