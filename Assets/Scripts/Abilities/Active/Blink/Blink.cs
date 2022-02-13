using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Blink : Ability
{
    public float distance;

    public override void Activate(GameObject parent, int level)
    {
        if (Vector2.Distance(parent.GetComponent<Player>().mouseInput, (Vector2)parent.transform.position) < distance)
            parent.transform.position = parent.GetComponent<Player>().mouseInput;
        else
            parent.transform.position = (Vector2)parent.transform.position + distance * parent.GetComponent<Entity>().lookDir;
    }
}
