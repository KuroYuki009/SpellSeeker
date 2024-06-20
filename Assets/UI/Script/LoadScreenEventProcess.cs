using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadScreenEventProcess : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;

    public Image iconImage;//�A�C�R���̃C���[�W�R���|�\�l���g�B

    public void EndCutInAnima()//�A�j���[�V�������J�b�g�C�����I���A�ҋ@��ԂɂȂ����B
    {
        sceneTransitionManager.StartingLoadProcess();//
    }

    public void EndCutOutAnima()//�A�j���[�V�������J�b�g�A�E�g���I���A�ҋ@��ԂɂȂ����B
    {
        sceneTransitionManager.EndLoadProcess();
    }
    public void InvalidationObject()//���̃I�u�W�F�N�g�̖������ɂ���B
    {
        sceneTransitionManager.Invalidation();
    }

}
