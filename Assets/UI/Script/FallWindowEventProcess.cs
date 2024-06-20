using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallWindowEventProcess : MonoBehaviour
{
    public PlayerGSWInput playerGSWInput;

    public void EndFallAnima()
    {
        playerGSWInput.HUD_Hidden();
    }
}
