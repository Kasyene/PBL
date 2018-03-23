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
    protected float distance;
    protected float heightDifference;

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
        Vector3 playerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerPosition);
        distance = Vector3.Distance(player.transform.position, transform.position);
        heightDifference = System.Math.Abs(player.transform.position.y - transform.position.y);
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
        // to override
    }

    protected virtual void Attack()
    {
        // to override
    }


}
