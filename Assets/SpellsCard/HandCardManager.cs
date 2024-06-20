using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class HandCardManager : MonoBehaviour
{
    //���̃X�N���v�g�̃Z���t�e�B(false�ŉ������)
    public bool HCM_ProcessSafety;

    ////���X�N���v�g�֘A�B
    PlayingData playingData;//�X�N���v�g�uplayingData�v�i�[�p
    PlayerMoving playerMoving;//�X�N���v�g�uplayerMoving�v�i�[�p
    StatusManager statusManager;//�X�N���v�g�uStatusManager�v�i�[�p�B

    ////�X�y���f�b�L�֘A�B
    public DeckData deckInst;

    //�n���h�P�[�X�̃A�N�V�����A�j���[�V�����֌W�B(UI��Pallet���Ɋ܂܂��Q�[���I�u�W�F�N�gActionAnima���A�^�b�`����B)
    public Animator handAnimator;//�n���h�P�[�X�̃A�j���[�V�����B
    public Text cooltime_Text;//�N�[���^�C���̃e�L�X�g�B
    public Image cooltime_FillImage;//�N�[���^�C���̓h��Ԃ��B

    //�n���h�P�[�X�̃L���X�g�p�l���̃A�j���[�V�����֌W�B(UI��Pallet���Ɋ܂܂��Q�[���I�u�W�F�N�gCastPanelAnima���A�^�b�`����B)
    public Animator castPanelAnimator;

    ////�C���X�y�N�^�[�ł̃A�^�b�`�K�{����
    public List<SpellData> deckCards;//�f�b�L�J�[�h
    [SerializeField] List<SpellData> depositDeck;//�V���t������ۂɓ���Ă������Ƃ��o����B
    public List<SpellData> handCards;//��D

    public Text castCardNameText;//�L���X�g�J�[�\���ɓ����Ă���J�[�h�̖��O�̕\������B
    public Text castCardDamageText;//�L���X�g�J�[�\���ɓ����Ă���J�[�h�̃_���[�W�l�̕\���B

    public List<PickCard> pickWindow;// �n���h�P�[�X�̕`�ʗpGUIImage�B
    public SpellData noneCardData;// ��\���p�̃J�[�h�f�[�^�B

    public Image CastCardCover;//�J�[�h���g�p�ł��Ȃ��ꍇ�ɕ\������J�o�[�摜�B

    public Animator manaCostWindow_0_Animator;//�őO��̃J�[�h�̃}�i�\���E�B���h�E��Animator�R���|�[�l���g�B

    public List<Text> manaCostWindow;//�n���h�P�[�X�̊e�J�[�h�̃R�X�g��`�ʂ���B

    public Text deckCountText;//�f�b�L�Ɋ܂܂��J�[�h�c��

    //
    public GameObject instPos;//�v���n�u�̐����n�_�Ƃ��Ďg�p������W�I�u�W�F�N�g�B

    ////����p
    public string switchRoot;
    [SerializeField]bool standbySW;//�����������ۂ��B

    //
    public int deckCount;//�����񐔃J�E���g�p
    int handCard_max = 5;//��D���

    //�N�[���^�C��
    public float drawCT;//�i�s�^�N�[���^�C���B
    public float drawCT_max;//�h���[�ɔ�������N�[���^�C���̒����B

    public float deckReloadCT;//�i�s�^�N�[���^�C���B
    public float deckReloadCT_max;//�f�b�L�����[�h�ɔ�������N�[���^�C���̒����B(basisTime��sumTime�̍��v�l)
    public float deckReloadCT_basisTime;//�f�b�L�����[�h�ɔ�������N�[���^�C���̊�b���l�B
    public float deckReloadCT_sumTime;//�g�p�����J�[�h�ɍ��킹�������鎞�Ԓl�����炩���ߊi�[����B�K�v�Ȏ��ɌĂт����A���l����ɂ���B

    //���͊֌W


    //���b�N�I���V�X�e���n
    public GameObject lockOnTarget;//���b�N�I�������I�u�W�F�N�g���i�[�B
    StatusManager TargetSM;//���b�N�I�������I�u�W�F�N�g��StatusManager�B
    public LineRenderer lineRenderer;
    float lockOnDistance;//���b�N�I���\�����B
    bool canlockOnSW;//���b�N�I���ł��邩���ɒl�ŕ\���B
    float cantLockOnTime;//���b�N�I�����N�[���^�C���B
    float lockOnOutTime;//���b�N�I���ێ����ԁB���b�N�I�����ɕǉz���ɂȂ����ۂ̉����܂ł̎��ԁB

    float lineSize;

    float lockOnLineInstPoint;//���b�N�I�����ɕ`�ʂ�����̕\�����鍂���B

    //�T�E���h�n

    AudioSource audioSource;

    public AudioClip ac_HandCase_Reload_Start;
    public AudioClip ac_HandCase_Reload_Charge;
    public AudioClip ac_HandCase_Reload_End;

    public AudioClip cardFoldSE;

    public AudioClip lockOn_InSE;
    public AudioClip lockOn_OutSE;

    void Start()
    {
        switchRoot = "SetUp";
    }

    
    void Update()
    {
        if(HCM_ProcessSafety == false)
        {
            switch (switchRoot)
            {
                case "SetUp"://�������s���O�ɔ��������鏈���B
                    SetUp();
                    break;

                case "HandCard_Cast"://�g�p
                    HandCard_Cast();
                    break;
                case "HandCard_Trash"://�j��
                    HandCard_Trash();
                    break;

                //

                case "SetUpDeckCharge":// �v���C���[���r���h�����f�b�L�f�[�^���f�b�L�ɓ���鏈���B
                    Debug.Log("�f�b�L�`���[�W");
                    break;
                case "DeckShuffle":// �f�b�L�̃V���b�t�����s������
                    deckCount = deckCards.Count;//�ꎞ�f�b�L���疇���J�E���g���擾�B
                    DeckShuffle();
                    if (deckCount <= 0) switchRoot = "MixingDeckCharge";
                    break;
                case "MixingDeckCharge":// �V���b�t�������f�b�L��{�f�b�L�ɕ�[���s������
                    deckCards = new List<SpellData>(depositDeck);
                    depositDeck.Clear();
                    switchRoot = "CardDraw";
                    break;
                case "CardDraw":// �{�f�b�L����J�[�h�����o���A��D�ɓ����B
                    HandDraw();
                    break;
                case "HundFalld"://��D�����ׂĔj�����A�N�[���^�C���𔭐����������CardDrow�Ɉڍs����B
                    HundFalld();
                    break;

                case "DrawCoolTime"://�h���[���ɔ������鎞�Ԍo�ߏ����B
                    DrowingTime();
                    break;
                case "DeckReloadingTime"://�f�b�L�����[�h���ɔ������鎞�Ԍo�ߏ����B
                    DeckReloadingTime();
                    break;

                default:
                    Standby();
                    break;
            }

            LockOnProcessing();//���b�N�I�����̏������s���B
        }
    }

    public void SetUp()//Start�ŏ���������e�B
    {
        Debug.Log("�Z�b�g�A�b�v���Ă܂�");
        drawCT_max = 2;
        deckReloadCT_basisTime = 5;
        //�������B
        deckReloadCT_sumTime = 0;
        cooltime_Text.enabled = false;
        cooltime_FillImage.enabled = false;
        lockOnLineInstPoint = (gameObject.transform.localScale.y * -0.9f) + gameObject.transform.position.y;//���b�N�I�����̕`�ʐ��̍�����ݒ�B
        //

        statusManager = GetComponent<StatusManager>();

        playingData = GetComponent<PlayingData>();

        //�f�b�L�f�[�^������B
        if (playingData.onHandDeckDate.Count == 0)
        {
            //
            deckCards = new List<SpellData>(deckInst.spells);
        }
        else
        {
            deckCards = new List<SpellData>(playingData.onHandDeckDate);
        }

        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();//�R���|�\�l���g���擾�B
        lineRenderer.enabled = false;//�R���|�[�l���g�̖������B

        audioSource = GetComponent<AudioSource>();

        depositDeck.Clear();
        handCards.Clear();

        HundWindowRefresh();//�\�L�̍X�V�B

        switchRoot = "DeckShuffle";
    }

    void Standby()//�v���C���[�̓��͂��󂯕t����B
    {
        if (standbySW == false) standbySW = true;//�X�^���o�C��ԂɂȂ�B

        //�s�b�N�E�B���h�E��̃J�[�h������R�X�g�𖞂����Ă���ꍇ�J�o�[�摜���\���ɂ���B
        if (statusManager.manaPoint >= handCards[0].manaCost && CastCardCover.enabled == true) CastCardCover.enabled = false;
        else if (statusManager.manaPoint < handCards[0].manaCost && CastCardCover.enabled == false) CastCardCover.enabled = true;

        if (handCards[0] == noneCardData)//�n���h�P�[�X���̃J�[�h����0�ꍇ�B
        {
            standbySW = false;//�X�^���o�C��Ԃ𖳌�������B
            switchRoot = "HundFalld";//��D�̎�O��noneCardDate�������ꍇ�A�h���[���s���B
        }

        /*
        //�������́B
        if (Input.GetKeyDown(KeyCode.Space)) switchRoot = "HundFalld";
        if(Input.GetMouseButtonDown(0)) switchRoot = "HandCard_Cast"; 
        if (Input.GetMouseButtonDown(1)) switchRoot = "HandCard_Trash";
        */
    }


    void HandCard_Cast()//�J�[�h�g�p
    {
        if (handCards.Count >= 0)
        {
            standbySW = false;//standby��Ԃ𖳌�������B

            castPanelAnimator.SetTrigger("CastTrigger");//�A�j���[�V�������Đ��B

            Vector3 vc3InstPos = 
                instPos.transform.position;//InstObj��Vector3�ɕϊ��A�i�[�B

            GameObject instPrefab = 
                Instantiate(handCards[0].spellPrefab, vc3InstPos, instPos.transform.rotation);//�J�[�h�f�[�^�����Ƀv���n�u�𐶐�����B

            instPrefab.GetComponent<SpellPrefabManager>().ownerObject = gameObject;//�X�y���v���n�u�}�l�[�W���[�ɃI�u�W�F�N�g����n���B

            //�g�p�����J�[�h�����炷�����B
            handCards.RemoveAt(0);
            pickWindow[handCards.Count - 1].CardRefresh();
            handCards.Add(noneCardData);

            HundWindowRefresh();

        }
    }

    void HandCard_Trash()//�J�[�h�j��
    {
        if (handCards.Count >= 0)
        {
            castPanelAnimator.SetTrigger("TrashTrigger");//�A�j���[�V�������Đ��B
            audioSource.PlayOneShot(cardFoldSE);//����炷�B
            statusManager.Mana_Inflict_Revenue(handCards[0].trashMana);//�}�i�|�C���g�̎擾�B

            //�g�p�����J�[�h�����炷�����B
            handCards.RemoveAt(0);
            pickWindow[handCards.Count - 1].CardRefresh();
            handCards.Add(noneCardData);

            HundWindowRefresh();

        }
    }

    void DeckShuffle()//�f�b�L�V���b�t��
    {
        int rundamInt = Random.Range(0, deckCount);

        if(deckCount > 0)
        {
            depositDeck.Add(deckCards[rundamInt]);
            deckCards.RemoveAt(rundamInt);
        }
    }
    

    public void HundWindowRefresh()//��D�̍ēǂݍ��݁B����ɂ�荡�����Ă����D�̉摜���X�V�����B
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            pickWindow[i].cardDate = handCards[i];
            pickWindow[i].CardRefresh();
            if(handCards[i] == noneCardData)
            {
                manaCostWindow[i].enabled = false;
            }
            else
            {
                if (manaCostWindow[i].enabled == false) manaCostWindow[i].enabled = true;
                manaCostWindow[i].text = string.Format("{00}", handCards[i].manaCost);
            }

            if(i == 0)//�ŏ��̏�������B
            {
                if (handCards[0] == noneCardData)
                {
                    castCardNameText.text = " ";
                    castCardDamageText.text = " ";
                }
                else
                {
                    castCardNameText.text = handCards[0].spellName;
                    if (handCards[0].damageString != null)
                        castCardDamageText.text = handCards[0].damageString + handCards[0].attackOftenString;
                }
            }
        }
        
        deckCountText.text = string.Format("{00}", deckCards.Count);//�f�b�L�c�ʕ\�����X�V����B

        switchRoot = default;
    }

    void HandDraw()//�f�b�L�����D�ɃJ�[�h�f�[�^��n���B
    {

        for(int i = 0; i < handCard_max; i++)
        {
            if(deckCards.Count <= 0)
            {
                handCards.Add(noneCardData);
            }
            else
            {
                handCards.Add(deckCards[0]);
                deckCards.RemoveAt(0);
            }
        }
        HundWindowRefresh();
    }

    void HundFalld()//��D��j�����鏈���B�܂���������R�D(�f�b�L)����J�[�h���h���[���鏈���ɂȂ���B
    {
        if (deckCards.Count > 0)//�f�b�L���ɃJ�[�h���L��A�h���[���\�ȏꍇ�B
        {
            handCards.Clear();

            audioSource.PlayOneShot(ac_HandCase_Reload_Start);//SE���Đ�����B

            audioSource.loop = true;//���[�v���I���ɂ���B
            audioSource.clip = ac_HandCase_Reload_Charge;//bgm�ݒ�B
            audioSource.Play();//bgm�Đ��B

            for (int i = 0; i < handCard_max; i++)
            {
                pickWindow[i].cardDate = noneCardData;
                pickWindow[i].CardRefresh();
                //Debug.Log("��D�`�ʓK���I");
            }
            drawCT = drawCT_max;//�h���[�ɕK�v�ȃN�[���^�C�����Đݒ�B

            handAnimator.SetBool("DrawBool", true);
            switchRoot = "DrawCoolTime";//�h���[�̏������s���B
        }
        else if(deckCards.Count <= 0)//�f�b�L��0�ɂȂ��Ă���ꍇ�Ɏ擾����B
        {
            handCards.Clear();

            audioSource.PlayOneShot(ac_HandCase_Reload_Start);//SE���Đ�����B

            audioSource.loop = true;//���[�v���I���ɂ���B
            audioSource.clip = ac_HandCase_Reload_Charge;//bgm�ݒ�B
            audioSource.Play();//bgm�Đ��B

            //�f�b�L�f�[�^������B
            if (playingData.onHandDeckDate.Count == 0)
            {
                //
                deckCards = new List<SpellData>(deckInst.spells);
            }
            else
            {
                deckCards = new List<SpellData>(playingData.onHandDeckDate);
            }

            deckReloadCT_max = deckReloadCT_basisTime + deckReloadCT_sumTime;//���v�N�[���^�C���̐��l���v�Z�B(��b�l�Ə�Z�l�����킹�B���v���Ԃɓ����B)
            deckReloadCT_sumTime = 0;//sumTime���[���ɂ���B
            deckReloadCT = deckReloadCT_max;//�f�b�L�����[�h�ɕK�v�ȃN�[���^�C�����Đݒ�B
            handAnimator.SetBool("DeckReloadBool", true);
            switchRoot = "DeckReloadingTime";//�f�b�L�����[�h�̏������s���B
        }
        else
        {
            Debug.Log("NullDeck");
            switchRoot = default;
        }
    }

    void DrowingTime()//��D���h���[�����܂ł̎��ԏ����B
    {
        if(drawCT >= 0)
        {
            drawCT -= 1 * Time.deltaTime;

            if(cooltime_FillImage.enabled == false) cooltime_FillImage.enabled = true;
            if (cooltime_Text.enabled == false) cooltime_Text.enabled = true;


            cooltime_Text.text = string.Format("{00}", drawCT);//
            cooltime_FillImage.fillAmount = drawCT / drawCT_max;
        }
        else
        {
            Debug.Log("�h���[�N�[���^�C���I��");

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//���[�v�𖳌��B
            audioSource.Stop();//�I�[�f�B�I�𖳌�������B
            audioSource.clip = null;//�ݒ艹���𖳌����B

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SE���Đ�����B

            handAnimator.SetBool("DrawBool", false);
            switchRoot = "CardDraw";
            
        }
    }

    void DeckReloadingTime()//�R�D���V���b�t�������܂ł̎��ԏ����B
    {
        if(deckReloadCT >= 0)
        {

            deckReloadCT -= 1 * Time.deltaTime;

            if (cooltime_FillImage.enabled == false) cooltime_FillImage.enabled = true;
            if (cooltime_Text.enabled == false) cooltime_Text.enabled = true;

            cooltime_Text.text = string.Format("{00}", deckReloadCT);//
            cooltime_FillImage.fillAmount = deckReloadCT / deckReloadCT_max;
        }
        else
        {
            Debug.Log("�����[�h�N�[���^�C���I��");

            cooltime_Text.enabled = false;
            cooltime_FillImage.enabled = false;

            audioSource.loop = false;//���[�v�𖳌��B
            audioSource.Stop();//�I�[�f�B�I�𖳌�������B
            audioSource.clip = null;//�ݒ艹���𖳌����B

            audioSource.PlayOneShot(ac_HandCase_Reload_End);//SE���Đ�����B

            Debug.Log("�f�b�L��[");
            handAnimator.SetBool("DeckReloadBool", false);
            switchRoot = "DeckShuffle";//�f�b�L�����[�h�̏������s���B
        }
    }

    ////�V�����́B---------------------------------------------

    public void CardCast(InputAction.CallbackContext context)//�J�[�h�g�p
    {
        if(HCM_ProcessSafety == false)
        {
            if (context.performed)
            {
                if (statusManager.shockSt == false)
                {
                    if (standbySW == true)
                    {
                        if (context.phase == InputActionPhase.Performed)
                        {
                            if (statusManager.manaPoint >= handCards[0].manaCost)
                            {
                                statusManager.Mana_Inflict_Expense(handCards[0].manaCost);//�R�X�g�������B
                                HandCard_Cast();//�������J�n�B
                                Debug.Log("�J�[�h�g�p");
                                standbySW = false;
                            }
                            else
                            {
                                Debug.Log(gameObject + ">> �X�y���g�p�ɕK�v�ȃ}�i������Ȃ��B");
                                manaCostWindow_0_Animator.SetTrigger("CostNonEnough_Trigger");
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void CardTrash(InputAction.CallbackContext context)//�J�[�h�j��
    {
        if (HCM_ProcessSafety == false)
        {
            if (context.performed)
            {
                if (statusManager.shockSt == false)
                {
                    if (standbySW == true)
                    {
                        if (context.phase == InputActionPhase.Performed)
                        {
                            HandCard_Trash();//�������J�n�B
                            Debug.Log("�J�[�h�j��");
                            standbySW = false;
                        }
                    }
                }
            }
        }
    }

    public void LockOnSearch(InputAction.CallbackContext context)//���b�N�I�����s���B
    {
        if (HCM_ProcessSafety == false)
        {
            if (context.performed)
            {
                if (lockOnTarget == null && canlockOnSW == true)
                {
                    lockOnDistance = 30.0f;//�e�X�g�Ń��b�N�I���ł��鋗����ݒ�B

                    Vector3 ori = gameObject.transform.position;
                    Ray ray = new Ray(ori, transform.forward);
                    int layerMask = ~LayerMask.GetMask(new string[] { "Default", "StructureObject", "InstObject", "Search",gameObject.tag});//Layer���肩�珜�O����B�^�O���ƃ��C���[���������ł��鎖���K�{�B
                    RaycastHit hit;

                    if (Physics.BoxCast(transform.position, new Vector3(1.5f, 1.5f, 1.5f), transform.forward, out hit, Quaternion.identity, lockOnDistance, layerMask))
                    {
                        if (hit.transform.GetComponent<StatusManager>() != null && hit.transform.tag != gameObject.tag)//�X�N���v�g�uStatusManager�v���t���Ă��Ď��g�ƈႤ�^�O���t���Ă���ꍇ�ɔ�������B
                        {
                            Debug.Log("���b�N�I���I");

                            audioSource.PlayOneShot(lockOn_InSE);//���ʉ���炷�B

                            lockOnTarget = hit.transform.gameObject;
                            if (playerMoving == null) playerMoving = GetComponent<PlayerMoving>();
                            playerMoving.lockOnTargetObj = lockOnTarget;
                            cantLockOnTime = 0.2f;//�N�[���^�C����ݒ�B
                            canlockOnSW = false;//���b�N�I����s�\�ɂ���B

                            if (TargetSM == null) TargetSM = lockOnTarget.GetComponent<StatusManager>();//�^�[�Q�b�g���擾�B
                            
                        }
                        //string name = hit.collider.gameObject.name;//�q�b�g�����I�u�W�F�N�g�̖��O���i�[�B
                        //Debug.Log(name);//�q�b�g�����I�u�W�F�N�g�̖��O�����O�ɏo���B

                    }
                }
                else if (lockOnTarget != null && canlockOnSW == true)
                {
                    LockOnOut(); //���b�N�I�����������ꂽ�ۂ̏����𑖂点��B
                }
            }
        }
    }

    //--------------------------------------------------------------

    void LockOnProcessing()
    {
        if(canlockOnSW == false)//���b�N�I���ł��Ȃ���Ԃł���ꍇ�A
        {
            //canTime�̐��l���o�ߒቺ������B
            if (cantLockOnTime >= 0)
            {
                cantLockOnTime -= 1 * Time.deltaTime;
            }
            else if (cantLockOnTime <= 0.0f && cantLockOnTime != 0)
            {
                cantLockOnTime = 0.0f;
                canlockOnSW = true;
            }
        }

        if(lockOnTarget != null)//�^�[�Q�b�g���i�[����Ă���ꍇ�́B
        {
            LockOnLineChain();//���b�N�I�����Ă���I�u�W�F�N�g�̊Ԃɐ��𐶐��B
            GameObject target = lockOnTarget;
            Vector3 pos = target.transform.position - transform.position;

            Ray ray = new Ray(transform.position, pos);//�R���W���������G�̍��W�����C�ڕW�ʒu�ɐݒ�B

            int layerMask = LayerMask.GetMask(new string[] { "Default","StructureObject", "InstObject",target.tag}); //Layer�����ݒ肷��B�^�O���ƃ��C���[���������ł��鎖���K�{�B
            RaycastHit hit;

            //Debug.DrawRay(ray.origin, ray.direction * lockOnDistance, Color.red, 0.2f);//�e�X�g�@�\�B��΂���Ray�̋O��������B

            if (Physics.Raycast(ray, out hit, lockOnDistance,layerMask))
            {

                if (hit.collider.gameObject != target)//�������b�N�I�����Ă���I�u�W�F�N�g���ǉz���ɂȂ�ƁB
                {
                    //���b�N�I�������܂ł̎��Ԍo�߁A�������s���B

                    if (lockOnOutTime >= 0)
                    {
                        lockOnOutTime -= 1 * Time.deltaTime;
                    }
                    else if (lockOnOutTime <= 0)
                    {
                        LockOnOut(); //���b�N�I�����������ꂽ�ۂ̏����𑖂点��B
                    }
                }
                else if (lockOnOutTime <= 0.5f)//�����A���b�N�I�����Ă���I�u�W�F�N�g�̃��C���[�����o���Ă���Ԃ́B
                {
                    //���b�N�I�������܂ł̎��Ԃ����Z�b�g�B
                    lockOnOutTime += 0.1f;
                }
            }

            if (TargetSM.hitPoint <= 0) LockOnOut();//�������b�N�I�����Ă���Ώۂ�HP��0�̏ꍇ�A���b�N�I�����I�t�ɂ���B
        }
    }

    void LockOnOut()//���b�N�I�����������ꂽ�ۂ̂̏����B
    {
        Debug.Log("���b�N�I�������I");

        audioSource.PlayOneShot(lockOn_OutSE);//���ʉ���炷�B

        lineRenderer.enabled = false;//�R���|�[�l���g�𖳌���������B
        lineSize = 0f;
        lockOnTarget = null;
        TargetSM = null;
        if (playerMoving == null) playerMoving = GetComponent<PlayerMoving>();
        playerMoving.lockOnTargetObj = lockOnTarget;
        cantLockOnTime = 1f;//�N�[���^�C����ݒ�B
        canlockOnSW = false;//���b�N�I����s�\�ɂ���B
    }

    void LockOnLineChain()//���b�N�I�����̃I�u�W�F�N�g�Ԃ̐���\��������B
    {
        lineRenderer.enabled = true;
        Vector3 p_vc3 = new Vector3(gameObject.transform.position.x, lockOnLineInstPoint, gameObject.transform.position.z);
        Vector3 t_vc3 = new Vector3(lockOnTarget.transform.position.x, lockOnLineInstPoint, lockOnTarget.transform.position.z);

        
        lineRenderer.SetPosition(0, p_vc3);
        lineRenderer.SetPosition(1, t_vc3);

        if (lineSize <= 0.5f)
        {
            lineSize += 5 * Time.deltaTime;
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;
        }
    }
    
}
