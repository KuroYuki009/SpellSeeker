using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create SceneProfileDate")]
public class SceneProfileDate : ScriptableObject
{
    //シーン系
    //public string IDName;//検索時に入力する為の名前(半角限定)
    public string sceneVisName;//表示する際に使われる名前。

    public string sceneDateName;//シーンデータの名前を記載する必要がある。

    //モード系
    public string sceneGameMode;//このシーンに使用されるゲームモード。

    //詳細

}
