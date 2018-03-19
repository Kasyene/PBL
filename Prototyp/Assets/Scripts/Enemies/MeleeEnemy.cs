using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void EnemyBehaviour()
    {
        Vector3 playerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerPosition);
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance < range)
        {
            Attack();
        }
        else
        {
            Movement();
        }
    }

    protected override void Movement()
    {
        transform.position += transform.forward * Time.deltaTime * 4f;
    }

    protected override void Attack()
    {
        if(!animator.GetBool("isAttacking"))
        {
            animator.SetBool("isAttacking", true);
        }
    }


}
