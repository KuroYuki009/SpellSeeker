using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtemisColliJudge : MonoBehaviour
{
    public SpellData spellDate;

    int damage;

    public GameObject meinObj;//�I�u�W�F�N�g�̖{��(�e)���A�^�b�`����B
    public string ownerTag;
    public int ownerLayer;

    //�G�t�F�N�g
    public ParticleSystem hitEffect;
    public ParticleSystem hitDamageEffect;
    //���ʉ�
    public AudioClip hitSE;
    private void Start()
    {
        ownerTag = meinObj.GetComponent<Spell_Artemis>().ownerObj.tag;
        ownerLayer = meinObj.GetComponent<Spell_Artemis>().ownerObj.layer;
        gameObject.tag = ownerTag;
        //gameObject.layer = ownerLayer;
        damage = spellDate.primaryDamage;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != ownerTag && other.tag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                otherSM.St_Inflict_Shock(0.2f, 1);//�V���b�N��ԃ��x��1��0.2�b�^����B
                //otherSM.St_Inflict_Invincible(0.4f);//0.2�b�̖��G���Ԃ�t�^����B

                AudioSource otherAS = other.transform.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(hitDamageEffect, other.transform.position, Quaternion.identity);
            }
            else Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(meinObj.gameObject);
        }
    }
}
