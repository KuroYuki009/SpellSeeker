using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionManager : MonoBehaviour
{
    InGameManager gameModeManager;

    //�e���X�e�[�W���B
    public List<SceneProfileDate> stageProfileDate;//�o�����X�e�[�W�͊e�����悤�ɂ���B
    public int loadStageNamber;

    // ���[�h���ɕ\������UI�Q�B
    public GameObject loadingUI_Obj;
    LoadScreenEventProcess loadScreen_EP;
    Animator loadUI_Animator;

    // ���[�h�̐i���󋵂��Ǘ����邽�߂̕ϐ�
    private AsyncOperation async;

    void Start()
    {
        loadScreen_EP = loadingUI_Obj.GetComponent<LoadScreenEventProcess>(); //���[�h���̃A�j���[�V����UI�̐���B
        loadUI_Animator = loadingUI_Obj.GetComponent<Animator>();//���[�hUI�̃A�j���[�^�[�R�����擾�A�i�[�B
        loadScreen_EP.sceneTransitionManager = GetComponent<SceneTransitionManager>();//���̃X�N���v�g�̃p�X��n���B
        gameModeManager = GetComponent<InGameManager>();
    }

    // ���[�h���J�n���郁�\�b�h
    public void StartLoad()
    {
        // ���[�h��ʂ�\������
        loadingUI_Obj.SetActive(true);
        loadUI_Animator.SetTrigger("CutIn_Trigger");//�A�j���[�V�������J�b�g�C���X�e�[�g�Ɉړ��B
    }

    public void StartingLoadProcess()//UI�̃A�j���[�V�����C�x���g����Ăяo�����B
    {
        Debug.Log("�C�x���g���ǂݍ��܂ꂽ");
        StartCoroutine(Load());//�R���[�`�����g�p���ăV�[����ǂݍ��ށB
        loadUI_Animator.SetTrigger("LoadStandby_Trigger");//�A�j���[�V������ҋ@�X�e�[�g�Ɉړ��B
    }

    // �R���[�`�����g�p���ă��[�h�����s���郁�\�b�h
    private IEnumerator Load()
    {
        yield return new WaitForSeconds(0.2f);//0.2�b�ҋ@�B

        Debug.Log("���[�h�J�n");
        // �V�[����񓯊��Ń��[�h������
        async = SceneManager.LoadSceneAsync(stageProfileDate[loadStageNamber].sceneDateName);

        
        // ���[�h����������܂őҋ@����
        while (!async.isDone)
        {
            yield return null;
        }
        Debug.Log("���[�h�I��");

        Resources.UnloadUnusedAssets();// ���������������B

        EndLoad(); //���[�h�I����̏������s���B
    }

    public void EndLoad()//���[�h�I����̃��]�b�g�B
    {
        loadUI_Animator.SetTrigger("CutOut_Trigger");//�A�j���[�V�������J�b�g�A�E�g�X�e�[�g�Ɉړ��B
        //�����ŊO���֐M���𑗂�A���V�[��������v�f�𔭐�������B
        gameModeManager.currentSceneDate = stageProfileDate[loadStageNamber];//�Q�[�����[�h�}�l�[�W���Ɍ��݂̃V�[�����𗬂��B 

        gameModeManager.SceneEntryProcess();//�C���Q�[���}�l�[�W���[�ɐM���𑗂�B
    }

    public void EndLoadProcess()//���[�h�I����̏����B
    {
        
    }

    public void Invalidation()//���̃I�u�W�F�N�g�̖������B
    {
        // ���[�h��ʂ��\���ɂ���
        loadingUI_Obj.SetActive(false);
    }
}
