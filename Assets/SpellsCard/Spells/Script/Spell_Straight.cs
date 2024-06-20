using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Straight : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;
    string ownerTag;

    public LineRenderer tr;

    public ParticleSystem shotEffect;//�ˌ����̃G�t�F�N�g
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g

    int damage;

    bool shotSW;//�������ォ�B

    //
    public AudioClip shotSE;
    public AudioClip hitSE;

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        tr = GetComponent<LineRenderer>();

        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//���L�҂ƂȂ�I�u�W�F�N�g���i�[����B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = spm.ownerObject.tag;//���L�҂̃^�O���i�[����B

        //ownerObj.GetComponent<StatusManager>().St_Inflict_NoMove(0.2f);//���L�҂Ɉړ��s��t�^����B

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
                
                tr.SetPosition(0, gameObject.transform.position);
                tr.SetPosition(1, hit.point);
                tr.enabled = true;

                if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != ownerTag)
                {
                    AudioSource hitAS = hit.transform.GetComponent<AudioSource>();
                    hitAS.PlayOneShot(hitSE);

                    Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//���������G�t�F�N�g
                    hit.transform.GetComponent<StatusManager>().HP_Inflict_Damage(damage);//40�_���[�W�𔭐�������B
                }
                else
                {
                    Instantiate(hitEffect, hit.point, Quaternion.identity);//�G�t�F�N�g�𐶐��B
                }    
                shotSW = true;

                //string name = hit.collider.gameObject.name;//�q�b�g�����I�u�W�F�N�g�̖��O���i�[�B
                //Debug.Log(name);//�q�b�g�����I�u�W�F�N�g�̖��O�����O�ɏo���B
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
