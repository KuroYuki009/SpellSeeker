using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletSnapSystem : MonoBehaviour
{
    PlayableDateManager pDM;//Pallet���Q�Ƃ���ۂɎg�p�B

    public List<RectTransform> snapPoint_Classic;
    public List<RectTransform> snapPoint_Corner;
    public List<RectTransform> snapPoint_Duel;

    public bool sw;
    void Start()
    {
        pDM = GetComponent<PlayableDateManager>();
    }

    void Update()
    {
        if(sw == true)
        {
            Snap_Corner();
            sw = false;
        }
    }
    public void Snap_Classic()
    {
        Debug.Log("���ݒ�");
    }

    public void Snap_Corner()//
    {
        if(pDM.joinPlayerInt == 2  ||  pDM.joinPlayerInt==1)
        {
            for(int i = 0;i < pDM.joinPlayerInt;i++)
            {
                RectTransform rect = pDM.playableChara_UI_Pallet[i].GetComponent<RectTransform>();
                rect.pivot = snapPoint_Duel[i].pivot;
                rect.transform.position = snapPoint_Duel[i].transform.position;
            }
        }
        else
        {
            for(int i = 0;i < pDM.joinPlayerInt;i++)
            {
                pDM.playableChara_UI_Pallet[i].GetComponent<RectTransform>().transform.position = snapPoint_Corner[i].transform.position;
            }
        }
      
    }
}
