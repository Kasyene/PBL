using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void EnemyBehaviour()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance < 3)
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
        Vector3 playerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerPosition);
        transform.position += transform.forward * Time.deltaTime * 4f;
    }

    protected override void Attack()
    {
        animator.SetBool("isAttacking", true);
    }


}
