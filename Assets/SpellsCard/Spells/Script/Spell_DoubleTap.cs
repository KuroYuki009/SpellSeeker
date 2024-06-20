using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_DoubleTap : MonoBehaviour
{
    public SpellData spellDate;

    SpellPrefabManager spm;
    GameObject ownerObj;
    string ownerTag;

    public LineRenderer lr;

    public ParticleSystem shotEffect;//�ˌ����̃G�t�F�N�g
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g

    int damage;

    int tapCount;//��x�ɉ���ˌ����s�����B
    //int magCount;//�c�艽��ˌ����s�����B
    bool shotTrigger;//�ˌ��ς݂��ǂ��������o����B

    GameObject instPos;//owner�I�u�W�F�N�g�������Ă���instPos���擾�A�i�[����B

    float elapsedTime;//�o�ߎ���
    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        lr = GetComponent<LineRenderer>();
        
        damage = spellDate.primaryDamage;
        ownerObj = spm.ownerObject;//���L�҂ƂȂ�I�u�W�F�N�g���i�[����B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = spm.ownerObject.tag;//���L�҂̃^�O���i�[����B
        instPos = ownerObj.GetComponent<HandCardManager>().instPos;

        LineView();

        tapCount = 2;
    }

    void Update()
    {
        if (tapCount != 0)
        {
            if (shotTrigger == false)
            {
                Debug.Log("�^�b�v�V���b�g�I�I");
                Shot();
            }
            else if (shotTrigger == true)//�g���K�[�������ꂽ��B�܂��^�b�v�J�E���g��0�ł͂Ȃ��ꍇ�B
            {
                elapsedTime += 1 * Time.deltaTime;
                if (elapsedTime >= 0.3f)
                {

                    elapsedTime = 0;
                    shotTrigger = false;
                }
                else if (elapsedTime >= 0.1f)
                {
                    //LineHide(); 
                }
            }
            else Destroy(gameObject, 0.1f);
        } 
    }

    void Shot()
    {
        
        Instantiate(shotEffect, instPos.transform.position, Quaternion.identity);

        Vector3 ori = instPos.transform.position;
        Ray ray = new Ray(ori, transform.forward);


        //int outMask = LayerMask.GetMask(new string[] { " "});//���C���[" "

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f))
        {
            

            lr.SetPosition(0, instPos.transform.position);
            lr.SetPosition(1, hit.point);
            

            if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != ownerTag)
            {
                Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//���������G�t�F�N�g
                StatusManager sm = hit.transform.GetComponent<StatusManager>();
                sm.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                if(sm.hitPoint <= 0 || sm.hitPoint == 0 || sm.gameObject == null)//���������Ώۂ̗̑͂�0�A�������͏��ōς݂̏ꍇ
                {
                    //�Ȃɂ����ʂ������Ăǂ���
                }

            }
            else
                Instantiate(hitEffect, hit.point, Quaternion.identity);//�G�t�F�N�g�𐶐��B
        }


        //Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 1.0f);//��΂���Ray�̋O��������B
         
        tapCount -= 1;
        shotTrigger = true;
    }

    void LineHide()
    {
        lr.startWidth = 0.0f;
        lr.endWidth = 0.0f;
    }
    void LineView()
    {
        lr.startWidth = 0.15f;
        lr.endWidth = 0.15f;
    }
}
