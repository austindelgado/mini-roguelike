using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float speed;
    public float acceleration;
    public float deceleration;

    public Vector2 velocity;
    public Vector2 moveInput;
    public Vector2 lookDir;

    public virtual void Kill(){}
}
