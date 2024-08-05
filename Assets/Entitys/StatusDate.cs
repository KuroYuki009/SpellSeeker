using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create StatusDate")]
public class StatusDate : ScriptableObject
{
    //���
    public int maxHitPoint;//�̗͂̍ő�l�B

    public int maxManaPoint;//�}�i�̍ő�l�B
    public float manaChargeWantTime;//���̃}�i�`���[�W�ɕK�v�Ȏ��ԁB

    public float maxSpeed;//�ő�ړ����x
    public float moveSpeed;//�ړ����x
    //extra
    public bool thisCommonEnemy;//���̃X�e�[�^�X�͈�ʂ̓G�̕����H(���ꂪ�I���ɂȂ����ꍇ�A�ꕔ��O�������O������̖��G���Ԃ������Ȃ�܂��B)
}
