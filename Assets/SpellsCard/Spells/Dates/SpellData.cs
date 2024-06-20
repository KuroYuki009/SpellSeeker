using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create SpellDate")]
public class SpellData : ScriptableObject
{
    
    //情報
    public string IDName;//検索時に入力する為の名前(半角限定)
    public string spellName;//描写する際に使われる名前。
    public Sprite imageSprite;//表示させる画像

    public string manaCostString;//表示させる消費マナ。
    public string damageString;//表示させる攻撃力。
    public string attackOftenString;//表示させる攻撃数。複数の弾を飛ばす際に何発放つかの表示に使用する。
    [TextArea]
    public string manualText;//表示させる説明文。
    public string commentText;//なにか言いたい事は.....?

    public GameObject spellPrefab;//使用時に発生させるプレハブ。

    //ステータス

    public int manaCost;//カードの使用時に消耗するマナの量。
    public int trashMana;//トラッシュした際にステータスに与えるマナの増減。(基本的に+1になる。)

    public int primaryDamage;//主な攻撃力。
    public int secondDamage;//副の攻撃力。

    
    //extra
}
