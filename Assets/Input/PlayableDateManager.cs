using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayableDateManager : MonoBehaviour
{
    //���̃X�N���v�g�̓v���C���[�̑���L�����Ƃ��̃L������UI�𓯎��Ɋi�[���A�Ǘ�����ׂ̃X�N���v�g�ɂȂ�܂��B
    //�܂��ǂ̃V�[���ɂ����̃X�N���v�g��ێ�����K�v������܂��B

    PlayerInputManager playerInputManager;/* �{�^���������ƃQ�[���ւ̎Q�����s���B
    �C���Q�[�����ł͖��������鎖�������B(�Đڑ��I�ɏ�肭�����Ȃ��ꍇ������ׁB
    ���͉����ɑ���L��������x�ł����͉�����ƌ������͂��D�����ג��ӁB)�@*/
    int playerGrantNumberCount;//�v���C���[�ɕt�^����i���o�[���i�[����B����͏����̒��Ŏg�p�����B
    public int maxEntryJoinPlayerInt;//���݁A�ő剽�l�܂œ���邩�B(����ɂ��Q���\�l���𐧌�����B�܂�����͎��R���ŕ\���B)
    public int joinPlayerInt;//�Q�[���ɎQ�����Ă���v���C���[�l���B

    string unPlayableSearchTag = "UnknownPlayer";//���������������v���C���[��T���ۂɎg�p����^�O�B
    string joinPlayableTag = "JoinPlayer";//���������v���C���[�Ɏg�p����^�O�B

    public GameObject playersPrefab_Group;//���������v���C�A�u���v���n�u���܂Ƃ߂�I�u�W�F�N�g

    //�e�v���C�A�u���L�����̃X�N���v�g�擾�B
    public List<GameObject> playableChara_OBJ;//����L�����{�́B

    public GameObject playable_CanvasOBJ;

    public List<GameObject> playableChara_UI_PCW;//UI_PlayerCustomWindow

    public List<GameObject> playableChara_UI_GSW;//UI_GeneralSelectWindow

    public List<GameObject> playableChara_UI_Pallet;//UI_Pallet�B

    public List<Image> playableChara_UI_PalletFrame_Image;//UI_Pallet�̃t���[���B

    public List<Image> playableCharaUI_DestroyCover;//UI_Destroy�̃J�o�[�t���[���B

    public List<PlayingData> playeable_PD;//PlayingData�B

    public List<StatusManager> playableChara_SM;//�X�e�[�^�X�}�l�[�W���[�B

    public List<PlayerMoving> playableChara_PM;//�v���C���[�}�l�[�W���[�B

    public List<HandCardManager> playableChara_HCM;//�n���h�J�[�h�}�l�[�W���[�B

    public List<PlayerUIManager> playableChara_UI_Manager;//�v���C���[UI�}�l�[�W���[�B

    public List<PlayerPreparationInputDate> playableChara_PID;//�v���C���[�̐ݒ��ʂ̓��͒l�ۊǁB

    public List<PlayerGSWInput> PlayeableChara_GSW;//�ėp�I��g�̑���p�X�N���v�g�B

    public List<PlayerAdopt_Card> playeableChara_Adopt_Card;//�v���C���[�̃J�[�h���󂯓n�����߂̃X�N���v�g�B

    //�e�v�f�̏������s��Script���A�^�b�`����K�v������B

    WindowManager windowManager;//CustomWindow�̃X�i�b�v���s�����萧����s���B

    public List<Color32> presetPlayerColor;//�v���Z�b�g�̃v���C���[�̃J���[�����O�Q�B�C���X�y�N�^�[����ݒ�\�B

    public List<Material> playerCursorMaterials;//�v���C���[�ɕt�^����}�e���A���B�C���X�y�N�^�[����A�^�b�`�K�{�B

    public List<Material> playerLockOnLineMaterial;//�v���C���[�̎g�p���郍�b�N�I�����C���̃}�e���A���B�C���X�y�N�^�[����A�^�b�`�K�{�B

    public List<Material> playerOverRayMaterial;//�v���C���[�̓��ߎ��̃}�e���A���B�C���X�y�N�^�[����A�^�b�`�K�{�B

    int depositJoinPlayer;//�ꎞ�I�ɎQ�����̃v���C���[�l�����擾����B����̓C���Q�[�����ɔ�ѓ���Q���𖳌��ɂ��邽�߂Ɏg�p�����B
    SceneTransitionManager scene_TM;

    //
    public int stageNumberInt = 1;//���[�h������X�e�[�W�ԍ��B1��Grid�A2��Concrete�B�Ȃ�.....�Ƃ��������ɂȂ��Ă���B
    void Start()
    {
        maxEntryJoinPlayerInt = 4;
        playerInputManager = GetComponent<PlayerInputManager>();
        windowManager = GetComponent<WindowManager>();
        scene_TM = GetComponent<SceneTransitionManager>();
    }

    public void OnPlayerJoined(PlayerInput playerInput)//�Ή������{�^���������ē��������ꍇ�B
    {
        print($"�v���C���[#{playerInput.user.index}�������I");

        int i = playerInput.user.index;
        joinPlayerInt++;

        GameObject joinPlayables = GameObject.FindWithTag(unPlayableSearchTag);//���o�^�v���C���[���p�̃^�O�Ō��m�A�擾����B

        if(maxEntryJoinPlayerInt >= (i + 1))
        {
            joinPlayables.tag = joinPlayableTag;
            joinPlayables.transform.parent = playersPrefab_Group.gameObject.transform;
            PlayablePrefabManager pm = joinPlayables.GetComponent<PlayablePrefabManager>();//���������v���C���[����PlayablePrefabManager���擾����B


            pm.PalletFrame_NumberTag_Text.text = joinPlayerInt.ToString();//�v���C���[�̔ԍ�������B
            pm.PCW_NumberTag_Image[playerInput.user.index].enabled = true;//�v���C���[�̔ԍ��ʂ��ImageObject��\������B
            pm.playable_CursorRing.GetComponent<SpriteRenderer>().material = playerCursorMaterials[playerInput.user.index];
            pm.playable_DireArrow.GetComponent<SpriteRenderer>().material = playerCursorMaterials[playerInput.user.index];
            pm.playableObject.GetComponent<LineRenderer>().material = playerLockOnLineMaterial[playerInput.user.index];


            pm.playableObject.GetComponent<PlayerPreparationInputDate>().pDM = GetComponent<PlayableDateManager>();
            PlayingData pd = pm.playableObject.GetComponent<PlayingData>();
            pd.playerNumber = playerInput.user.index;//�����ԍ����v���C���[Number�Ɋ��蓖�Ă�B
            pd.playerVisualColor = presetPlayerColor[playerInput.user.index];

            playableChara_OBJ.Add(pm.playableObject);//����L�������擾�B
            playable_CanvasOBJ = pm.canvasOBJ;
            playableChara_UI_PCW.Add(pm.ui_CustomWindow);//Ui_PlayerCastomWindow���擾�B
            playableChara_UI_GSW.Add(pm.ui_GeneralSelectWindow);//ui_GeneralSelectWindow���擾�B

            playableChara_UI_Pallet.Add(pm.ui_Pallet);//UI_Pallet���擾�B
            playableChara_UI_PalletFrame_Image.Add(pm.ui_Pallet_Frame);
            playableCharaUI_DestroyCover.Add(pm.ui_DestroyCover);
            playableChara_SM.Add(pm.playable_SM);//StatusManager���擾�B
            playeable_PD.Add(pm.playable_PD);//PlayingData���擾�B
            playableChara_PM.Add(pm.playable_PM);//PlayerMoving���擾�B
            playableChara_HCM.Add(pm.playable_HCM);//HandCardManager���擾�B
            playableChara_UI_Manager.Add(pm.playable_UI_Manager);//StatusUIManager���擾�B
            playableChara_PID.Add(pm.playable_PID);//PlayerPreparationInputDate���擾�B
            PlayeableChara_GSW.Add(pm.playable_GSW);//PlayerGSWInput���擾�B
            playeableChara_Adopt_Card.Add(pm.playeable_Adopt_Card);//PlayerAdopt_Card���擾�B


            windowManager.WindowSetUp(i);//Canvas�ɓ����Ă�UI�Q����̃L�����o�X�Ɋi�[����B


            joinPlayables = null;
            pm = null;
        }
        else//������ꍇ�B
        {
            joinPlayerInt--;

            Destroy(joinPlayables);//�T�[�`�A���h�f�X�g���C�b�b�B�T�[�`�A���h�f�X�g���C�b!!

            Debug.Log("��������𒴂��܂����B");
        }
    }

    // �v���C���[�ގ����Ɏ󂯎��ʒm(�{���g�p���Ȃ��@�\�ׁ̈A���e�s���m�B)
    public void OnPlayerLeft(PlayerInput playerInput)//�폜(������)���ꂽ�ꍇ�B
    {
        print($"�v���C���[#{playerInput.user.index}���ގ��I");
    }

    void PlayableDateInit()//�v���C�A�u���f�[�^�̏�����`�B
    {

    }

    public void ReadySW_detection()//�S����ReadySW��treu�ɂȂ��Ă��邩�𒲂ׂ�B
    {
        if(joinPlayerInt >= 2)
        {
            int t_Count = 0;

            for (int i = 0; i < playableChara_PID.Count; i++)
            {
                if (playableChara_PID[i].readySW == true)
                {
                    t_Count++;
                }
            }

            if (t_Count == playableChara_PID.Count)
            {
                Debug.Log("��������");
                windowManager.Snap_Corner();//�p���b�g���R�[�i�[�ɃX�i�b�v����B(�f���G�����ɂ͍��E�ݒu�B)

                for (int i = 0; i < playableChara_PID.Count; i++)
                {
                    playableChara_PID[i].PID_InputSafety = true;//PID���ׂẴZ���t�e�B���I���ɂ���B
                }

                scene_TM.loadStageNamber = stageNumberInt;//�X�e�[�WGrid�ɑJ�ڂ�����B
                scene_TM.StartLoad();//�V�[���J�ڂ��J�n����B
            }
        }
    }

    

    
}
