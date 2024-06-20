using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickCard : MonoBehaviour
{
    //Ši”[—p
    public SpellData cardDate;

    [SerializeField]Sprite imageSprite;
    [SerializeField]string cardName;

    Image image;

    void Start()
    {
        CardRefresh();
    }

    public void CardRefresh()
    {
        imageSprite = cardDate.imageSprite;
        cardName = cardDate.spellName;

        image = GetComponent<Image>();
        image.sprite = imageSprite;
    }
}
