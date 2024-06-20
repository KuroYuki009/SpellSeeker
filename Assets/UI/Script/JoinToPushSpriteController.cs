using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinToPushSpriteController : MonoBehaviour
{
    Animator animator;
    public WindowManager wm;
    int typeNamber;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Join_FillAnima(int number)
    {
        typeNamber = number;
        animator.SetTrigger("Join_Trigger");
        Invoke("OpenWindow_Trigger", 1);
    }

    void OpenWindow_Trigger()
    {
        wm.PCW_Open(typeNamber);
    }

    /*
    public void Leaving_Window()//アニメーションを初期状態に戻す。
    {
        animator.SetTrigger("Leaving_Trigger");
    }
    */
}
