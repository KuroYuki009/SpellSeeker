using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_SpreadShoot : MonoBehaviour
{
    [Header("�X�y���f�[�^")]//---
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B


    [Header("���˂���I�u�W�F�N�g")]//---
    #region
    [Tooltip("���˂���I�u�W�F�N�g��ݒ�o���܂��B")]
    public GameObject shootObject;//���˂����I�u�W�F�N�g��ݒ�B

    [Tooltip("���˂���ׂ̎n�_��ݒ�ł��܂��B")]
    public List<Transform> instPos;//�ˏo�Ɏg�p����ʒu���`�B
    #endregion


    [Header("�G�t�F�N�g")]//---
    #region
    [Tooltip("�ˌ����� �`�ʂ����G�t�F�N�g�ł��B")]
    public GameObject shotEffect;//�ˌ����̃G�t�F�N�g�B

    [Tooltip("������������ �`�ʂ����G�t�F�N�g�ł��B")]
    public GameObject standbyEffect;//�����������̃G�t�F�N�g�B�ˏo�O�ɔ���������B
    #endregion


    [Header("�T�E���h")]//---
    #region
    [Tooltip("�`���[�W���� �Đ������T�E���h�ł��B")]
    public AudioClip chargeSE;
    #endregion

    ////--------------------------------------------------

    SpellPrefabManager spm;//���L�҂���f�[�^�����p����ׂɁB

    AudioSource audioSource;

    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    int damage;//�Ώۂɗ^����_���[�W


    string swirchRoute;//�����𕪂���ׂɎg�p���Ă���Switch���𐧌䂷��ׁB
    float elapsedTime;//�o�ߎ��Ԃ�ۊǂ���ׂ̓��ꕨ�B

    int shotCount = 0;//���񔭎˂��������J�E���g����B

    GameObject C_Effect;//���������G�t�F�N�g���ꎞ�I�Ɋi�[�B

    ////--------------------------------------------------

    void Start()
    {
        damage = spellDate.primaryDamage;

        spm = GetComponent<SpellPrefabManager>();
        ownerObj = spm.ownerObject;//���L�҂̃^�O�����̃I�u�W�F�N�g�ɓn���B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = chargeSE;
        audioSource.Play();

        C_Effect = Instantiate(shotEffect, gameObject.transform.position, Quaternion.identity);//�`���[�W�G�t�F�N�g�𐶐��B

        swirchRoute = "Charge";
    }

    
    void Update()
    {
        switch(swirchRoute)
        {
            case "Charge":
                elapsedTime += 1 * Time.deltaTime;
                if (elapsedTime >= 0.4f)
                {
                    Destroy(C_Effect);
                    swirchRoute = "SpShoot";
                }
                break;
            case "SpShoot":
                SpShoot();
                if (shotCount == 5 || shotCount >= 5) Destroy(gameObject);//�܉񐶐������炱�̃I�u�W�F�N�g��j��B
                break;
        }
    }

    void SpShoot()
    {
        Instantiate(standbyEffect, instPos[shotCount].transform.position, Quaternion.identity);//�ˏo�ꏊ�ɃG�t�F�N�g�����B

        GameObject shotNote = Instantiate(shootObject, instPos[shotCount].transform.position, instPos[shotCount].transform.rotation);//�Ԏ��ꏊ����e�𔭎ˁB
        shotNote.GetComponent<SpreadShootNote>().damage = damage;//���˕��ɕt���Ă���Script�����̕ϐ��udamage�v�ɂ�����̃_���[�W������B
        shotNote.GetComponent<SpreadShootNote>().ownerObj = ownerObj;//���˕��ɕt���Ă���Script�����̕ϐ��uownerObj�v�ɂ�����̏��L�ҏ�������
        shotCount++;
    }
}
