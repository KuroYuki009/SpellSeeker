using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Gatling : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B

    
    [Header("���˂���I�u�W�F�N�g")]//---
    #region
    [Tooltip("�}�K�W���T�C�Y�B���̃X�y�����e�𔭎˂ł��鐔")]
    public int magazineSizeInt;//���˂���e���B�܂��͎c�e��

    [Tooltip("���˂���I�u�W�F�N�g��ݒ�o���܂��B")]
    public GameObject instBulletObject;//���˂���e�ƂȂ�v���n�u�I�u�W�F�N�g�B

    [Tooltip("���˂���ׂ̎n�_��ݒ�ł��܂��B")]
    public List<Transform> instBarrelTF;// �e�̔��˂Ɏg�p����n�_���I�u�W�F�N�g�̍��W���痘�p����B
    #endregion


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�ˌ����� �`�ʂ����G�t�F�N�g�ł��B")]
    public ParticleSystem shotEffect;// �ˌ����̃G�t�F�N�g�B
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�ˌ����� �Đ������T�E���h�ł��B")]
    public AudioClip shotSE;// �ˌ��T�E���h�B
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;

    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O
    [HideInInspector] public StatusManager ownerSM;//���L�҂̃X�e�[�^�X�}�l�[�W���[���L���b�V���B����͔��˂����e�ɓn�����B
    GameObject hcmInstPos;// HandCardManager�̃X�y���n�_�����B

    AudioSource audioSource;


    string swirchRoute;// switch���Ɏg�p�B

    int useBarrelInt;// ���˂̎n�_��i�K�I�ɐ؂�ւ���ۂɎg�p���� �J�E���g�l�B

    float intervalMax;
    float intervalTime;

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        audioSource = GetComponent<AudioSource>();
        
        ownerObj = spm.ownerObject;
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        gameObject.tag = ownerTag;
        gameObject.layer = ownerObj.layer;

        if(ownerObj.GetComponent<HandCardManager>() != null)//�n���h�J�[�h�}�l�[�W���[�������Ă���ꍇ�A
        {
            hcmInstPos = ownerObj.GetComponent<HandCardManager>().instPos;// �X�y���̔��ˎn�_���擾�B

            Transform sameObject = null;
            sameObject = hcmInstPos.transform.Find(this.gameObject.name);// ���݁A�������̏����^����X�y�������邩��T���B

            if (sameObject == null)// �����Ȃ������ꍇ�A
            {
                gameObject.transform.parent = hcmInstPos.transform;// �X�y�����ˎn�_�̎q�ɓ����B
            }

            else if(sameObject.name == gameObject.name) // ���ꖼ�̃X�y�����������ꍇ�ɂ�
            {
                sameObject.GetComponent<Spell_Gatling>().magazineSizeInt += 12;// �������̃}�V���K���̎c��c�e���Ƀv���X12���ǉ�����B
                Destroy(gameObject);// ������ "���̃X�y��" ��j������B
            }
        }

        ownerSM = ownerObj.GetComponent<StatusManager>();// ���L�҂̃X�e�[�^�X�}�l�[�W���[���擾�B

        intervalMax = 0.08f;// �ˌ��Ԃ̃C���^�[�o�����Ԃ�ݒ�B

        swirchRoute = "Fire";
    }


    void Update()
    {
        switch(swirchRoute)
        {
            case "FireInterval":
                FireInterval();
                break;

            case "Fire":
                Fire();
                break;
        }
        if (ownerSM.shockSt == true)//���L�҂��V���b�N��Ԃ������ꍇ�A
        {
            //���̍U�������S�ɖ��͉�����B
            Destroy(gameObject);
        }
    }

    void FireInterval()// �ꔭ�����Ƃɔ�������C���^�[�o������
    {
        intervalTime -= 1 * Time.deltaTime;
        if(intervalTime <= 0)
        {
            swirchRoute = "Fire";
        }
    }

    void Fire()
    {
        if(useBarrelInt >= instBarrelTF.Count)//Barrel�����g�p�ς�Barrel�̐����������ꍇ�A
        {
            useBarrelInt = 0;//�J�E���g�񐔂����Z�b�g����B
        }


        GameObject instbullet = Instantiate(instBulletObject, instBarrelTF[useBarrelInt].transform.position, instBarrelTF[useBarrelInt].transform.rotation);
        GatlingBullet gbLocal = instbullet.GetComponent<GatlingBullet>();
        instbullet.tag = ownerTag;
        instbullet.layer = ownerObj.layer;

        gbLocal.ownerObj = ownerObj;
        gbLocal.ownerTag = ownerTag;

        gbLocal.damage = spellDate.primaryDamage;

        audioSource.PlayOneShot(shotSE);// �ˌ� ���ʉ��Đ�

        Instantiate(shotEffect, transform.position, transform.rotation,ownerObj.transform);// �ˌ� �G�t�F�N�g����

        useBarrelInt++;// �ˌ��n�_ �J�E���g�l�𑝉�
        magazineSizeInt--;// �c�e������e�����ۂ�����B
        
        if (magazineSizeInt <= 0)//�c��c�e����0���ɂȂ��Ă���ꍇ�A
        {
            swirchRoute = null;//�������~�B
            Destroy(gameObject);//�j������B
        }
        else//�����łȂ���Ώ����𑱍s����B
        {
            intervalTime = 0;
            intervalTime += intervalMax;
            swirchRoute = "FireInterval";
        }
    }
}
