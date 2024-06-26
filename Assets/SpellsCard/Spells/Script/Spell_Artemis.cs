using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Artemis : MonoBehaviour
{
    [Header("�X�y���f�[�^")]
    public SpellData spellDate;// �e��f�[�^�Q�ƂɎg�p�����B


    [Header("�G�t�F�N�g")]
    #region 
    [Tooltip("�������� �`�ʂ���G�t�F�N�g�ł��B")]
    public ParticleSystem GeneEffect;//�������̃G�t�F�N�g�B

    [Tooltip("�ˌ����� �`�ʂ���G�t�F�N�g�ł��B")]
    public ParticleSystem shotEffect;//�ˌ����̃G�t�F�N�g�B

    [Tooltip("�W�I�� ������������ �`�ʂ���G�t�F�N�g�ł��B")]
    public ParticleSystem standbyEffect;//�W�I�̔������̃G�t�F�N�g�B

    [Tooltip("�W�I�� �q�b�g�������� �`�ʂ���G�t�F�N�g�ł��B")]
    public ParticleSystem targetHitEffect;//�Ώۂւ̃q�b�g�G�t�F�N�g�B

    [Tooltip("��������� �q�b�g�������ɕ`�ʂ���G�t�F�N�g�ł��B")]
    public ParticleSystem hitEffect;//���I�u�W�F�N�g�ւ̃q�b�g�G�t�F�N�g�B
    #endregion


    [Header("�T�E���h")]
    #region 
    [Tooltip("�������� �Đ�������T�E���h�ł��B")]
    public AudioClip instSE;

    [Tooltip("��s���[�h���� �Đ�������T�E���h�ł��B")]
    public AudioClip flyingSE;

    [Tooltip("�W�I�� ������������ �Đ�������T�E���h�ł��B")]
    public AudioClip lockonSE;

    [Tooltip("�ˌ����� �Đ�������T�E���h�ł��B")]
    public AudioClip shotSE;

    [Tooltip("�W�I�� �q�b�g�����ۂ� �Đ�������T�E���h�ł��B")]
    public AudioClip hitSE;
    #endregion


    [Header("���f�� �A�j��")]
    [Tooltip("���f���̃A�j������Ɏg�p����܂��B")]
    public Animator animator;//���f���̃A�j������Ɏg�p����B

    [Header("���ˍ��W")]
    [Tooltip("��`���ꂽ�I�u�W�F�N�g�̍��W�� ���[�U�[�̎n�_�ɂȂ�܂��B")]
    public Transform laserShootPos;//���˂���ꏊ���I�u�W�F�N�g���g�p���Đݒ肷��B

    ////--------------------------------------------------
    
    SpellPrefabManager spm;

    Rigidbody rb;
    LineRenderer lineRenderer;
    AudioSource audioSource;

    [HideInInspector] public GameObject ownerObj;// ���L�҃I�u�W�F�N�g
    string ownerTag;// ���L�҂̃^�O

    int damage;// �Ώۂɗ^����_���[�W�l�B
    float objSpeed;// ���̃I�u�W�F�N�g�̈ړ��X�s�[�h�l�B


    string switchRoute;// switch���Ɏg�p����ϐ��B

    GameObject targetObj;// �^�[�Q�b�g�ƂȂ�I�u�W�F�N�g�����i�[�B

    float elapsedTime;// �o�ߎ��Ԃ��i�[���邽�߂̓��ꕨ�B
    float exTime;// �ˌ��O�̑ҋ@���Ԃ��i�[���邽�߂̓��ꕨ�B

    bool moveSW;// �O�i���邩�ǂ����̓�ɒl�B
    [HideInInspector] bool searchSW;// ���G�����ǂ����̓�ɒl�B
    bool rotationSW = true;// ��]

    Quaternion lookRotation;

    ////--------------------------------------------------

    void Start()
    {
        spm = GetComponent<SpellPrefabManager>();
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        damage = spellDate.primaryDamage;

        ownerObj = spm.ownerObject;//���L�҂ƂȂ�I�u�W�F�N�g���i�[����B
        ownerTag = spm.ownerObject.tag;//���L�҂̃^�O���i�[����B
        //gameObject.tag = ownerTag;

        audioSource.PlayOneShot(instSE);
        audioSource.loop = true;
        audioSource.clip = flyingSE;
        audioSource.Play();

        //Instantiate(shotEffect, transform.position, Quaternion.identity);//�X�y���g�p���̃G�t�F�N�g�𐶐��B
        lineRenderer.enabled = false;//�R���|�[�l���g��L�����B
        objSpeed = 5.0f;
        switchRoute = "Generate";
    }

    void Update()
    {
        switch (switchRoute)
        {
            case "Generate"://����
                Generate();
                break;
            case "Aviation"://��s
                Aviation();
                break;
            case "Discover"://����
                Discocer();
                break;
            case "Fire"://�ˌ�
                Fire();
                break;
            case "FadeOut"://�㏈��
                FadeOut();
                break;
        }
    }
    void Generate()//�������̋����B
    {
        var ownerSM = ownerObj.GetComponent<StatusManager>();
        ownerSM.St_Inflict_NoMove(0.2f);//���L�҂Ɉړ��s��t�^����B
        

        Instantiate(GeneEffect, gameObject.transform.position, Quaternion.identity);//�G�t�F�N�g����
        switchRoute = "Aviation";
    }

    void Aviation()//��s���B���G���B
    {
        if (moveSW == false) moveSW = true;

        elapsedTime += 1 * Time.deltaTime;//���Ԃ��o�߂�����B

        if (elapsedTime >= 4.0f)//�l�b�o�߂���ƃI�u�W�F�N�g���폜
        {
            Destroy(gameObject);
        }
        else if(elapsedTime >= 0.4f)//0.4�b�o�߂���ƍ��G���J�n����B
        {
            searchSW = true;
        }

        if(targetObj != null)//�^�[�Q�b�g���ݒ肳�ꂽ�ꍇ�A�����𔭌��ɕύX����B
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.PlayOneShot(lockonSE);
            switchRoute = "Discover";
        }
    }

    void Discocer()//�W�I�𔭌����B
    {
        rb.velocity = Vector3.zero;
        exTime += 1 * Time.deltaTime;//�ˌ��܂ł̒x�����Ԃ��o�߂�����B

        if (exTime >= 0.0f)//�G�̌����։�]������A�j���[�V���������
        {
            if (rotationSW == true)
            {
                
                Instantiate(standbyEffect, gameObject.transform.position, Quaternion.identity);
                lookRotation =
                        Quaternion.LookRotation(targetObj.transform.position - transform.position, transform.forward);//�G���������猩�Ăǂ̕��p�ɂ��邩�����G����B
                rotationSW = false;

            }
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.4f);//��]����B
            animator.SetBool("LockOn", true);//Animation��Bool�l.LockOn��true�ɁB
        }

        if (exTime >= 0.6f)//�^�[�Q�b�g�̕����m���Ɍ����A�ˌ��̏����Ɉڍs����B
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 2.0f);
            switchRoute = "Fire";
        }
    }

    void Fire()//�ˌ����쎞�B
    {
        audioSource.PlayOneShot(shotSE);

        Vector3 ori = gameObject.transform.position;
        Ray ray = new Ray(ori, transform.forward);

        int layerMask = ~LayerMask.GetMask(new string[] {ownerTag});

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f,layerMask))
        {

            Instantiate(shotEffect, laserShootPos.transform.position, Quaternion.identity);//�G�t�F�N�g����
            
            lineRenderer.enabled = true;//���C�������_�����O�R���|�[�l���g��L�����B

            lineRenderer.SetPosition(0, laserShootPos.transform.position);
            lineRenderer.SetPosition(1, hit.point);


            if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != ownerTag)
            {
                AudioSource hitAS = hit.transform.GetComponent<AudioSource>();
                hitAS.PlayOneShot(hitSE);

                Instantiate(targetHitEffect, hit.transform.position, Quaternion.identity);//���������G�t�F�N�g
                var hitSM = hit.transform.GetComponent<StatusManager>();
                hitSM.HP_Inflict_Damage(damage);//�_���[�W�𔭐�������B
                hitSM.St_Inflict_Shock(0.2f, 1);//�V���b�N��ԃ��x��1��0.2�b�^����B
                //hitSM.St_Inflict_Invincible(0.4f);//0.4�b�̖��G���Ԃ�t�^����B
            }
            else
                Instantiate(hitEffect, hit.point, Quaternion.identity);//�G�t�F�N�g�𐶐��B

            //string name = hit.collider.gameObject.name;//�q�b�g�����I�u�W�F�N�g�̖��O���i�[�B
            //Debug.Log(name);//�q�b�g�����I�u�W�F�N�g�̖��O�����O�ɏo���B
        }

        switchRoute = "FadeOut";
        //Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 1.0f);//��΂���Ray�̋O��������B

    }

    void FadeOut()//�s����㏈���B
    {
        if(lineRenderer.startWidth >= 0 || lineRenderer.endWidth >= 0)
        {
            lineRenderer.startWidth -= 0.1f;
            lineRenderer.endWidth -= 0.1f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(moveSW == true)
        {
            rb.velocity = transform.forward * objSpeed;//�O�i������B
        }
    }

    private void OnTriggerStay(Collider other)
    {
        string hitObjectTag = other.tag;
        if (searchSW == true && (hitObjectTag != "Search" && hitObjectTag != "Untagged" && hitObjectTag != "Structure" && hitObjectTag != "Installation" && hitObjectTag != "Ground" && hitObjectTag != ownerTag))
        {
            if(other.GetComponent<StatusManager>() != null)
            {
                GameObject target = other.gameObject;

                Vector3 pos = target.transform.position - transform.position;

                Ray ray = new Ray(transform.position, pos);//�R���W���������G�̍��W�����C�ڕW�ʒu�ɐݒ�B

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20f))
                {
                    if (hit.collider.gameObject == target)
                    {
                        targetObj = target.gameObject;
                        moveSW = false;
                        searchSW = false;
                    }
                }
            }

            //Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 1.0f);//��΂���Ray�̋O��������B
        }
    }
}
