using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallProjectile : Projectile
{
    private Vector3 startingScale;

    public float scaling;
    
    public override void Start()
    {
        base.Start();
        startingScale = transform.localScale;
    }

    public override void Update()
    {
        base.Update();

        Debug.Log(distance);
        transform.localScale = new Vector3(startingScale.x + distance * scaling, startingScale.y + distance * scaling, startingScale.z);
    }
}
