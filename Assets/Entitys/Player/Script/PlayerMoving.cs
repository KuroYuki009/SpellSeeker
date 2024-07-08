using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoving : MonoBehaviour
{
    public StatusManager statusManager;

    Rigidbody RB;

    public Camera visCamera;
    Vector3 cameraForward;
    public Vector3 move_forwardPointVc3;
    Vector3 look_forwardPointVc3;

    InputAction move, look;

    Vector2 moveInput;//移動入力保管用
    Vector2 lookInput;//視点入力保管用

    public GameObject lockOnTargetObj;//ロックオンしたオブジェクトを格納。

    float maxSpeed;//最大移動速度
    float moveSpeed;//移動速度
    float moveMag = 1.0f;//移動速度倍率。

    // ロックオン時の対象への追従値。
    float lockonLookFollowValue;

    //状態値。StatusManagerから取得する必要がある。
    bool noMoveSts;//行動不可状態か。
    bool shockSts;//ショック状態か。


    //追加要素。
    float sdAtDeadZone;//最低要求の入力の強さ。
    public float sdAtMaxCoolTime;//セカンドアクションの代入クールタイム
    public float sdAtCoolTime;//セカンドアクションのクールタイム。
    public bool sdActionSW;
    public float sdActionTime;//動作時間。
    Vector3 depositForwardVc3;//一時格納用。

    //効果音

    AudioSource audioSource;

    public AudioClip shiftDashSE;

    void Start()
    {
        CameraSearch();
        statusManager = GetComponent<StatusManager>();
        RB = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        sdAtDeadZone = 0.30f;
        lockonLookFollowValue = 0.32f;

        //inputSystemを接続する。
        PlayerInput playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];

    }
    void Define()//ステータスマネージャーから各変数の定義を行う。
    {
        maxSpeed = statusManager.maxSpeed;
        moveSpeed = statusManager.moveSpeed;
        moveMag = statusManager.moveMag;
        noMoveSts = statusManager.noMoveSt;
        shockSts = statusManager.shockSt;
    }

    void Update()
    {
        Define();// 各変数の接続を行う。

        MovingProcessing();// 操作時の処理を行う。

        SdAcProcessing();// セカンダリアクションの処理を行う。
    }

    public void CameraSearch()
    {
        if (visCamera == null) visCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        //落下処理
        RB.AddForce((-transform.up * 25.0f) / Time.deltaTime, ForceMode.Force);

        if(shockSts == false)//ショック状態で無ければ実行される。
        {
            ////移動処理
            if (noMoveSts == false)//移動不可状態で無ければ実行される。
            {
                if(sdActionSW == true)//sdAcSWがtrueであれば通常の移動処理を行う。
                {
                    ////キャラ移動処理。
                    if (RB.velocity.magnitude < maxSpeed)//スピードに制限をかける。
                    {
                        // 移動方向にスピード(moveSpeedとscroll値)を掛ける。
                        RB.velocity = move_forwardPointVc3 * (moveSpeed * moveMag) + new Vector3(0, 0, 0);

                    }
                    else if (RB.velocity.magnitude > maxSpeed)
                    {
                        RB.velocity = move_forwardPointVc3 * (moveSpeed * moveMag) + new Vector3(0, 0, 0);
                    }

                    //Debug.Log(RB.velocity.magnwitude);

                    // カメラの方向から、X-Z平面の単位ベクトルを取得。正面を割り出します。
                    cameraForward = Vector3.Scale(visCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

                    // 方向キーの入力値とカメラの向きから、移動方向を決定
                    move_forwardPointVc3 = cameraForward * moveInput.y + visCamera.transform.right * moveInput.x;
                }
            }

            ////キャラ向き処理。
            //
            if (lockOnTargetObj == null)
            {
                if (lookInput != Vector2.zero)
                {
                    look_forwardPointVc3 = cameraForward * lookInput.y + visCamera.transform.right * lookInput.x;
                    
                    Quaternion qt = Quaternion.LookRotation(look_forwardPointVc3);
                    transform.rotation = Quaternion.Lerp(transform.rotation, qt, 0.5f);
                }
                else if (move_forwardPointVc3 != Vector3.zero)
                {
                    Quaternion qt = Quaternion.LookRotation(move_forwardPointVc3);
                    transform.rotation = Quaternion.Lerp(transform.rotation, qt, 0.5f);
                }
            }
            else if (lockOnTargetObj != null)// ロックオン 捕捉対象がいる場合
            {
                Quaternion qton = Quaternion.LookRotation(lockOnTargetObj.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, qton, lockonLookFollowValue);
            }

            ////追加要素。
            if(noMoveSts == false)
            {
                if (sdActionSW == false)//sdAcSWがfalseであれば処理を行う。
                {
                    ////キャラ移動処理。
                    if (RB.velocity.magnitude < maxSpeed + 35)//スピードに制限をかける。
                    {
                        // 移動方向にスピード(moveSpeedとscroll値)を掛ける。
                        RB.velocity = depositForwardVc3 * ((35 + moveSpeed) * moveMag) + new Vector3(0, 0, 0);

                    }
                    else if (RB.velocity.magnitude > maxSpeed)
                    {
                        RB.velocity = depositForwardVc3 * ((35 + moveSpeed) * moveMag) + new Vector3(0, 0, 0);
                    }
                }
            }
        }

        
    }

    void SdAcProcessing()//セカンダリアクションの処理。
    {
        if(sdAtCoolTime >= 0)//クールタイムを処理。
        {
            sdAtCoolTime -= 1 * Time.deltaTime;
        }

        if(sdActionTime >= 0)//Actionタイムを処理。
        {
            sdActionTime -= 1 * Time.deltaTime;
        }
        else if(sdActionSW == false)
        {
            sdAtMaxCoolTime = 0.2f;//テストでクールタイムを定義します。
            sdAtCoolTime = sdAtMaxCoolTime;//クールタイムを設定。
            sdActionSW = true;
        }

    }

    void MovingProcessing()//移動時の処理。
    {
        Vector2 moveVC2 = move.ReadValue<Vector2>();
        moveInput = new Vector2(moveVC2.x, moveVC2.y);
        Vector2 loocVC2 = look.ReadValue<Vector2>();
        lookInput = new Vector2(loocVC2.x, loocVC2.y);
    }

    public void ShiftDash(InputAction.CallbackContext context)// セカンドアクション「シフトダッシュ」
    {
        if (context.performed)
        {
            if(statusManager.shockSt == false)
            {
                if ((move_forwardPointVc3.x >= sdAtDeadZone || move_forwardPointVc3.x <= -sdAtDeadZone) || (move_forwardPointVc3.z >= sdAtDeadZone || move_forwardPointVc3.z <= -sdAtDeadZone))// 一定の入力が行われていない場合、発動しない。
                {
                    if (sdActionSW == true && sdAtCoolTime <= 0)
                    {
                        if (statusManager.manaPoint >= 1)// コスト数があっているか確認。
                        {
                            statusManager.Mana_Inflict_Expense(1);// マナを1消費する。
                            depositForwardVc3 = move_forwardPointVc3;// 入力値を一時格納。
                            sdActionTime = 0.08f;

                            audioSource.PlayOneShot(shiftDashSE);// 効果音を鳴らす。

                            sdActionSW = false;
                        }
                    }
                }
            }
        }
    }
}