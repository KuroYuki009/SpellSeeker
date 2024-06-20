using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    //対象の親オブジェクトに付ける必要あり。
    StatusManager statusManager;
    int childInt;
    bool collapseSW;

    public List<GameObject> go;
    void Start()
    {
        statusManager = GetComponent<StatusManager>();

        childInt = gameObject.transform.childCount;//子オブジェクトの数を取得。
        for (int i = 0; i < childInt; i++)
        {
            Transform childTF = transform.transform.GetChild(i);
            go.Add(childTF.gameObject);//取得。
        }
    }
    private void Update()
    {
        if(statusManager.hitPoint <= 0 && collapseSW == false)
        {
            Collapse();
        }
        else if(collapseSW == true)
        {
            Destroy(gameObject,4);
        }
    }

    void Collapse()//
    {
        GetComponent<Collider>().enabled = false;//親オブジェクトの主なコライダーを無力化します。

        for (int i= 0;i < childInt;i++)
        {
            go[i].AddComponent<BoxCollider>();
            go[i].AddComponent<Rigidbody>();
            Destroy(go[i].gameObject, 2);
        }

        collapseSW = true;
    }

}
