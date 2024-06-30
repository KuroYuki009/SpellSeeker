using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingData : MonoBehaviour
{
    [Header("��b���")]//---
    #region
    public GameObject playableObject;//���삷��v���C�A�u���I�u�W�F�N�g�B
    
    public int playerNumber;//�v���C���[�̑������B0~3��z��B

    public int teamNumber;//�v���C���[�̏����`�[���������B0~3��z��B

    [Space]
    
    public Color32 playerVisualColor;//�L�����J�[�\�����̃v���C���[��\���F��\������ׂɎg�p�����B
    #endregion


    [Header("�X�y�� �p���b�g")]//---
    #region
    public List<SpellData> onHandDeckDate;//���ݏ��L���Ă���J�[�h�f�[�^�Q�ō\�����ꂽ�f�b�L���w���B

    public string onHandSecondAction;//���ݐݒ肵�Ă���Z�J���h�A�N�V�������w���B
    #endregion
}