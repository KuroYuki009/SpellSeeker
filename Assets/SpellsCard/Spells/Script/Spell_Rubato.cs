using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Rubato : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B

    
    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("���ˎ��� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem shotEffect;// �ˌ����̃G�t�F�N�g�B

    [Tooltip("������������ �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem standbyEffect;// �����������̃G�t�F�N�g�B�������ɔ���������B

    [Tooltip("�W�I�� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem targetHitEffect;// �Ώۂւ̃q�b�g�G�t�F�N�g�B

    [Tooltip("��������� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem hitEffect;// �ΏۈȊO�̃I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("���ˎ��� �Đ������T�E���h�ł��B")]
    public AudioClip shotSE;

    [Tooltip("��������� �q�b�g�������� �Đ������T�E���h�ł��B")]
    public AudioClip hitSE;
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    Rigidbody rb;
    AudioSource audioSource;

    int damage;// ���̃I�u�W�F�N�g�̃_���[�W�l�B
    float speed;// ���̃I�u�W�F�N�g�̈ړ��X�s�[�h�l�B


    bool highSpeedSW;// �������x�Ɉڍs�������̓�ɒl�B

    float elapsedTime;//�o�ߎ��Ԃ�ۊǂ���ׂ̓��ꕨ�B

    ////--------------------------------------------------

    void Start()
    {
        speed = 10f;
        rb = GetComponent<Rigidbody>();
        spm = GetComponent<SpellPrefabManager>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//���L�҂̃^�O�����̃I�u�W�F�N�g�ɓn���B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(shotSE);

        Instantiate(shotEffect, gameObject.transform.position, Quaternion.identity);
    }

    
    void Update()
    {
        elapsedTime += 1 * Time.deltaTime;

        if (elapsedTime >= 10.0f)
            Destroy(gameObject);
        else if (elapsedTime >= 2.0f && highSpeedSW == false)
        {
            Instantiate(shotEffect, gameObject.transform.position, Quaternion.identity);
            audioSource.pitch = 1.6f;
            audioSource.PlayOneShot(shotSE);
            speed = 14.0f;
            highSpeedSW = true;
        }
        else if (elapsedTime >= 0.1f && elapsedTime <= 2f) speed = 0.8f;

    }

    void SpellCast()//�X�y���g�p�����m�B���������B�i....
    {

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
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                otherSM.St_Inflict_Shock(0.2f, 1);//�V���b�N��ԃ��x��1��0.2�b�^����B
                //otherSM.St_Inflict_Invincible(0.4f);//0.2�b�̖��G���Ԃ�t�^����B

                AudioSource otherAS = other.GetComponent<AudioSource>();
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
