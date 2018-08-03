using UnityEngine;
using Spanner;

[RequireComponent(typeof(JumpController2D))]
public class InputManager : MonoBehaviour {
    private JumpController2D _controller;

    private void Start() {
        _controller = GetComponent<JumpController2D>();
    }

    private void Update() {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        _controller.InputVector = new Vector2(x, y);
        if (Input.GetButtonDown("Jump")) {
            _controller.Jump();
        }
        _controller.JumpButtonHeld = Input.GetButton("Jump");
    }
}
