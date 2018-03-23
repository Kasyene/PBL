using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject bulletPrefab;
    protected override void EnemyBehaviour()
    {
        base.EnemyBehaviour();
        if (distance < wakeUpDistance && heightDifference < 2.0f)
        {
            if (distance < range && distance > 5 && !animator.GetBool("isAttacking"))
            {
                Attack();
            }
            else if (distance > range || distance < 5)
            {
                Movement();
            }
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
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            new Vector3(this.transform.position.x, this.transform.position.y + 2.0f, this.transform.position.z),
            this.transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * range;

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 1.0f);

    }


}
