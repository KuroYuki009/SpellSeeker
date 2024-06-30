using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create SpellDate")]
public class SpellData : ScriptableObject
{
    
    [Header("�X�y��")]//---
    [Tooltip("�J�[�hID�B���p����ł̓��́B")]
    public string IDName;//�������ɓ��͂���ׂ̖��O(���p����)

    [Tooltip("�J�[�h���B�Q�[�����ŕ\������ۂɎg�p����B")]
    public string spellName;//�`�ʂ���ۂɎg���閼�O�B

    [Space]

    [Tooltip("�C���X�^���X������I�u�W�F�N�g")]
    public GameObject spellPrefab;//�g�p���ɔ���������v���n�u�B

    [Space]

    [Header("�X�e�[�^�X")]//--
    [Tooltip("�J�[�h�� �g�p���� ������}�i�̗ʁB")]
    public int manaCost;//�J�[�h�̎g�p���ɏ��Ղ���}�i�̗ʁB

    [Tooltip("�J�[�h�� �j������ �񕜂���}�i�̗ʁB")]
    public int trashMana;//�g���b�V�������ۂɃX�e�[�^�X�ɗ^����}�i�̑����B(��{�I��+1�ɂȂ�B)

    [Tooltip("��ȍU���͒l�B")]
    public int primaryDamage;//��ȍU���́B

    [Tooltip("(�C��) ���I�ȍU���͒l�B�Q�Ƃ̎���ɂ��g�p�p�r���ω��B")]
    public int secondDamage;//���̍U���́B

    [Space]

    [Header("�\������")]//---
    [Tooltip("�J�[�h�� �T���l�C���Ƃ��� �\�������� �摜�f�[�^")]
    public Sprite imageSprite;//�\��������摜

    [Space]

    [Tooltip("����}�i�� �\���Ɏg���� �ϐ��B�����ȊO�����͉\")]
    public string manaCostString;//�\�����������}�i�B

    [Tooltip("�U���͂� �\���Ɏg���� �ϐ��B�����ȊO�����͉\�B")]
    public string damageString;//�\��������U���́B

    [Tooltip("(�C��) ���̎g�p�ɂ�蔭������U�����̕\���Ɏg���� �ϐ��B ")]
    public string attackOftenString;//�\��������U�����B�����̒e���΂��ۂɉ��������̕\���Ɏg�p����B

    [Space]
    
    [Tooltip("�g�p���̌��ʂ⋓������ ������"),TextArea]public string manualText;//�\��������������B

    [Space,Tooltip("(�C��)��������̕��́B�J�[�h�̒[�ɕ\�����ꂽ�肷��B")]
    public string commentText;//�Ȃɂ�������������.....?


    

    
    //extra
}
