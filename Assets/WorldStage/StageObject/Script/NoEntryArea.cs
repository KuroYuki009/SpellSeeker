using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEntryArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<StatusManager>() != null)
        {
            Debug.Log("�Ăяo�����I");
            StatusManager otherSM = other.GetComponent<StatusManager>();
            otherSM.this_Eliminated = true;

            other.transform.position = new Vector3(0, 0, 0);//�ʒu�����Z�b�g�B
        }
    }
}
