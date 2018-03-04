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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "MainWeapon")
        {
            this.hp = this.hp - 1;
        }
    }
}
