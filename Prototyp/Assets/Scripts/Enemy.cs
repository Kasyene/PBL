using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


	    if (this.hp == 0)
	    {
            Destroy(gameObject);
	    }
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "MainWeapon" && (collider.gameObject.GetComponentInChildren<MainWeapon>().firstAttack == 1 
                                             || collider.gameObject.GetComponentInChildren<MainWeapon>().firstAttack == 2))
        {
            this.hp = this.hp - collider.gameObject.GetComponentInChildren<MainWeapon>().dmg;
            Debug.Log("HIT");
        }
    }
}
