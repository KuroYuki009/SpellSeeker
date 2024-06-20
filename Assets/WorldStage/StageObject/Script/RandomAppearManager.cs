using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAppearManager : MonoBehaviour
{
    public GameObject targetObject;
    void Start()
    {
        targetObject.SetActive(false);

        int i = Random.Range(0, 8);//8ï™ÇÃ1Ç≈èoåªÇ∑ÇÈÅB
        if(i == 0)
        {
            targetObject.SetActive(true);
        }
    }
}
