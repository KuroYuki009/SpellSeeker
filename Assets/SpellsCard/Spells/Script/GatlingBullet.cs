using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBullet : MonoBehaviour
{
    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�W�I�� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem targetHitEffect;
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("��������� �q�b�g�������� �Đ������T�E���h�ł��B")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------
    
    [HideInInspector]public GameObject ownerObj;//���L�ҁB
    [HideInInspector]public string ownerTag;

    Rigidbody rb;


    [HideInInspector]public int damage;// ���̃I�u�W�F�N�g�̃_���[�W�l�B
    float speed;// ���̃I�u�W�F�N�g�̈ړ��X�s�[�h�B

    ////--------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        speed = 24.0f;//�X�s�[�h������B


        Destroy(gameObject, 5);
    }

    private void FixedUpdate()
    {
        rb.velocity = (transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        string hitObjectTag = other.tag;// ���������I�u�W�F�N�g�̃^�O���擾�B

        if (hitObjectTag != ownerTag && hitObjectTag != "Search")// ���L�҂̃^�O��Search�^�O�ȊO�̃I�u�W�F�N�g�������ꍇ�A
        {
            if (other.transform.GetComponent<StatusManager>() != null)// �Ώۂ�StatusManager�̃X�N���v�g�������Ă���ꍇ�A
            {
                var otherSM = other.GetComponent<StatusManager>();

                otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                //otherSM.St_Inflict_Invincible(0.4f);//0.2�b�̖��G���Ԃ�t�^����B

                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect, transform.position, Quaternion.identity);


                Destroy(gameObject);//���̃I�u�W�F�N�g��j������B
            }
            else if (hitObjectTag =="Player_1" || hitObjectTag == "Player_2" || hitObjectTag == "Player_3" || hitObjectTag == "Player_4")// �v���C���[���ʂ̃^�O�������ꍇ
            {
                //�����N�����܂���B
            }
            else Destroy(gameObject);//���̃I�u�W�F�N�g��j������B
        }
    }
}
