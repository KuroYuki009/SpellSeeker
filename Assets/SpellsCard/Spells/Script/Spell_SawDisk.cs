using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_SawDisk : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("���ˎ��� �`�ʂ����G�t�F�N�g�ł��B")]
    public GameObject shotEffect;//�ˌ����̃G�t�F�N�g

    [Tooltip("��������� �q�b�g�����ۂ� �`�ʂ����G�t�F�N�g�ł��B")]
    public GameObject hitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�g�p���� �Đ������T�E���h�ł��B")]
    public AudioClip shotSE;//�������ɔ������鉹�B

    [Tooltip("�W�I�� �q�b�g�������� �Đ������T�E���h�ł��B")]
    public AudioClip hitSE;//���������ۂ̉��B

    [Tooltip("��������� �q�b�g�����ۂ� �Đ������T�E���h�ł��B")]
    public AudioClip colliSE;//��������ɂԂ��������B
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    Rigidbody rb;
    AudioSource audioSource;

    GameObject ownerObj;// ���L�҃I�u�W�F�N�g
    string ownerTag;// ���L�҂̃^�O
    AudioSource ownerAS;// ���L�҃I�u�W�F�N�g��AudioSource

    int health;// ���̃I�u�W�F�N�g�̗̑͒l�B
    int damage;// ���̃I�u�W�F�N�g�̃_���[�W�l�B
    float speed;//���̃I�u�W�F�N�g�̈ړ��X�s�[�h�l�B


    float aliveTime;// �������ԁB

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        health = 200;
        damage = spellDate.primaryDamage;
        speed = 12.0f;

        ownerObj = spm.ownerObject; //���L�҂ƂȂ�I�u�W�F�N�g���i�[����B
        ownerTag = spm.ownerObject.tag;

        gameObject.tag = ownerTag;
        gameObject.layer = ownerObj.layer;

        ownerAS = ownerObj.GetComponent<AudioSource>();
        //gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B�Օ��ɓ������ۂɐ�p�F�ŕ`�ʂ����

        ownerAS.PlayOneShot(shotSE);

        rb.velocity = transform.forward * speed;//�O�i������B
    }


    void Update()
    {
        aliveTime += 1 * Time.deltaTime;

        if (aliveTime >= 7f || health <= 0)
        {
            Destroy(gameObject);
        }

        SpeedKeeper();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool seSW = false;
        if (collision.gameObject.tag != ownerTag && collision.gameObject.tag != "Search") //���L�҈ȊO�Ƀq�b�g�B
        {
            if (collision.transform.GetComponent<StatusManager>() != null)
            {
                AudioSource hitAS = collision.gameObject.GetComponent<AudioSource>();//��������������I�[�f�B�I�\�[�X���擾�B

                if (collision.gameObject.tag == "Installation" && seSW == false)
                {
                    hitAS.PlayOneShot(colliSE);//�Đ��B
                    seSW = true;
                }
                else if(seSW == false)
                {
                    hitAS.PlayOneShot(hitSE);//�Đ��B
                    seSW = true;
                }

                Instantiate(hitEffect, collision.transform.position, Quaternion.identity);

                var otherSM = collision.gameObject.GetComponent<StatusManager>();//��x�擾���A�i�[����B
                otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                //otherSM.St_Inflict_Shock(0.2f, 1);//�V���b�N��ԃ��x��1��0.2�b�^����B
                //otherSM.St_Inflict_Invincible(0.2f);//0.2�b�̖��G���Ԃ�t�^����B

                health -= 10;
            }
            health -= 10;
        }

        if (collision.gameObject.tag != "Ground" && seSW == false) 
        {
            audioSource.PlayOneShot(colliSE);
            health -= 10;
            aliveTime = 0;//�j��P�\���Ԃ����Z�b�g�B
            seSW = true;
        }
    }

    void SpeedKeeper()
    {
        Vector3 vt3 = rb.velocity;
        if (rb.velocity.magnitude != speed)
        {
            rb.velocity = vt3.normalized * speed;
        }
    }
}
