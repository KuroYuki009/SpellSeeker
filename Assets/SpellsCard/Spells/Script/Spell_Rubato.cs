using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Rubato : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    public ParticleSystem shotEffect;//�ˌ����̃G�t�F�N�g�B
    public ParticleSystem standbyEffect;//�����������̃G�t�F�N�g�B�������ɔ���������B
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g�B
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g

    Rigidbody rb;
    int damage;
    float speed;//���̃I�u�W�F�N�g�̈ړ��X�s�[�h
    bool highSpeedSW;

    float elapsedTime;//�o�ߎ��Ԃ�ۊǂ���ׂ̓��ꕨ�B

    AudioSource audioSource;
    public AudioClip shotSE;
    public AudioClip hitSE;
    // public AudioClip progressSE;

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
        if (other.tag != ownerTag && other.tag != "Search") 
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
            }
            else Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
