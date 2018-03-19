﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int hp = 10;
    public Animator animator;
    protected Player player;
    public float range;

    // Use this for initialization
    protected void Start () {
	    player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
	
	// Update is called once per frame
	protected void Update () {
        CheckIfDead();
	    if (!player.timeStop)
	    {
	        EnemyBehaviour();
	    }
	}
    // Method for dealing damage to our enemy
    public void Damage()
    {
        hp -= 1;
    }

    protected virtual void EnemyBehaviour()
    {
        if (true)
        {
            Movement();
        }
        else
        {
            Attack();
        }
    }

    protected void CheckIfDead()
    {
        if (this.hp == 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Movement()
    {
        // to do
    }

    protected virtual void Attack()
    {
        // to do
    }

  /*  void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "MainWeapon")
        {
            this.hp = this.hp - collider.gameObject.GetComponent<MainWeapon>().dmg;
            Debug.Log("HIT");
        }
    }*/
}