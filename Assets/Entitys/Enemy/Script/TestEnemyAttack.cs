using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAttack : MonoBehaviour
{
    public GameObject atkPrefab;//攻撃に使用するプレバフを格納。

    public Transform instPos;//攻撃を発生させる位置。

    float atkDelayTime;//攻撃の感覚時間の指定。
    float elapsedTime;//経過時間を指す。
    void Start()
    {
        atkDelayTime = 1;
    }

    void Update()
    {
        if(elapsedTime <= atkDelayTime)
        {
            elapsedTime += 1 * Time.deltaTime;
        }
        else
        {
            GameObject instPrefab = Instantiate(atkPrefab, instPos.transform.position, instPos.transform.rotation);//カードデータを元にプレハブを生成する。

            instPrefab.GetComponent<SpellPrefabManager>().ownerObject = gameObject;//スペルプレハブマネージャーにオブジェクト情報を渡す。

            elapsedTime = 0;
        }
    }
}
