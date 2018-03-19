using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    private float distance;
    protected override void EnemyBehaviour()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        Vector3 playerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerPosition);
        if (distance < range && distance > 5 && !animator.GetBool("isAttacking"))
        {
            Attack();
        }
        else if (distance > range || distance < 5)
        {
            Movement();
        }
    }

    protected override void Movement()
    {
        if (distance > range)
        {
            transform.position += transform.forward * Time.deltaTime * 4f;
        }
        else if (distance < 5)
        {
            transform.position += -transform.forward * Time.deltaTime * 4f;
        }

    }

    protected override void Attack()
    {
        animator.SetBool("isAttacking", true);
    }


}
