using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScreenEventProcess : MonoBehaviour
{
    public InGameManager inGameManager;

    public void End_Anima()//
    {
        gameObject.SetActive(false);
    }

    public void IGM_BattleReadySE_Play()//�퓬�J�n�O�̌��ʉ�
    {
        inGameManager.BattleReadySE_Play();
    }
    public void IGM_BattleStartSE_Play()//�퓬�J�n�̌��ʉ�
    {
        inGameManager.BattleStartSE_Play();
    }
    public void IGM_BattleEndSE_Play()//�퓬�I���̌��ʉ�
    {
        inGameManager.BattleEndSE_Play();
    }

    public void End_StartBattle_AnimaEvent()//
    {
        inGameManager.StartBattlePhase();//
    }

    public void End_BreakEndBattle_AnimaEvent()//
    {
        inGameManager.Conflict_EndRoundProcess();//
    }
}
