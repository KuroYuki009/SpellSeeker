using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShootNote : MonoBehaviour
{
    [Header("���f�� �I�u�W�F�N�g")]
    [Tooltip("���f���̐���Ɏg�p����܂��B")]
    public GameObject modelObject;//�q�t������Ă��郂�f���I�u�W�F�N�g�B


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�W�I�� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g�B

    [Tooltip("��������� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("���ˎ��� �Đ������T�E���h�ł��B")]
    public AudioClip shotSE;
    [Tooltip("��������� �q�b�g�������� �Đ������T�E���h�ł��B")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------
    
    [HideInInspector] public GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    Rigidbody rb;
    AudioSource audioSource;


    [HideInInspector]public int damage;// ���̃I�u�W�F�N�g�̃_���[�W�l�B
    float speed;// ���̃I�u�W�F�N�g�̈ړ��X�s�[�h�l�B

    ////--------------------------------------------------

    void Start()
    {
        if (ownerObj == null) return;
        if (modelObject != null) modelObject.GetComponent<LayerConverter>().parentDate = gameObject;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        speed = 10;//�X�s�[�h������B

        audioSource.PlayOneShot(shotSE);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;//�O�i������B
    }

    private void OnTriggerEnter(Collider other)
    {
        string hitObjectTag = other.tag;

        if (hitObjectTag != ownerTag && hitObjectTag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                other.transform.GetComponent<StatusManager>().HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B

                Instantiate(targetHitEffect, other.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else if (hitObjectTag == "Player_1" || hitObjectTag == "Player_2" || hitObjectTag == "Player_3" || hitObjectTag == "Player_4")// �v���C���[���ʂ̃^�O�������ꍇ
            {
                //�����N�����܂���B
            }
            else
            {
                Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
