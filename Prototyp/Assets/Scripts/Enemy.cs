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
        if (collider.tag == "MainWeapon")
        {
            this.hp = this.hp - collider.gameObject.GetComponent<MainWeapon>().dmg;
            Debug.Log("HIT");
        }
    }
}
