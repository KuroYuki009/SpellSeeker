using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayablePrefabManager : MonoBehaviour
{
    public GameObject playableObject;

    public GameObject canvasOBJ;

    public GameObject ui_CustomWindow;

    public GameObject ui_GeneralSelectWindow;

    public GameObject ui_Pallet;

    public Image ui_Pallet_Frame;

    public Image ui_DestroyCover;

    public PlayingData playable_PD;//プレイングデータ。

    public StatusManager playable_SM;//ステータスマネージャー。

    public PlayerMoving playable_PM;//プレイヤーマネージャー。

    public HandCardManager playable_HCM;//ハンドカードマネージャー。

    public PlayerUIManager playable_UI_Manager;//プレイヤーUIマネージャー。

    public PlayerPreparationInputDate playable_PID;

    public PlayerGSWInput playable_GSW;

    public PlayerAdopt_Card playeable_Adopt_Card;
    //NumberTag関係。
    public List<Image> PCW_NumberTag_Image;
    public Text PalletFrame_NumberTag_Text;

    //外見要素

    //キャラクター部分

    public GameObject playable_Skin_Head;//外見の頭
    public GameObject playable_Skin_Body;//外見の体

    //視覚部分
    public GameObject playable_CursorRing;
    public GameObject playable_DireArrow;

}
