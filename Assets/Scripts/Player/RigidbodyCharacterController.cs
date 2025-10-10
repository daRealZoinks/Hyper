using System;
using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    public Vector2 MoveInput { private get; set; }
    public event Action OnJumpPressed;
    public bool Sliding { private get; set; }
}
