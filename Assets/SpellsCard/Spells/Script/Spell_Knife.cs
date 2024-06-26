using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Knife : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�ˌ����� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem shotEffect;//�ˌ����̃G�t�F�N�g�B

    [Tooltip("�W�I�� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g�B

    [Tooltip("��������� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�ˌ����� �Đ������T�E���h�ł��B")]
    public AudioClip shotSE;

    [Tooltip("��������� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    Rigidbody rb;

    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    int damage;// ���̃I�u�W�F�N�g�̃_���[�W�l
    float speed;// ���̃I�u�W�F�N�g�̈ړ��X�s�[�h

    ////--------------------------------------------------
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spm = GetComponent<SpellPrefabManager>();
        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//���L�҂̃^�O�����̃I�u�W�F�N�g�ɓn���B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        AudioSource ownerAS = ownerObj.GetComponent<AudioSource>();
        ownerAS.PlayOneShot(shotSE);

        speed = 22.0f;// �I�u�W�F�N�g�̈ړ��X�s�[�h�l�� 22 �ɐݒ�B

        Instantiate(shotEffect, transform.position, Quaternion.identity);// �ˌ��G�t�F�N�g�𐶐��B
    }

    void FixedUpdate()
    {
        rb.velocity = (transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        string hitObjectTag = other.tag;

        if (hitObjectTag != ownerTag && hitObjectTag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                otherSM.St_Inflict_Shock(0.2f, 1);//�V���b�N��ԃ��x��1��0.2�b�^����B
                otherSM.St_Inflict_Vulnerable(4.0f);//�Ǝ��Ԃ�4�b�^����B
                //otherSM.St_Inflict_Invincible(0.4f);//0.2�b�̖��G���Ԃ�t�^����B

                AudioSource otherAS = other.transform.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

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
