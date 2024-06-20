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

    public void IGM_BattleReadySE_Play()//戦闘開始前の効果音
    {
        inGameManager.BattleReadySE_Play();
    }
    public void IGM_BattleStartSE_Play()//戦闘開始の効果音
    {
        inGameManager.BattleStartSE_Play();
    }
    public void IGM_BattleEndSE_Play()//戦闘終了の効果音
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
