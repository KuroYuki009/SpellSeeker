using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create PoolCardDate")]
public class PoolCardDate : ScriptableObject
{
    ////���

    public int useReferencePoolSize;//�g�p����v�[������ݒ肷��K�v������B

    public string poolName_1;//
    public int poolRateInt_1;//���I����銄�����w���B
    public List<SpellData> poolCardDate_1;//

    public string poolName_2;//
    public int poolRateInt_2;
    public List<SpellData> poolCardDate_2;//

    public string poolName_3;//
    public int poolRateInt_3;
    public List<SpellData> poolCardDate_3;//

    //extra
}
