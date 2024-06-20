using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WindowManager : MonoBehaviour
{
    public PlayableDateManager pbDM;
    public PlayerPreparationInputDate p_PID; //PlayerPreparationInputDate

    public RectTransform rect_MenuUI;//メニュー等のUIをまとめるRTF。
    public RectTransform rect_InGameUI;//ゲーム内等で使用するUIをまとめるRectTF。
    public RectTransform rect_PCWGroup;//カスタムウィンドウをまとめるRectTF。
    public RectTransform rect_GSWGroup;//汎用セレクトウィンドウをまとめるRectTF。

    public List<RectTransform> pcw_snapRTFs;//pcw用のスナップポイント。
    public List<RectTransform> gsw_snapRTFs;//pcw用のスナップポイント。

    public List<GameObject> fillSpriteObject;//JtPSprite。

　　public List<JoinToPushSpriteController> spriteController;//JtPSのスクリプトを格納。
    int stanbyCount;//待機しているPlayer番号。

    //プレイヤーのパレットUIをスナップする際に使用する。
    public List<RectTransform> snapPoint_Classic;
    public List<RectTransform> snapPoint_Corner;
    public List<RectTransform> snapPoint_Duel;

    //保管数値 
    bool joinSpOpenSW;
    float joinSpOpenElapseTime;
    void Start()
    {
        if (pbDM == null) pbDM = GetComponent<PlayableDateManager>();

        SetUp();
    }

    void Update()
    {
       
    }

    void SetUp()
    {
        int count = fillSpriteObject.Count;
        for (int i = 0; count > i; i++)
        {
            JoinToPushSpriteController ptpSC = fillSpriteObject[i].GetComponent<JoinToPushSpriteController>();
            ptpSC.wm = gameObject.GetComponent<WindowManager>();
            spriteController.Add(ptpSC);
            
        }
    }

    public void WindowSetUp(int number) //PlayableDateManagerで取得した情報を基礎にUI等を指定された場所に移動、ならびに格納を行う。
    {
        stanbyCount = number;//待機数値に入れる。

        RectTransform pcw_RT = pbDM.playableChara_UI_PCW[number].GetComponent<RectTransform>();//カスタムウィンドウの座標を取得。
        RectTransform gsw_RT = pbDM.playableChara_UI_GSW[number].GetComponent<RectTransform>();//汎用型セレクトウィンドウの座標を取得。
        pbDM.playableChara_UI_GSW[number].SetActive(false);//座標を取得した時点でそれ単体を無効化しておく。

        RectTransform pallet_RT = pbDM.playableChara_UI_Pallet[number].GetComponent<RectTransform>();//パレットUIの座標を取得。

        pcw_RT.transform.SetParent(rect_PCWGroup.transform,false);
        gsw_RT.transform.SetParent(rect_GSWGroup.transform, false);
        
        pallet_RT.transform.SetParent(rect_InGameUI.transform, false);

        pcw_RT.position = pcw_snapRTFs[number].position;//カスタムウィンドウの位置を変更。
        gsw_RT.position = gsw_snapRTFs[number].position;//汎用セレクトウィンドウの位置を変更。

        Destroy(pbDM.playable_CanvasOBJ);//カスタムウィンドウとパレットシステムUIが他キャンバスに移ったら元のキャンバスを削除する。

        spriteController[number].Join_FillAnima(number);

    }
    /*
    public void WindowLeaving(int Number)
    {
        spriteController[Number].Leaving_Window();
    }
    */

    public void PCW_Open(int Number)
    {
        pbDM.playableChara_UI_PCW[Number].GetComponent<Animator>().SetTrigger("OpenWindow_Trigger");
    }

    public void Snap_Corner()//角にスナップする。
    {
        if (pbDM.joinPlayerInt == 2 || pbDM.joinPlayerInt == 1)
        {
            for (int i = 0; i < pbDM.joinPlayerInt; i++)
            {
                RectTransform rect = pbDM.playableChara_UI_Pallet[i].GetComponent<RectTransform>();
                rect.pivot = snapPoint_Duel[i].pivot;
                rect.transform.position = snapPoint_Duel[i].transform.position;
            }
        }
        else
        {
            for (int i = 0; i < pbDM.joinPlayerInt; i++)
            {
                RectTransform rect = pbDM.playableChara_UI_Pallet[i].GetComponent<RectTransform>();
                rect.pivot = snapPoint_Corner[i].pivot;
                rect.transform.position = snapPoint_Corner[i].transform.position;
                
            }
        }

    }
}
