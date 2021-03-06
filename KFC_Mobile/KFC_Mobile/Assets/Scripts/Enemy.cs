﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Hittable
{
    GameObject player;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PC");
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
    }

    protected override void HitByArmor()
    {
        Destroy(gameObject);
    }
}
