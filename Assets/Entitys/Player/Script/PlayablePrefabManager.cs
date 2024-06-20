using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayablePrefabManager : MonoBehaviour
{
    public GameObject playableObject;

    public GameObject canvasOBJ;

    public GameObject ui_CustomWindow;

    public GameObject ui_GeneralSelectWindow;

    public GameObject ui_Pallet;

    public Image ui_Pallet_Frame;

    public Image ui_DestroyCover;

    public PlayingData playable_PD;//�v���C���O�f�[�^�B

    public StatusManager playable_SM;//�X�e�[�^�X�}�l�[�W���[�B

    public PlayerMoving playable_PM;//�v���C���[�}�l�[�W���[�B

    public HandCardManager playable_HCM;//�n���h�J�[�h�}�l�[�W���[�B

    public PlayerUIManager playable_UI_Manager;//�v���C���[UI�}�l�[�W���[�B

    public PlayerPreparationInputDate playable_PID;

    public PlayerGSWInput playable_GSW;

    public PlayerAdopt_Card playeable_Adopt_Card;
    //NumberTag�֌W�B
    public List<Image> PCW_NumberTag_Image;
    public Text PalletFrame_NumberTag_Text;

    //�O���v�f

    //�L�����N�^�[����

    public GameObject playable_Skin_Head;//�O���̓�
    public GameObject playable_Skin_Body;//�O���̑�

    //���o����
    public GameObject playable_CursorRing;
    public GameObject playable_DireArrow;

}
