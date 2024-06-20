using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TestInputVis : MonoBehaviour
{
    //このスクリプトはステック入力が正しく取れているかを確認する為の機能です。


    InputAction move;
    public RectTransform ui_rtf;//[Test]InputImagecontrollをアタッチ。
    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();//本体以外からの入力で動く為一時コメントアウト。
        move = playerInput.actions["Move"];
    }

    // Update is called once per frame
    void Update()
    {
        InputCursor();
    }

    void InputCursor()
    {
        Vector2 moveVC2 = move.ReadValue<Vector2>();
        Vector3 moveInput = new Vector3(moveVC2.x, moveVC2.y, 0);
        if (moveInput.y >= 0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(0, 80f, 0);
            // Debug.Log("上");
        }
        else if (moveInput.y <= -0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(0, -80f, 0);
            // Debug.Log("下");
        }

        else if (moveInput.x >= 0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(80f, 0, 0);
            // Debug.Log("左");
        }
        else if (moveInput.x <= -0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(-80, 0, 0);
            // Debug.Log("右");
        }
        else
        {
            ui_rtf.anchoredPosition = new Vector3(0, 0, 0) + new Vector3(80 * moveInput.x, 80 * moveInput.y, 0);
        }
    }


}
