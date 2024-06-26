using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Slash : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�Ώۂ� �q�b�g�������� �`�ʂ����G�t�F�N�g�ł��B")]
    public GameObject hitEffect;// �Ώۂւ̃q�b�g�G�t�F�N�g
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�g�p���� �Đ������T�E���h�ł��B")]
    public AudioClip shotSE;// �������ɔ������鉹�B

    [Tooltip("�W�I�� �q�b�g�����ۂ� �Đ������T�E���h�ł��B")]
    public AudioClip hitSE;// ���������ۂ̉��B
    #endregion

    ////--------------------------------------------------
    
    SpellPrefabManager spm;

    GameObject ownerObj;// ���L�҃I�u�W�F�N�g
    string ownerTag;// ���L�҂̃^�O
    AudioSource ownerAS;// ���L�҃I�u�W�F�N�g��AudioSource

    int damage;// �Ώۂɗ^����_���[�W


    float aliveTime;// �I�u�W�F�N�g�̐������ԁB
    bool oneHit;// ��x�����������̔���B

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject; //���L�҂ƂȂ�I�u�W�F�N�g���i�[����B
        ownerTag = spm.ownerObject.tag;
        gameObject.tag = ownerTag;
        ownerAS = ownerObj.GetComponent<AudioSource>();
        //gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B�Օ��ɓ������ۂɐ�p�F�ŕ`�ʂ����

        ownerAS.PlayOneShot(shotSE);

        aliveTime = 0.15f;
    }

    
    void Update()
    {
        aliveTime -= 1 * Time.deltaTime;

        if (aliveTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneHit == false)//��x�q�b�g���Ă���ꍇ����������B
        {
            if(other.tag != ownerTag && other.tag != "Search") //���L�҈ȊO�Ƀq�b�g�B
            {
                if (other.transform.GetComponent<StatusManager>() != null)
                {
                    AudioSource hitAS = other.GetComponent<AudioSource>();//��������������I�[�f�B�I�\�[�X���擾�B
                    hitAS.PlayOneShot(hitSE);//�Đ��B

                    Instantiate(hitEffect, other.transform.position, Quaternion.identity);

                    var otherSM = other.GetComponent<StatusManager>();//��x�擾���A�i�[����B
                    otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                    otherSM.St_Inflict_Shock(0.2f, 1);//�V���b�N��ԃ��x��1��0.2�b�^����B
                    //otherSM.St_Inflict_Invincible(0.4f);//0.8�b�̖��G���Ԃ�t�^����B

                    if (other.GetComponent<Rigidbody>() != null)//���肪�d�͂ɉe�����󂯂�ꍇ�A���̕����֗͂�������B
                    {
                        other.GetComponent<Rigidbody>().AddForce(transform.forward*50, ForceMode.Impulse);//����𐁂���΂��B
                    }
                }
                oneHit = true;
            }
        }
    }
}
