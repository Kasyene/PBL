using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void EnemyBehaviour()
    {
        base.EnemyBehaviour();
        if (distance < wakeUpDistance && heightDifference < 2.0f)
        {
            if (distance < range)
            {
                Attack();
            }
            else
            {
                Movement();
            }
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
