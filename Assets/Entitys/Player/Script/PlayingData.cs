using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingData : MonoBehaviour
{
    public int playerNumber;//�v���C���[�̑������B0~3��z��B

    public int teamNumber;//�v���C���[�̏����`�[���������B0~3��z��B

    public GameObject playableObject;//���삷��v���C�A�u���I�u�W�F�N�g�B

    public Color32 playerVisualColor;//�L�����J�[�\�����̃v���C���[��\���F��\������ׂɎg�p�����B

    public List<SpellData> onHandDeckDate;//���ݏ��L���Ă���J�[�h�f�[�^�Q�ō\�����ꂽ�f�b�L���w���B

    public string onHandSecondAction;//���ݐݒ肵�Ă���Z�J���h�A�N�V�������w���B
}