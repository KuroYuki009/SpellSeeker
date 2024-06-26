using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Straight : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�ˌ����� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem shotEffect;// �ˌ����̃G�t�F�N�g

    [Tooltip("�Ώۂ� �q�b�g�����ۂ� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem targetHitEffect;// �Ώۂւ̃q�b�g�G�t�F�N�g

    [Tooltip("��������� �q�b�g�����ۂ� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem hitEffect;// ���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�ˌ����� �Đ�������T�E���h�ł��B")]
    public AudioClip shotSE;// �ˌ����̌��ʉ��B

    [Tooltip("��������Ƀq�b�g�������� �Đ������T�E���h�ł��B")]
    public AudioClip hitSE;// �q�b�g���̌��ʉ��B
    #endregion

    ////--------------------------------------------------
    
    SpellPrefabManager spm;

    LineRenderer tr;

    GameObject ownerObj;//���L�҃I�u�W�F�N�g�B
    string ownerTag;//���L�҂̃^�O

    int damage;//�Ώۂɗ^����_���[�W�l �ϐ��B

    bool shotSW;//�ˌ��������̓�ɒl�B

    ////--------------------------------------------------
    
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        tr = GetComponent<LineRenderer>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//���L�҂ƂȂ�I�u�W�F�N�g���i�[����B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = spm.ownerObject.tag;//���L�҂̃^�O���i�[����B

        Instantiate(shotEffect, transform.position,transform.rotation);

        AudioSource ownerAS = ownerObj.GetComponent<AudioSource>();
        ownerAS.PlayOneShot(shotSE);

        tr.startWidth = 0.4f;
        tr.endWidth = 0.4f;

    }

    private void Update()
    {
        shoting();
    }

    void shoting()
    {
        if (shotSW == false)
        {
            Vector3 ori = gameObject.transform.position;
            Ray ray = new Ray(ori, transform.forward);


            int outMask = ~LayerMask.GetMask(new string[] { "Search",ownerTag });//���L�҃^�O���Ɠ������C���[�����O�B

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 30f,outMask))
            {
                string hitObjectTag = hit.transform.tag;//�q�b�g�Ώۂ̃^�O�����o�B
                StatusManager hitObjectStatusM = null;

                if (hit.transform.GetComponent<StatusManager>() != null) hitObjectStatusM = hit.transform.GetComponent<StatusManager>();// StatusManager SC�������Ă����炻�̂܂܃L���b�V���B

                tr.SetPosition(0, gameObject.transform.position);
                tr.SetPosition(1, hit.point);
                tr.enabled = true;

                if (hitObjectStatusM != null && hitObjectTag != ownerTag)
                {
                    AudioSource hitAS = hit.transform.GetComponent<AudioSource>();
                    hitAS.PlayOneShot(hitSE);

                    Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//���������G�t�F�N�g
                    hitObjectStatusM.HP_Inflict_Damage(damage);//40�_���[�W�𔭐�������B
                }
                else Instantiate(hitEffect, hit.point, Quaternion.identity);//�G�t�F�N�g�𐶐��B

                shotSW = true;

            }
        }
        else if(shotSW == true)
        {
            tr.startWidth -= 0.05f;
            tr.endWidth -= 0.05f;
            if(tr.startWidth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
