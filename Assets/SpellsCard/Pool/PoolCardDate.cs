using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create PoolCardDate")]
public class PoolCardDate : ScriptableObject
{
    ////情報

    public int useReferencePoolSize;//使用するプール数を設定する必要がある。

    public string poolName_1;//
    public int poolRateInt_1;//抽選される割合を指す。
    public List<SpellData> poolCardDate_1;//

    public string poolName_2;//
    public int poolRateInt_2;
    public List<SpellData> poolCardDate_2;//

    public string poolName_3;//
    public int poolRateInt_3;
    public List<SpellData> poolCardDate_3;//

    //extra
}
