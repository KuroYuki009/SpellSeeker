using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayablePrefabManager : MonoBehaviour
{
    [Header("オブジェクト")]
    public GameObject playableObject;

    public GameObject canvasOBJ;

    [Space]
    [Space]

    //外的見目
    public GameObject playable_Skin_Head;//外見の頭
    public GameObject playable_Skin_Body;//外見の体

    [Space]

    //視覚部分
    public GameObject playable_CursorRing;
    public GameObject playable_DireArrow;


    [Header("各種 UI")]
    public GameObject ui_CustomWindow;

    public GameObject ui_GeneralSelectWindow;

    public GameObject ui_Pallet;

    public Image ui_Pallet_Frame;

    public Image ui_DestroyCover;

    [Space]
    //NumberSprite・Text関係。
    public Text PalletFrame_NumberTag_Text;

    public List<Image> PCW_NumberTag_Image;


    [Header("スクリプト")]
    public PlayingData playable_PD;//プレイングデータ。

    public StatusManager playable_SM;//ステータスマネージャー。

    public PlayerMoving playable_PM;//プレイヤーマネージャー。

    public HandCardManager playable_HCM;//ハンドカードマネージャー。

    public PlayerUIManager playable_UI_Manager;//プレイヤーUIマネージャー。

    public PlayerPreparationInputDate playable_PID;

    public PlayerGSWInput playable_GSW;

    public PlayerAdopt_Card playeable_Adopt_Card;


    void Awake()
    {
        // 各スクリプトのキャッシュをプレイアブルオブジェクトから取得する。
        playable_PD = playableObject.GetComponent<PlayingData>();

        playable_SM = playableObject.GetComponent<StatusManager>();

        playable_PM = playableObject.GetComponent<PlayerMoving>();

        playable_HCM = playableObject.GetComponent<HandCardManager>();

        playable_UI_Manager = playableObject.GetComponent<PlayerUIManager>();

        playable_PID = playableObject.GetComponent<PlayerPreparationInputDate>();

        playable_GSW = playableObject.GetComponent<PlayerGSWInput>();

        playeable_Adopt_Card = playableObject.GetComponent<PlayerAdopt_Card>();
    }
}
