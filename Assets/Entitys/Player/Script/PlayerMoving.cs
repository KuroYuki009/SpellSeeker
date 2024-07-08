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

    Vector2 moveInput;//�ړ����͕ۊǗp
    Vector2 lookInput;//���_���͕ۊǗp

    public GameObject lockOnTargetObj;//���b�N�I�������I�u�W�F�N�g���i�[�B

    float maxSpeed;//�ő�ړ����x
    float moveSpeed;//�ړ����x
    float moveMag = 1.0f;//�ړ����x�{���B

    // ���b�N�I�����̑Ώۂւ̒Ǐ]�l�B
    float lockonLookFollowValue;

    //��Ԓl�BStatusManager����擾����K�v������B
    bool noMoveSts;//�s���s��Ԃ��B
    bool shockSts;//�V���b�N��Ԃ��B


    //�ǉ��v�f�B
    float sdAtDeadZone;//�Œ�v���̓��͂̋����B
    public float sdAtMaxCoolTime;//�Z�J���h�A�N�V�����̑���N�[���^�C��
    public float sdAtCoolTime;//�Z�J���h�A�N�V�����̃N�[���^�C���B
    public bool sdActionSW;
    public float sdActionTime;//���쎞�ԁB
    Vector3 depositForwardVc3;//�ꎞ�i�[�p�B

    //���ʉ�

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

        //inputSystem��ڑ�����B
        PlayerInput playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];

    }
    void Define()//�X�e�[�^�X�}�l�[�W���[����e�ϐ��̒�`���s���B
    {
        maxSpeed = statusManager.maxSpeed;
        moveSpeed = statusManager.moveSpeed;
        moveMag = statusManager.moveMag;
        noMoveSts = statusManager.noMoveSt;
        shockSts = statusManager.shockSt;
    }

    void Update()
    {
        Define();// �e�ϐ��̐ڑ����s���B

        MovingProcessing();// ���쎞�̏������s���B

        SdAcProcessing();// �Z�J���_���A�N�V�����̏������s���B
    }

    public void CameraSearch()
    {
        if (visCamera == null) visCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        //��������
        RB.AddForce((-transform.up * 25.0f) / Time.deltaTime, ForceMode.Force);

        if(shockSts == false)//�V���b�N��ԂŖ�����Ύ��s�����B
        {
            ////�ړ�����
            if (noMoveSts == false)//�ړ��s��ԂŖ�����Ύ��s�����B
            {
                if(sdActionSW == true)//sdAcSW��true�ł���Βʏ�̈ړ��������s���B
                {
                    ////�L�����ړ������B
                    if (RB.velocity.magnitude < maxSpeed)//�X�s�[�h�ɐ�����������B
                    {
                        // �ړ������ɃX�s�[�h(moveSpeed��scroll�l)���|����B
                        RB.velocity = move_forwardPointVc3 * (moveSpeed * moveMag) + new Vector3(0, 0, 0);

                    }
                    else if (RB.velocity.magnitude > maxSpeed)
                    {
                        RB.velocity = move_forwardPointVc3 * (moveSpeed * moveMag) + new Vector3(0, 0, 0);
                    }

                    //Debug.Log(RB.velocity.magnwitude);

                    // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾�B���ʂ�����o���܂��B
                    cameraForward = Vector3.Scale(visCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

                    // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
                    move_forwardPointVc3 = cameraForward * moveInput.y + visCamera.transform.right * moveInput.x;
                }
            }

            ////�L�������������B
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
            else if (lockOnTargetObj != null)// ���b�N�I�� �ߑ��Ώۂ�����ꍇ
            {
                Quaternion qton = Quaternion.LookRotation(lockOnTargetObj.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, qton, lockonLookFollowValue);
            }

            ////�ǉ��v�f�B
            if(noMoveSts == false)
            {
                if (sdActionSW == false)//sdAcSW��false�ł���Ώ������s���B
                {
                    ////�L�����ړ������B
                    if (RB.velocity.magnitude < maxSpeed + 35)//�X�s�[�h�ɐ�����������B
                    {
                        // �ړ������ɃX�s�[�h(moveSpeed��scroll�l)���|����B
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

    void SdAcProcessing()//�Z�J���_���A�N�V�����̏����B
    {
        if(sdAtCoolTime >= 0)//�N�[���^�C���������B
        {
            sdAtCoolTime -= 1 * Time.deltaTime;
        }

        if(sdActionTime >= 0)//Action�^�C���������B
        {
            sdActionTime -= 1 * Time.deltaTime;
        }
        else if(sdActionSW == false)
        {
            sdAtMaxCoolTime = 0.2f;//�e�X�g�ŃN�[���^�C�����`���܂��B
            sdAtCoolTime = sdAtMaxCoolTime;//�N�[���^�C����ݒ�B
            sdActionSW = true;
        }

    }

    void MovingProcessing()//�ړ����̏����B
    {
        Vector2 moveVC2 = move.ReadValue<Vector2>();
        moveInput = new Vector2(moveVC2.x, moveVC2.y);
        Vector2 loocVC2 = look.ReadValue<Vector2>();
        lookInput = new Vector2(loocVC2.x, loocVC2.y);
    }

    public void ShiftDash(InputAction.CallbackContext context)// �Z�J���h�A�N�V�����u�V�t�g�_�b�V���v
    {
        if (context.performed)
        {
            if(statusManager.shockSt == false)
            {
                if ((move_forwardPointVc3.x >= sdAtDeadZone || move_forwardPointVc3.x <= -sdAtDeadZone) || (move_forwardPointVc3.z >= sdAtDeadZone || move_forwardPointVc3.z <= -sdAtDeadZone))// ���̓��͂��s���Ă��Ȃ��ꍇ�A�������Ȃ��B
                {
                    if (sdActionSW == true && sdAtCoolTime <= 0)
                    {
                        if (statusManager.manaPoint >= 1)// �R�X�g���������Ă��邩�m�F�B
                        {
                            statusManager.Mana_Inflict_Expense(1);// �}�i��1�����B
                            depositForwardVc3 = move_forwardPointVc3;// ���͒l���ꎞ�i�[�B
                            sdActionTime = 0.08f;

                            audioSource.PlayOneShot(shiftDashSE);// ���ʉ���炷�B

                            sdActionSW = false;
                        }
                    }
                }
            }
        }
    }
}