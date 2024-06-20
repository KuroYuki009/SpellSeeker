using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    //�Ώۂ̐e�I�u�W�F�N�g�ɕt����K�v����B
    StatusManager statusManager;
    int childInt;
    bool collapseSW;

    public List<GameObject> go;
    void Start()
    {
        statusManager = GetComponent<StatusManager>();

        childInt = gameObject.transform.childCount;//�q�I�u�W�F�N�g�̐����擾�B
        for (int i = 0; i < childInt; i++)
        {
            Transform childTF = transform.transform.GetChild(i);
            go.Add(childTF.gameObject);//�擾�B
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
        GetComponent<Collider>().enabled = false;//�e�I�u�W�F�N�g�̎�ȃR���C�_�[�𖳗͉����܂��B

        for (int i= 0;i < childInt;i++)
        {
            go[i].AddComponent<BoxCollider>();
            go[i].AddComponent<Rigidbody>();
            Destroy(go[i].gameObject, 2);
        }

        collapseSW = true;
    }

}
