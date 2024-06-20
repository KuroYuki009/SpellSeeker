using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBullet : MonoBehaviour
{
    Rigidbody rb;

    public GameObject ownerObj;//���L�ҁB
    public string ownerTag;

    float speed;//�e�̑��x�B
    public int damage;//�З́B

    //float elapsedTime;//�o�ߎ��ԁB

    //�G�t�F�N�g�ށB
    public ParticleSystem hitEffect;
    public ParticleSystem targetHitEffect;

    //���ʉ��ށB
    public AudioClip hitSE;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        speed = 24.0f;//�X�s�[�h������B


        Destroy(gameObject, 5);
    }

    private void FixedUpdate()
    {
        rb.velocity = (transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != ownerTag && other.tag != "Search")
        {
            if (other.transform.GetComponent<StatusManager>() != null)
            {
                var otherSM = other.GetComponent<StatusManager>();
                otherSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                //otherSM.St_Inflict_Invincible(0.4f);//0.2�b�̖��G���Ԃ�t�^����B

                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect,transform.position, Quaternion.identity);
            }
            //else Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
