using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerConverter : MonoBehaviour
{
    /*���̃X�N���v�g�̓X�N���v�g�I�u�W�F�N�g�ƃ��f�����ʊ��e�q�t���֌W�ł������ꍇ�Ƀ��C���[��K�����邽�߂̃X�N���v�g�ł��B*/
    
    public GameObject parentDate;

    void Start()
    {
        if (parentDate == null) return;
        gameObject.layer = parentDate.layer;
    }
}
