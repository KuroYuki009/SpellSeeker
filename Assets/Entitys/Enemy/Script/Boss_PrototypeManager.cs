using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_PrototypeManager : MonoBehaviour
{
    ////���̃{�X�͐�p�̃X�e�[�W��ł����g�p�ł��܂���B

    StatusManager statusManager;
    InGameManager inGameManager;

    Rigidbody rb;
    Collider cd;

    //�O��
    PlayableDateManager playableDateManager;
    //

    float maxSpeed;
    float moveSpeed;
    float moveMag = 1.0f;//�ړ����x�{���B

    public Vector3 moveVc3;

    string actionRoute;//�s�����򏈗��Ɏg�p�����B

    ////�A�N�V�����g�p�v�f��

    //���Ԍv��-------
    float processingTime;//�o�ߎ��ԑ���p�̕ϐ�
    float goalProcessTime;//�o�ߎ��Ԃ̖ڕW�l
    string nextProcessString;//���̏�������

    //MissilePack_TopAttack-------
    GameObject topAttackMissile_Prefab;//�U���Ɏg�p�����Prefab
    int shootMagSize;
    List<Vector3> mpLookonPlayesPos;
    

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        rb = GetComponent<Rigidbody>();
        cd = GetComponent<Collider>();

        maxSpeed = 28;
        moveSpeed = 28;

        //�v���C�A�u���f�[�^�}�l�[�W���[���擾���܂��B
        GameObject gmObj = GameObject.FindWithTag("GameManager");//Tag"GameManager"���t�����I�u�W�F�N�g��T���B
        playableDateManager = gmObj.GetComponent<PlayableDateManager>();//�Q�Ƃ����I�u�W�F�N�g����Script�����o���B
        inGameManager = gmObj.GetComponent<InGameManager>();
    }

    void Update()
    {
        switch(actionRoute)
        {
            case "MovingAction" :
                Debug.Log("�ړ����s���܂��B");
                break;
            case "Attack_VulcanStrafing":
                Debug.Log("�@�֏e�ɂ��|�ˍU�����s���܂��B(���b�N�I�����s����莞�Ԍ�ɑ|�ˊJ�n����B�������ǔ����Ȃ���ǂ�������B)");
                break;

            //�~�T�C���|�b�h�ɂ��U�����s���܂��B(�W���x�����~�T�C���̂悤�ɏ�Ɉ�x�e��ł��グ�A�ڕW�Ɍ������Ē��p�ɗ����Ă����B)
            case "SetUp_MissilePack_TopAttack"://�ŏ��ɋN���B
                SetUp_MissilePack_TopAttack();
                break;
            case "Attack_MissilePack_TopAttack"://���C�������B
                Attack_MissilePack_TopAttack();
                break;
            case "End_MissilePack_TopAttack"://�I�������B
                End_MissilePack_TopAttack();
                break;
            //-------------

            case "Attack_GroundCrackWave":
                Debug.Log("�O�����h�N���b�N�ɂ��g��U�����s���B(�@�̑O���ɃO�����h�N���b�N�𔭐�������B)");
                break;
            case "Attack_JunkFall":
                Debug.Log("�����ォ�痎��������B(�X�e�[�W�^�㕔�����ˌ����A�x���J�[�\���Ƌ��ɕ��������Ă���B)");
                break;
            case "Attack_Artemis_ThirdFixation":
                Debug.Log("3�̃A���e�~�X���g�p���U������B(�A���e�~�X������v���C���[�̈ʒu�ɔ�΂��Œ�B3�Œ肳���ƍŏ��ɌŒ肳�ꂽ�����珇�ԂɃ��b�N�I��������s���A���[�U�[�𔭎˂�����B)");
                break;
            case "Attack_RailCannon_OneShot":
                Debug.Log("������Ƒ�_���[�W��^���s���s�\����莞�ԕt�^����f�����ԋ����U�����s���B(���b�N�I����ɔ��ˁB���˂���O��ArcBarrel��I�o����B)");
                break;
            case "Attack_RailCannon_ShootingArcBlast":
                Debug.Log("���i�_���[�W�ɉ������З́A�L�͈͂��U������r�[�����ˏo����B(�U���O�ɂ�ArcBarrel��I�o����`���[�W������s���B)");
                break;
            case "Attack_RailCannon_RotationArcBlast":
                Debug.Log("�L�͈͂��U������r�[�����ˏo���Ȃ����]���s��(�K���X�e�[�W�����Ɉړ����s���B�U���O�ɂ�ArcBarrel��I�o����`���[�W������s���B)");
                break;


            case "TimeProcess":
                TimeProcess();//�o�ߏ���
                break;
        }
    }

    void TimeProcess()
    {
        processingTime += 1 * Time.deltaTime;

        if (processingTime >= goalProcessTime)//�ڕW���Ԃ𖞂������ꍇ�ɏ�����ʂ��B
        {
            actionRoute = nextProcessString;
        }
    }

    //MissilePack_TopAttack------------ //�~�T�C���|�b�h�ɂ��U�����s���܂��B(�W���x�����~�T�C���̂悤�ɏ�Ɉ�x�e��ł��グ�A�ڕW�Ɍ������Ē��p�ɗ����Ă����B)

    void SetUp_MissilePack_TopAttack()//�ŏ��ɋN�����鏈���B
    {
        shootMagSize = 8;//�e���̕�[�B
        //�A�j���[�V�����̍Đ��B



        mpLookonPlayesPos.Clear();//�������B
        for (int i = 0; i < playableDateManager.joinPlayerInt; i++)//�������Ă���v���C���[�̍��W�����ׂĎ擾�B
        {
            mpLookonPlayesPos.Add(inGameManager.beingPlayerObj[i].transform.position);//���W������B
        }

        //���Ԃ��o�߂�����B
        nextProcessString = "TimeProcess";//���Ԍo�ߌ�ɏ���������e��ݒ�B
        processingTime = 0f;//���Ԃ̏������B
        goalProcessTime = 1.4f;//�ڕW���Ԃ̐ݒ�B
        actionRoute = "TimeProcess";//�������ڍs����B
    }

    void Attack_MissilePack_TopAttack()//���s���鏈���B
    {
        for (int pli = 0; pli < mpLookonPlayesPos.Count;pli++)
        {
            Vector3 instPos = new Vector3(mpLookonPlayesPos[pli].x, 50.0f, mpLookonPlayesPos[pli].z);//�e�v���C���[�̍��W���擾�B
            GameObject tam = Instantiate(topAttackMissile_Prefab, instPos, Quaternion.Euler(0, 90, 0));//�^���Ɍ����Ĕ��ł���
            shootMagSize--;
            if (shootMagSize <= 0)//�e�����c��[���ɂȂ����ꍇ�B
            {
                actionRoute = "End_MissilePack_TopAttack";//�������ڍs����B
            }
        } 
    }

    void End_MissilePack_TopAttack()//�Ō�ɋN�����鏈���B
    {
        //�A�j���[�V�������̉���


        actionRoute = default;//������ҋ@��ԂɈڍs����B
    }
    //---------------------

    void FixedUpdate()
    {
        //��������
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
