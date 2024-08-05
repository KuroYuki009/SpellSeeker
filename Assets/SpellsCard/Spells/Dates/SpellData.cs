using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create SpellDate")]
public class SpellData : ScriptableObject
{
    
    [Header("スペル")]//---
    [Tooltip("カードID。半角限定での入力。")]
    public string IDName;//検索時に入力する為の名前(半角限定)

    [Tooltip("カード名。ゲーム内で表示する際に使用する。")]
    public string spellName;//描写する際に使われる名前。

    [Space]

    [Tooltip("インスタンス化するオブジェクト")]
    public GameObject spellPrefab;//使用時に発生させるプレハブ。

    [Space]

    [Header("ステータス")]//--
    [Tooltip("カードの 使用時に 消費されるマナの量。")]
    public int manaCost;//カードの使用時に消耗するマナの量。

    [Tooltip("カードの 破棄時に 回復するマナの量。")]
    public int trashMana;//トラッシュした際にステータスに与えるマナの増減。(基本的に+1になる。)

    [Tooltip("主な攻撃力値。")]
    public int primaryDamage;//主な攻撃力。

    [Tooltip("(任意) 副的な攻撃力値。参照の次第により使用用途が変化。")]
    public int secondDamage;//副の攻撃力。

    [Space]

    [Header("表示項目")]//---
    [Tooltip("カードの サムネイルとして 表示させる 画像データ")]
    public Sprite imageSprite;//表示させる画像

    [Space]

    [Tooltip("消費マナの 表示に使われる 変数。数字以外も入力可能")]
    public string manaCostString;//表示させる消費マナ。

    [Tooltip("攻撃力の 表示に使われる 変数。数字以外も入力可能。")]
    public string damageString;//表示させる攻撃力。

    [Tooltip("(任意) 一回の使用により発生する攻撃数の表示に使われる 変数。 ")]
    public string attackOftenString;//表示させる攻撃数。複数の弾を飛ばす際に何発放つかの表示に使用する。

    [Space]
    
    [Tooltip("使用時の効果や挙動等の 説明文"),TextArea]public string manualText;//表示させる説明文。

    [Space,Tooltip("(任意)何かしらの文章。カードの端に表示されたりする。")]
    public string commentText;//なにか言いたい事は.....?


    

    
    //extra
}
