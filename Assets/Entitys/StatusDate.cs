using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create StatusDate")]
public class StatusDate : ScriptableObject
{
    //情報
    public int maxHitPoint;//体力の最大値。

    public int maxManaPoint;//マナの最大値。
    public float manaChargeWantTime;//一回のマナチャージに必要な時間。

    public float maxSpeed;//最大移動速度
    public float moveSpeed;//移動速度
    //extra
    public bool thisCommonEnemy;//このステータスは一般の敵の物か？(これがオンになった場合、一部例外を除き外部からの無敵時間が無くなります。)
}
