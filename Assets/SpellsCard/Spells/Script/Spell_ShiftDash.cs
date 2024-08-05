using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spell_ShiftDash : MonoBehaviour
{
    SpellPrefabManager spm;
    GameObject ownerObj;//所有者。
    //string ownerTag;//所有者のタグ

    StatusManager sm;
    PlayerMoving pm;

    public Rigidbody rb;

    float dashPower;
    float moveInputDeadZone;
    //格納用
    bool actionSW;
    float actionTime;//経過時間を保管する為の入れ物。

    float moveMag;
    float maxSpeed;
    float movingSpeed;

    Vector3 forwardVc3;
    Vector3 depositForwardVc3;

    bool noMoveStsBool;
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();//取得。

        ownerObj = spm.ownerObject;//所有者のタグをこのオブジェクトに渡す。
        rb = ownerObj.GetComponent<Rigidbody>();//所有者のrbを取得。
        sm = ownerObj.GetComponent<StatusManager>();//所有者のsmを取得。
        pm = ownerObj.GetComponent<PlayerMoving>();

        forwardVc3 = pm.move_forwardPointVc3;

        moveMag = sm.moveMag;
        maxSpeed = sm.maxSpeed;
        movingSpeed = sm.moveSpeed;

        moveInputDeadZone = 0.30f;

        noMoveStsBool = sm.noMoveSt;

        dashPower = 35.0f;

        //処理を開始。
        DashAction();
    }

    
    void DashAction()
    {
        depositForwardVc3 = forwardVc3;//入力値を一時格納。
        actionTime = 0.08f;

        //audioSource.PlayOneShot(shiftDashSE);//効果音を鳴らす。

        actionSW = true;
    }

    void Update()
    {
        if (noMoveStsBool == true)
        {
            Destroy(gameObject);
        }

        if (actionTime >= 0)//Actionタイムを処理。
        {
            actionTime -= 1 * Time.deltaTime;
        }
        else if (actionTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if ((forwardVc3.x >= moveInputDeadZone || forwardVc3.x <= -moveInputDeadZone) || (forwardVc3.z >= moveInputDeadZone || forwardVc3.z <= -moveInputDeadZone))//一定の入力が行われていない場合、発動しない。
        {
            if (actionSW == true)//sdAcSWがfalseであれば処理を行う。
            {
                ////キャラ移動処理。
                if (rb.velocity.magnitude < maxSpeed + dashPower)//スピードに制限をかける。
                {
                    // 移動方向にスピード(moveSpeedとscroll値)を掛ける。
                    rb.velocity = depositForwardVc3 * ((dashPower + movingSpeed) * moveMag) + new Vector3(0, 0, 0);

                }
                else if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = depositForwardVc3 * ((dashPower + movingSpeed) * moveMag) + new Vector3(0, 0, 0);
                }
            }
        }
    }
}
