using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spell_ShiftDash : MonoBehaviour
{
    SpellPrefabManager spm;
    GameObject ownerObj;//���L�ҁB
    //string ownerTag;//���L�҂̃^�O

    StatusManager sm;
    PlayerMoving pm;

    public Rigidbody rb;

    float dashPower;
    float moveInputDeadZone;
    //�i�[�p
    bool actionSW;
    float actionTime;//�o�ߎ��Ԃ�ۊǂ���ׂ̓��ꕨ�B

    float moveMag;
    float maxSpeed;
    float movingSpeed;

    Vector3 forwardVc3;
    Vector3 depositForwardVc3;

    bool noMoveStsBool;
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();//�擾�B

        ownerObj = spm.ownerObject;//���L�҂̃^�O�����̃I�u�W�F�N�g�ɓn���B
        rb = ownerObj.GetComponent<Rigidbody>();//���L�҂�rb���擾�B
        sm = ownerObj.GetComponent<StatusManager>();//���L�҂�sm���擾�B
        pm = ownerObj.GetComponent<PlayerMoving>();

        forwardVc3 = pm.move_forwardPointVc3;

        moveMag = sm.moveMag;
        maxSpeed = sm.maxSpeed;
        movingSpeed = sm.moveSpeed;

        moveInputDeadZone = 0.30f;

        noMoveStsBool = sm.noMoveSt;

        dashPower = 35.0f;

        //�������J�n�B
        DashAction();
    }

    
    void DashAction()
    {
        depositForwardVc3 = forwardVc3;//���͒l���ꎞ�i�[�B
        actionTime = 0.08f;

        //audioSource.PlayOneShot(shiftDashSE);//���ʉ���炷�B

        actionSW = true;
    }

    void Update()
    {
        if (noMoveStsBool == true)
        {
            Destroy(gameObject);
        }

        if (actionTime >= 0)//Action�^�C���������B
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
        if ((forwardVc3.x >= moveInputDeadZone || forwardVc3.x <= -moveInputDeadZone) || (forwardVc3.z >= moveInputDeadZone || forwardVc3.z <= -moveInputDeadZone))//���̓��͂��s���Ă��Ȃ��ꍇ�A�������Ȃ��B
        {
            if (actionSW == true)//sdAcSW��false�ł���Ώ������s���B
            {
                ////�L�����ړ������B
                if (rb.velocity.magnitude < maxSpeed + dashPower)//�X�s�[�h�ɐ�����������B
                {
                    // �ړ������ɃX�s�[�h(moveSpeed��scroll�l)���|����B
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
