using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_PushLater : MonoBehaviour
{
    public SpellData spellDate;//�A�^�b�`�����X�y���f�[�^�B

    Rigidbody rb;
    AudioSource audioSource;

    SpellPrefabManager spm;
    GameObject ownerObj;//���L�ҁB
    string ownerTag;//���L�҂̃^�O
    AudioSource ownerAS;
    public GameObject modelObject;//���̃X�y���̃��f���B

    ////�G�t�F�N�g
    public ParticleSystem shotEffect;//�ˌ����̃G�t�F�N�g�B
    public ParticleSystem muzzleFlashEffect;//�ˌ����}�Y���t���b�V���G�t�F�N�g�B
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g�B
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g
    public ParticleSystem windEffect;//�N�����̃G�t�F�N�g

    public ParticleSystem pushDirectionalEffect;
    //------------

    int damage;//���̃X�y���̃_���[�W�i�[�p�B

    public float speed;//���̃X�y���̑��x�i�[�p�B
    bool hitSW;//�������Ă��邩�ǂ����B
    Vector3 hitPos;//���������ʒu���i�[�B���C�����������ʒu���i�[���K�v�ȍۂɌĂяo���B

    string rootString;

    GameObject hitObj;
    StatusManager hitSM;
    

    float processTime;//��������Ă���̌o�ߎ���

    float stickProcessTime;//�������Ă���̌o�ߎ��Ԃ��i�[����ׂ̕ϐ��B
    float goalTime;

    //���ʉ�
    public AudioClip shotSE;//�������ɔ������鉹�B
    public AudioClip hitSE;//���������ۂ̉��B
    public AudioClip blastSE;//�������ɂȂ鉹�B
    AudioSource otherAS;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        spm = GetComponent<SpellPrefabManager>();
        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//���L�҂̃^�O�����̃I�u�W�F�N�g�ɓn���B
        gameObject.layer = ownerObj.layer;//���L�҂̃��C���[�����̃I�u�W�F�N�g�ɓn���B
        ownerTag = ownerObj.tag;
        gameObject.tag = ownerTag;
        ownerAS = ownerObj.GetComponent<AudioSource>();

        ownerAS.PlayOneShot(shotSE);

        speed = 28.0f;
        goalTime = 3.0f;

        rootString = "ShellMode";//�V�F�����[�h�Ɉڍs�B

        Instantiate(muzzleFlashEffect, transform.position, transform.rotation);
        Instantiate(shotEffect, transform.position, Quaternion.identity);
    }

    void Update()
    {
        switch(rootString)
        {
            case "ShellMode": //���˂��ꂽ�ۂ̏�ԁB�G�Ƀq�b�g����Ǝ��̏����Ɉڍs����B
                ShellMode();
                break;
            case "Stick_at": //�Ώۂɂ������������s���B
                Stick_at();
                break;
            case "Stick_TimeProcess": //�����t���Ă���̎��Ԍo�ߏ����B
                Stick_TimeProcess();
                break;
            case "Blast_Impact": //�����̏����B
                Blast_Impact();
                break;
        }
    }

    void FixedUpdate()
    {
        if(hitSW == false)
        {
            rb.velocity = (transform.forward * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hitSW == false)
        {
            if (other.tag != ownerTag && other.tag != "Search")
            {
                hitObj = other.gameObject;

                if (other.transform.GetComponent<StatusManager>() != null)
                {
                    gameObject.transform.position = other.transform.position;//�Ԃ������I�u�W�F�N�g�̈ʒu�Ɉړ�������B

                    gameObject.transform.parent = other.transform;//���������I�u�W�F�N�g�̎q�ɓ����B
                    hitSM = other.GetComponent<StatusManager>();

                    otherAS = other.GetComponent<AudioSource>();
                    otherAS.PlayOneShot(hitSE);

                    hitSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                    hitSM.St_Inflict_Shock(0.1f, 1);//�V���b�N��ԃ��x��1��0.1�b�^����B

                    Instantiate(targetHitEffect, hitObj.transform.position, Quaternion.identity);

                    Instantiate(pushDirectionalEffect, gameObject.transform.position, transform.rotation,transform);
                }
                else//�ÓI�ȃI�u�W�F�N�g�Ƀq�b�g�����ꍇ�B
                {
                    Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
                    audioSource.PlayOneShot(hitSE);
                }
                
                
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;//�d�͂𖳌�������B
                modelObject.SetActive(false);//���f���\���𖳌�������B

                Stick_at();
            }
        }
        
    }

    void ShellMode()
    {
        /*
        //���C�L���X�g���g�����ڒ��n�_�擾�B
        Vector3 ori = gameObject.transform.position;
        Ray ray = new Ray(ori, transform.forward);


        int outMask = ~LayerMask.GetMask(new string[] { "Search", ownerTag });//���L�҃^�O���Ɠ������C���[�����O�B

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f, outMask))
        {
            if (hit.point == null) hitPos = Vector3.zero;
            else if(hitPos != hit.point) hitPos = hit.point;
        }
        */
        //�j��܂ł̎��ԁB
        processTime += 1 * Time.deltaTime;
        if(processTime >= 3)
        {
            Destroy(gameObject);
        }
    }

    void Stick_at()//�����t���������̏����B�I�������Ɏ��ԏ����Ɉڍs�B
    {
        hitSW = true;

        
        // if(hitPos != Vector3.zero) gameObject.transform.position = hitPos;//���C�L���X�g�̓��������ʒu�Ɉړ�������B

        if (hitObj.GetComponent<Rigidbody>() != null)//���肪�d�͂ɉe�����󂯂�ꍇ�A���̕����֗͂�������B
        {
            hitObj.GetComponent<Rigidbody>().AddForce(transform.forward * 40, ForceMode.Impulse);//����𐁂���΂��B
        }
        
        stickProcessTime = 0;//�������Ԃ��O�ɂ���B
        rootString = "Stick_TimeProcess";
    }

    void Stick_TimeProcess()//���ԏ����B��莞�ԗ��Ɣ��������Ɉڍs�B
    {
        stickProcessTime += 1 * Time.deltaTime;
        if(stickProcessTime >= goalTime)
        {
            rootString = "Blast_Impact";//������ύX�B
        }
    }

    void Blast_Impact()//���������B
    {
        Instantiate(windEffect, gameObject.transform.position, Quaternion.identity);

        if (hitObj.GetComponent<Rigidbody>() != null)//���肪�d�͂ɉe�����󂯂�ꍇ�A���̕����֗͂�������B
        {
            otherAS.PlayOneShot(blastSE);

            hitSM.St_Inflict_Shock(0.2f, 2);
            hitObj.GetComponent<Rigidbody>().AddForce(transform.forward * 200, ForceMode.Impulse);//����𐁂���΂��B
        }
        else
        {
            audioSource.PlayOneShot(blastSE);
        }

        Destroy(gameObject);
    }
}
