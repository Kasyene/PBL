using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Pawn
{
    public Animator animator;
    protected Player player;
    public float range;
    public float wakeUpDistance;

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


}
