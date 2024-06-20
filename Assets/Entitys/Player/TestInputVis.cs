using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TestInputVis : MonoBehaviour
{
    //���̃X�N���v�g�̓X�e�b�N���͂����������Ă��邩���m�F����ׂ̋@�\�ł��B


    InputAction move;
    public RectTransform ui_rtf;//[Test]InputImagecontroll���A�^�b�`�B
    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();//�{�̈ȊO����̓��͂œ����׈ꎞ�R�����g�A�E�g�B
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
            // Debug.Log("��");
        }
        else if (moveInput.y <= -0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(0, -80f, 0);
            // Debug.Log("��");
        }

        else if (moveInput.x >= 0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(80f, 0, 0);
            // Debug.Log("��");
        }
        else if (moveInput.x <= -0.75f)
        {
            
            ui_rtf.anchoredPosition = new Vector3(-80, 0, 0);
            // Debug.Log("�E");
        }
        else
        {
            ui_rtf.anchoredPosition = new Vector3(0, 0, 0) + new Vector3(80 * moveInput.x, 80 * moveInput.y, 0);
        }
    }


}
