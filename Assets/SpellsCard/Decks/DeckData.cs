using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create DeckDate")]
public class DeckData : ScriptableObject
{
    public string deckName;//デッキの名前。

    public List<SpellData> spells;//デッキ

}
