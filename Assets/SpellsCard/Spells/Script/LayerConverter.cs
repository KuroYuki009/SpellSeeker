using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerConverter : MonoBehaviour
{
    /*このスクリプトはスクリプトオブジェクトとモデルが別且つ親子付け関係であった場合にレイヤーを適応するためのスクリプトです。*/
    
    public GameObject parentDate;

    void Start()
    {
        if (parentDate == null) return;
        gameObject.layer = parentDate.layer;
    }
}
