using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create SpellDate")]
public class SpellData : ScriptableObject
{
    
    //���
    public string IDName;//�������ɓ��͂���ׂ̖��O(���p����)
    public string spellName;//�`�ʂ���ۂɎg���閼�O�B
    public Sprite imageSprite;//�\��������摜

    public string manaCostString;//�\�����������}�i�B
    public string damageString;//�\��������U���́B
    public string attackOftenString;//�\��������U�����B�����̒e���΂��ۂɉ��������̕\���Ɏg�p����B
    [TextArea]
    public string manualText;//�\��������������B
    public string commentText;//�Ȃɂ�������������.....?

    public GameObject spellPrefab;//�g�p���ɔ���������v���n�u�B

    //�X�e�[�^�X

    public int manaCost;//�J�[�h�̎g�p���ɏ��Ղ���}�i�̗ʁB
    public int trashMana;//�g���b�V�������ۂɃX�e�[�^�X�ɗ^����}�i�̑����B(��{�I��+1�ɂȂ�B)

    public int primaryDamage;//��ȍU���́B
    public int secondDamage;//���̍U���́B

    
    //extra
}
