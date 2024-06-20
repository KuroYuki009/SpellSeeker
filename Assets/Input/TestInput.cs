using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TestInput : MonoBehaviour
{
    InputAction move, look;
    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];
    }

    
    void Update()
    {
        var m = move.ReadValue<Vector2>();
        var l = look.ReadValue<Vector2>();

        Debug.Log($"Move {m}, Look {l}");
    }
}
