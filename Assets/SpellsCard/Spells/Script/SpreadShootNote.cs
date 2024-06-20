using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShootNote : MonoBehaviour
{
    public GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O

    public GameObject modelObject;//�q�t������Ă��郂�f���I�u�W�F�N�g�B

    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g�B
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g

    public int damage;//�Ώۂɗ^����_���[�W�B���ˑ�����_���[�W�������Ă��炤�B
    float speed;//���̃I�u�W�F�N�g�̈ړ��X�s�[�h

    Rigidbody rb;

    AudioSource audioSource;
    public AudioClip shotSE;
    public AudioClip hitSE;
    void Start()
    {
        if (ownerObj == null) return;
        if (modelObject != null) modelObject.GetComponent<LayerConverter>().parentDate = gameObject;
        rb = GetComponent<Rigidbody>();
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;
        speed = 10;//�X�s�[�h������B

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(shotSE);
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
                AudioSource otherAS = other.GetComponent<AudioSource>();
                otherAS.PlayOneShot(hitSE);

                other.transform.GetComponent<StatusManager>().HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B

                Instantiate(targetHitEffect, other.transform.position, Quaternion.identity);
            }
            else Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
