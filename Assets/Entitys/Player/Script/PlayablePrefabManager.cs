using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayablePrefabManager : MonoBehaviour
{
    [Header("�I�u�W�F�N�g")]
    public GameObject playableObject;

    public GameObject canvasOBJ;

    [Space]
    [Space]

    //�O�I����
    public GameObject playable_Skin_Head;//�O���̓�
    public GameObject playable_Skin_Body;//�O���̑�

    [Space]

    //���o����
    public GameObject playable_CursorRing;
    public GameObject playable_DireArrow;


    [Header("�e�� UI")]
    public GameObject ui_CustomWindow;

    public GameObject ui_GeneralSelectWindow;

    public GameObject ui_Pallet;

    public Image ui_Pallet_Frame;

    public Image ui_DestroyCover;

    [Space]
    //NumberSprite�EText�֌W�B
    public Text PalletFrame_NumberTag_Text;

    public List<Image> PCW_NumberTag_Image;


    [Header("�X�N���v�g")]
    public PlayingData playable_PD;//�v���C���O�f�[�^�B

    public StatusManager playable_SM;//�X�e�[�^�X�}�l�[�W���[�B

    public PlayerMoving playable_PM;//�v���C���[�}�l�[�W���[�B

    public HandCardManager playable_HCM;//�n���h�J�[�h�}�l�[�W���[�B

    public PlayerUIManager playable_UI_Manager;//�v���C���[UI�}�l�[�W���[�B

    public PlayerPreparationInputDate playable_PID;

    public PlayerGSWInput playable_GSW;

    public PlayerAdopt_Card playeable_Adopt_Card;


    void Awake()
    {
        // �e�X�N���v�g�̃L���b�V�����v���C�A�u���I�u�W�F�N�g����擾����B
        playable_PD = playableObject.GetComponent<PlayingData>();

        playable_SM = playableObject.GetComponent<StatusManager>();

        playable_PM = playableObject.GetComponent<PlayerMoving>();

        playable_HCM = playableObject.GetComponent<HandCardManager>();

        playable_UI_Manager = playableObject.GetComponent<PlayerUIManager>();

        playable_PID = playableObject.GetComponent<PlayerPreparationInputDate>();

        playable_GSW = playableObject.GetComponent<PlayerGSWInput>();

        playeable_Adopt_Card = playableObject.GetComponent<PlayerAdopt_Card>();
    }
}
