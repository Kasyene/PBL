using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void Movement()
    {
        transform.LookAt(player.transform.position);
        transform.position += transform.forward * Time.deltaTime * 5f;
    }


}
