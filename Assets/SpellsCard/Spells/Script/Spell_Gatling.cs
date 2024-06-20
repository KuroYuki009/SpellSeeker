using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Gatling : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    public StatusManager ownerSM;

    string swirchRoute;//������ �p�B

    int magazineSizeInt;//���˂��鐔�B

    float intervalMax;
    float intervalTime;

    public GameObject instBulletObject;

    public List<Transform> instBarrelTF;
    int useBarrelInt;

    //�G�t�F�N�g��
    public ParticleSystem shotEffect;

    //���ʉ���
    AudioSource audioSource;
    public AudioClip shotSE;

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
        gameObject.transform.parent = ownerObj.GetComponent<HandCardManager>().instPos.transform;//HCM�̎ˌ�TF�̎q�ɐݒ肷��B

        ownerSM = ownerObj.GetComponent<StatusManager>();//�V���b�N��Ԃ��̃X�e�[�^�X���擾�B

        magazineSizeInt = 16;
        intervalMax = 0.08f;

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

    void FireInterval()
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

        Instantiate(shotEffect, transform.position, transform.rotation,ownerObj.transform);//�G�t�F�N�g����

        audioSource.PlayOneShot(shotSE);

        useBarrelInt++;
        magazineSizeInt--;

        if (magazineSizeInt <= 0)//�c�蔭�ˉ񐔂�0���ɂȂ��Ă���ꍇ�A
        {
            Destroy(gameObject, 2);//��b��ɖ{�̂�j��B
            swirchRoute = null;//�������~�B
        }
        else//�����łȂ���Ώ����𑱍s����B
        {
            intervalTime = 0;
            intervalTime += intervalMax;
            swirchRoute = "FireInterval";
        }
    }
}
