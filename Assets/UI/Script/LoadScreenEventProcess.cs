using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadScreenEventProcess : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;

    public Image iconImage;//アイコンのイメージコンポ―ネント。

    public void EndCutInAnima()//アニメーションがカットインが終わり、待機状態になった。
    {
        sceneTransitionManager.StartingLoadProcess();//
    }

    public void EndCutOutAnima()//アニメーションがカットアウトが終わり、待機状態になった。
    {
        sceneTransitionManager.EndLoadProcess();
    }
    public void InvalidationObject()//このオブジェクトの無効化にする。
    {
        sceneTransitionManager.Invalidation();
    }

}
