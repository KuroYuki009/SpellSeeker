using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingData : MonoBehaviour
{
    [Header("基礎情報")]//---
    #region
    public GameObject playableObject;//操作するプレイアブルオブジェクト。
    
    public int playerNumber;//プレイヤーの属性数。0~3を想定。

    public int teamNumber;//プレイヤーの所属チーム属性数。0~3を想定。

    [Space]
    
    public Color32 playerVisualColor;//キャラカーソル等のプレイヤーを表す色を表示する為に使用される。
    #endregion


    [Header("スペル パレット")]//---
    #region
    public List<SpellData> onHandDeckDate;//現在所有しているカードデータ群で構成されたデッキを指す。

    public string onHandSecondAction;//現在設定しているセカンドアクションを指す。
    #endregion
}