using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUIManager : MonoBehaviour
{
    ////��{�B//----
    public GameObject ownerPlayerOBJ;//���L�҂ƂȂ�v���C���[�I�u�W�F�N�g�B
    public StatusManager ownerPlayerSM;//���L�҂̃X�e�[�^�X�}�l�[�W���[�B
    public PlayerMoving ownerPlayerMV;//���L�҂̃v���C���[���[�r���O�B

    public GameObject manaUI_OBJ;
    Animator manaAnima;
    int depositManapoint;//�ꎞ�I�ɐ��l��ۊǂ��A����Ɏg�p�B

    ////�v���C�A�u�������ɕ`�ʂ���UI�Ɋւ���B//------
    /*public GameObject playerUnderUI;//�v���C���[�I�u�W�F�N�g�����ɕ\������UI�B
    Camera visCamera;//�J�����̎w��B
    Transform p_tf;//�v���C���[�̍��W�B
    RectTransform uu_tf;//����UI��Rect���W�B
    Vector3 offsetPos = new Vector3(0,-1.5f,0);*/

    ////�v���C���[�̃X�e�[�^�XUI�Ɋւ���B//-----
    public CanvasGroup playerUIGroup;//�X�e�[�^�XUI�����܂ƂĂ���e�ɕt���Ă���R���|�[�l���g�B
    float guiAlpha;

    ////�X�e�[�^�XUI�e��Ɋւ���B//------
    float healthDeposit;//�̗͂��ꎞ�i�[����BUI���̍X�V�̔��ʂɎg�p����B
    public Image healthFillSprite;
    public Text healthText;

    public Image manaFillSprite;
    public Text manaText;

    //�Ǝ���
    public Image healthCrackCoverImage;
    //float vulnerableDeposit;
    

    //�Z�J���h�A�N�V�����u�V�t�g�_�b�V���v��PlayerMoving�X�N���v�g�ɑ��݂��܂��B
    public Image shiftDashFillSprite;
    public Text shiftDashCooltimeText;
    ////�����Ɋւ���B
    public string windowSwitch;//switch���̕���Ɏg�p�B

    //�A�j���[�V������

    public bool healthDangerSW;
    public GameObject healthDangerObj;//
    public Animator healthDangerAnimator;//�̗͒l�x���̃A�j���[�V�����B

    public Animator healthDamageImpactAnimator;//��_���[�W���̗̑͒l�A�j���[�V�����B

    void Start()
    {
        if (ownerPlayerSM == null) ownerPlayerSM = ownerPlayerOBJ.GetComponent<StatusManager>();
        if (ownerPlayerMV == null) ownerPlayerMV = ownerPlayerOBJ.GetComponent<PlayerMoving>();
        guiAlpha = 1.0f;

        manaAnima = manaUI_OBJ.GetComponent<Animator>();

        /*if (visCamera == null) visCamera = Camera.main;//���C���J����������B
        if(p_tf == null) p_tf = ownerPlayerOBJ.GetComponent<Transform>();//�v���C���[�̍��W���擾�B
        if (uu_tf == null) uu_tf = playerUnderUI.GetComponent<RectTransform>(); //����UI�̍��W���擾�B*/
    }

    
    void Update()
    {
        switch (windowSwitch)//�E�B���h�E�̓��ߏ�ԁA�n�C���C�g�̐؂�ւ��B
        {
            case "HideUIGroup":
                HideUIGroup();
                break;
            case "AppearUIGroup":
                AppearUIGroup();
                break;
        }
        
        if(healthDeposit != ownerPlayerSM.hitPoint)//�����ꎞ�i�[�̗͕ϐ��ƃv���C���[�̌��݂̗̑͂̐��l���Ⴄ�ꍇ�A
        {
            //Debug.Log("�̗͂��X�V�B");
            HitPointUILoad();//���X�V�̏������s���B
            healthDeposit = ownerPlayerSM.hitPoint;//�i�[�p�ϐ��Ɋi�[�B

        }

        if (ownerPlayerSM.vulnerableSt == true)//�����Ǝ��Ԃ̓�ɒl��true�ɂȂ��Ă����ꍇ�B
        {
            healthCrackCoverImage.enabled = true;
        }
        else if(healthCrackCoverImage.enabled == true)
        {
            healthCrackCoverImage.enabled = false;
        }

        ManaUILoad();//�v���C���[�̃}�i�ɍ��킹�Atext�ASprite��ύX����B
        SecondActionUILoad();//�Z�J���h�A�N�V�����̐��l�ɍ��킹text�Asprite��ύX����B
    }

    /*
    void chaseUnderUI()//[�p�~]����UI���v���C���[�ɒǔ�������B
    {
        uu_tf.position = RectTransformUtility.WorldToScreenPoint(visCamera, p_tf.position + offsetPos);
    }
    */


    public void HitPointUILoad()//�v���C���[�̗̑͂ɍ��킹�BText�ASprite��ύX����B
    {
        float health = ownerPlayerSM.hitPoint;//int����float�ɕϊ��B
        float maxHealth = ownerPlayerSM.maxHitPoint;//int����float�ɕϊ��B
        healthText.text = string.Format("{00}", health);//�̗͂̃e�L�X�g���X�V����B
        healthFillSprite.fillAmount = health / maxHealth;

        healthDamageImpactAnimator.SetTrigger("DamageImpact_Trigger");//��_�����ɃA�j���[�V�������Đ�����B
        
        if(healthDangerSW != false && maxHealth / 5 <= health)
        {
            healthDangerObj.SetActive(false);
            healthDangerAnimator.SetBool("HealthWarning_Bool", false);
            healthDangerSW = false;
        }
        else if (maxHealth / 5 >= health)//�ő�̗͂�5���̂P�������̗͂����Ȃ��ꍇ�B
        {
            healthDangerObj.SetActive(true);
            healthDangerAnimator.SetBool("HealthWarning_Bool", true);
            healthDangerSW = true;
        }
    }

    public void ManaUILoad()//�v���C���[�̃}�i�ɍ��킹�Atext�ASprite��ύX����B
    {
        float mana = ownerPlayerSM.manaPoint;
        float wantTime = ownerPlayerSM.manaWantTime;//�K�v���Ԃ��擾�B
        float progressTime = ownerPlayerSM.manaProgressTime;//�o�ߎ��Ԃ��擾�B
        if (mana == ownerPlayerSM.maxManaPoint && manaFillSprite.fillAmount != 1f)
        {
            manaText.text = string.Format("{00}", mana);//�}�i�̃e�L�X�g���X�V����B
            manaFillSprite.fillAmount = 1f;
        }
        else if(mana != ownerPlayerSM.maxManaPoint)
        {

            manaText.text = string.Format("{00}", mana);//�}�i�̃e�L�X�g���X�V����B
            manaFillSprite.fillAmount = progressTime / wantTime;
        }

        if(ownerPlayerSM.manaPoint != depositManapoint)
        {
            manaAnima.SetTrigger("ManaFlash_Trigger");
            depositManapoint = ownerPlayerSM.manaPoint;
        }
    }

    void SecondActionUILoad()//
    {
        float cooltime = ownerPlayerMV.sdAtCoolTime;//�N�[���^�C���B
        float maxCooltime = ownerPlayerMV.sdAtMaxCoolTime;//����p�N�[���^�C���B
        //bool actionSW = ownerPlayerMV.sdActionSW;//Action�̎g�p�\���B

        if (cooltime <= 0 && shiftDashCooltimeText.enabled == true)
        {
            shiftDashCooltimeText.enabled = false;//�e�L�X�g���B��
        }
        else if (cooltime >= 0 && shiftDashCooltimeText.enabled == false)
        {
            shiftDashCooltimeText.enabled = true;//�e�L�X�g��\������B
        }

        if (cooltime >= 0)
        {
            shiftDashCooltimeText.text = string.Format("{00}", cooltime);
            shiftDashFillSprite.fillAmount = 1 - (cooltime / maxCooltime);
        }
        else if (shiftDashFillSprite.fillAmount != 1) shiftDashFillSprite.fillAmount = 1;
    }

    void HideUIGroup()
    {
        if(guiAlpha >= 0.5f)
        {
            guiAlpha -= 5 * Time.deltaTime;
            playerUIGroup.alpha = guiAlpha;
        }
        else
        {
            guiAlpha = 0.5f;
            playerUIGroup.alpha = guiAlpha;
            windowSwitch = null;
        }
    }

    void AppearUIGroup()
    {
        if (guiAlpha <= 1f)
        {
            guiAlpha += 5 * Time.deltaTime;
            playerUIGroup.alpha = guiAlpha;
        }
        else
        {
            guiAlpha = 1.0f;
            playerUIGroup.alpha = guiAlpha;
            windowSwitch = null;
        }

    }

}
