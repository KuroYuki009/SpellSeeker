using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusManager : MonoBehaviour
{
    //���̃X�N���v�g�̃Z���t�e�B(false�ŉ������)
    public bool SM_ProcessSafety;

    //// ��{�X�e�[�^�X���
    public int maxHitPoint;//�̗͂̍ő�l�B
    public int hitPoint;//���݂̗̑́B
    public bool hitPoint_Empty;//�̗͂����邩�ǂ����B
    public bool this_Eliminated;//���̃X�e�[�^�X�̎�����͊��S�ɓ|����Ă����Ԃ��H

    public int maxArmorPoint;//�A�[�}�[�V�[���h�̍ő�l�B
    public int armorPoint;//���݂̃A�[�}�[�V�[���h�B

    public int maxManaPoint;//�}�i�R�X�g�̍ő�l�B
    public int manaPoint;//���݂̃}�i�R�X�g�B
    public float manaWantTime;//���̃}�i�`���[�W�ɕK�v�Ȏ��ԁB
    public float manaProgressTime;//���̃}�i�`���[�W�̌o�ߎ��ԁB

    public float maxSpeed;//�ő�ړ����x
    public float moveSpeed;//�ړ����x
    public float moveMag = 1.0f;//�ړ����x�{���B

    //public int offensivePoint;//�U����//[�p�~]�P���ɕK�v�Ȃ��ׁB
    //public int defensePoint;//�h���//[�p�~]�P���ɕK�v�Ȃ��ׁB

    //�G�N�X�g��
    public bool thisCommonEnemy;//���̃X�e�[�^�X�̎�����͎G���G�ɊY�����邩�H(���ꂪ�I���ɂȂ����ꍇ�A�ꕔ��O�������O������̖��G���Ԃ������Ȃ�܂��B)

    ////�o�t�E�f�o�t�E��Ԍn�̕ϐ�

    public bool invincibleSt;//���G��Ԃ��H
    public float invincibleTime;//���G��Ԏ��ԁB

    public bool superArmorSt;//�X�[�p�[�A�[�}�[��Ԃ��B(�_���[�W�ɂ�鋯�݂������Ȃ�)

    public float slowTime;//�ړ����x�ቺ���ԁB

    public bool noMoveSt;//�ړ��s�\��Ԃ�?�B
    public float noMoveTime;//�ړ��s���ԁB

    public bool shockSt;//�V���b�N��Ԃ��H
    public float shockTime;//�V���b�N���ԁB

    public bool vulnerableSt;//�Ǝ��Ԃ�?�B
    float vulnerableMag = 1.5f;//�㏸�����_���[�W�{��
    public float vulnerableTime;//�Ǝ��Ԏ��ԁB

    //[�e�X�g�@�\]�q�b�g�l�v�Z_�ϐ�
    float takeDamege;
    float totalDamage;

    void Start()
    {
        manaWantTime = 1.5f;
        maxManaPoint = 5;
        maxSpeed = 15;//Default = 15
        moveSpeed = 15;//Default = 15
        hitPoint_Empty = false;
    }
    void Update()
    {
        if(SM_ProcessSafety == false)
        {
            ConditionCheck();//��Ԃ̌��o�B(�\��v�f // ��Ԉُ��t�^�����^�C�~���O�ŏ������N������悤�ɂ������������B)
            ManaCharge();
        }
    }

    // �q�b�g�|�C���g(HealthPoint) //-----------------------------------------------------------------------------------
    public void HP_Inflict_Damage(int damageInt)//�q�b�g�|�C���g�ɗ^������_���[�W�l�B
    {
        if(invincibleSt == false)
        {
            if (vulnerableSt == true)//�Ǝ��Ԃł���ꍇ�A�_���[�W��vulnerableMag�̐��l�Ŋ|����B
            {
                int calc = 0;
                float floatCalc = 0;
                floatCalc = (float)damageInt * vulnerableMag;//�v�Z�������l���i�[�B
                armorPoint -= (int)floatCalc;//�A�[�}�[�V�[���h�l���_���[�W�l�ō��B
                if (armorPoint < 0)//�����A�[�}�[�V�[���h�l��0�ȉ��ł����
                {
                    calc += armorPoint * -1;//���l�𐳓��l�ɂ��A���[�J��calc�ɓ����B
                    armorPoint = 0;//�A�[�}�[�l��0�ɂ���B
                    hitPoint -= calc;//�q�b�g�|�C���g�Ƀ��[�J��calc������B
                }

                if (hitPoint <= 0)
                {
                    hitPoint = 0;
                    hitPoint_Empty = true;
                }

                vulnerableTime = 0f;//�Ǝ��Ԃ̎��Ԃ��Ȃ����B
            }
            else//�Ǝ��ԂŖ�����Βʏ�̏������s���B
            {
                int calc = 0;
                armorPoint -= damageInt;//�A�[�}�[�V�[���h�l���_���[�W�l�ō��B
                if (armorPoint < 0)//�����A�[�}�[�V�[���h�l��0�ȉ��ł����
                {
                    calc += armorPoint * -1;//���l�𐳓��l�ɂ��A���[�J��calc�ɓ����B
                    armorPoint = 0;//�A�[�}�[�l��0�ɂ���B
                    hitPoint -= calc;//�q�b�g�|�C���g�Ƀ��[�J��calc������B
                }

                if (hitPoint <= 0)
                {
                    hitPoint = 0;
                    hitPoint_Empty = true;
                }
            }

            
            //[�e�X�g�@�\]�q�b�g�l�v�Z_����---------------------------------
            takeDamege = damageInt;
            totalDamage += takeDamege;
            Debug.Log("Inst : " + takeDamege + " | Total : " + totalDamage);
            //---------------------------------------------------------------

            if(hitPoint_Empty == true)//�̗͂��m���ɖ����Ȃ��Ă���ꍇ�A
            {
                if(hitPoint >= 0)hitPoint_Empty = false;
                this_Eliminated = true;
            }
            else if(this_Eliminated == true)
            {
                if(hitPoint >= 0)hitPoint_Empty = false;
            }
        }
    }

    public void HP_Inflict_Recovery(int recoveryInt)//�q�b�g�|�C���g�ɗ^������񕜒l�B
    {
        if(maxHitPoint >= hitPoint) hitPoint += recoveryInt;
    }

    // �}�i(Mana) //-----------------------------------------------------------------------------------

    public void Mana_Inflict_Expense(int costInt_L)//�}�i�|�C���g�̏���l�B
    {
        manaPoint -= costInt_L;
    }

    public void Mana_Inflict_Revenue(int revenueInt_L)//�}�i�|�C���g�̎擾�l�B
    {
        manaPoint += revenueInt_L;
    }

    // ���(StatusEffect) //-----------------------------------------------------------------------------------
    public void St_Inflict_Invincible(float invincibleTime_L)//�G����̍U����������Ȃ����G���ԁB
    {
        if(thisCommonEnemy == false)
        {
            invincibleTime = invincibleTime_L;
        }
    }


    public void St_Inflict_SlowNess(float slowNessTime_L)//�ړ����x�ቺ�̎��ԁB
    {
        if (slowTime <= slowNessTime_L) slowTime = slowNessTime_L;//�����ǉ����悤���Ă���^�C���̕����傫���ꍇ�A�������D�悳����B

    }

    public void St_Inflict_NoMove(float nomoveNessTime_L)//�ړ��s�̎��ԁB
    {
        noMoveTime = nomoveNessTime_L;
    }

    public void St_Inflict_Shock(float shockNessTime_L,int level)//�V���b�N��Ԃ̎��ԁB
    {
        if (level <= 1)//�󂯂��V���b�N��Ԃ̃��x����1��艺�ł���A�X�[�p�[�A�[�}�[�������ꍇ�͉e�����󂯂�B
        {
            if(superArmorSt == false)
            {
                noMoveTime = shockNessTime_L;
                shockTime = shockNessTime_L;
            }
        }
        else if (level >= 2)//�󂯂��V���b�N��Ԃ̃��x����2�ȏ�ł���΃X�[�p�[�A�[�}�[�̌��ʂ𖳌��ɂ���B
        {
            noMoveTime = shockNessTime_L;
            shockTime = shockNessTime_L;
        }
    }

    public void St_Inflict_Vulnerable(float vulnerableTime_L)//�Ǝ��Ԃ̎��ԁB
    {
        //�󂯂��Ǝ��Ԃ̎��Ԃ𑫂��B
        vulnerableTime += vulnerableTime_L;
    }

    void ManaCharge()//���Ԍo�߂ɂ��}�i�񕜁B
    {
        if (maxManaPoint > manaPoint) manaProgressTime += 1 * Time.deltaTime;
        else if(manaPoint > maxManaPoint) manaPoint = maxManaPoint;

        if (manaProgressTime >= manaWantTime)
        {
            manaPoint++;//�}�i��1�񕜁B
            manaProgressTime = 0;//���Ԍo�߂��������B
        }
        
    }

    void ConditionCheck()//��Ԃ̌��o���s���B----------------------------------------------------
    {
        float deltaTime = Time.deltaTime;//�f���^�^�C����������Ő��������A�n���B
        
        if(thisCommonEnemy == false)
        {
            //���G��ԁF�v���C���[���g���_���[�W����؎󂯕t���Ȃ��Ȃ�B
            if (invincibleTime >= 0.0f && invincibleTime != 0)
            {
                if (invincibleSt != true) invincibleSt = true;
                invincibleTime -= 1 * deltaTime;
            }
            else if (invincibleSt != false)
            {
                invincibleTime = 0;
                invincibleSt = false;
            }
        }
        

        //�ړ����x�ቺ�F�v���C���[�̈ړ����x���ቺ����B
        if (slowTime >= 0.0f && slowTime != 0)//slowTime��0�ȏ�̐��l�ɂȂ��Ă���ꍇ�A�ړ��ቺ�𔭐������Ȃ��玞�Ԃ����炷�B
        {
            if (moveMag != 0.7f) moveMag = 0.7f;//�ړ����x�ቺ
            slowTime -= 1 * deltaTime;
        }
        else if (moveMag != 1.0f)
        {
            slowTime = 0;
            moveMag = 1.0f;//�ړ����x�{�����P�ɖ߂��B
        }

        //�ړ��s�\�F�ړ����삪�o���Ȃ��Ȃ�B���_�͑���\�B
        if (noMoveTime >= 0.0f && noMoveTime != 0)
        {
            if (noMoveSt != true) noMoveSt = true;
            noMoveTime -= 1 * deltaTime;
        }
        else if (noMoveSt != false)
        {
            noMoveTime = 0;
            noMoveSt = false;
        }

        //�V���b�N��ԁF�̂������Ԃ��w���B���ݍs���Ă��铮����~�A�ړ��E���_�E�J�[�h�I�� �g�p�̑��삪�o���Ȃ��Ȃ�B
        if(shockTime >= 0.0f && shockTime != 0)
        {
            if (shockSt != true) shockSt = true;
            shockTime -= 1 * deltaTime;
        }
        else if(shockSt != false)
        {
            shockTime = 0;
            shockSt = false;
        }

        //�Ǝ��ԁF���Ɏ󂯂�_���[�W��1.5�{�ɂȂ�B��莞�ԗ����_���[�W���󂯂�Ɖ��������B
        if (vulnerableTime >= 0.0f && vulnerableTime != 0)
        {
            if (vulnerableSt != true) vulnerableSt = true;
            vulnerableTime -= 1 * deltaTime;
        }
        else if (vulnerableSt != false)
        {
            vulnerableTime = 0;
            vulnerableSt = false;
        }
    }
}
