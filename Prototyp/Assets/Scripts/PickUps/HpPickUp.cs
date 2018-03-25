using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPickUp : MonoBehaviour
{

    public int hpValue = 3;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && collider.GetComponent<Player>().hp != collider.GetComponent<Player>().maxHpValue)
        {
            Destroy(gameObject);
        }
    }
}
