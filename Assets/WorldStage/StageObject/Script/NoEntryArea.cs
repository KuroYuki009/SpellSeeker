using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEntryArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<StatusManager>() != null)
        {
            Debug.Log("呼び出した！");
            StatusManager otherSM = other.GetComponent<StatusManager>();
            otherSM.this_Eliminated = true;

            other.transform.position = new Vector3(0, 0, 0);//位置をリセット。
        }
    }
}
