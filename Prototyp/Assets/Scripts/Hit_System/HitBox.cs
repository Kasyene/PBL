using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.GetComponent<HitTrigger>() != null)
        {
            var temp = other.gameObject.GetComponent<HitTrigger>();
            if (temp.IsActive)
            {
                Debug.Log("Player has hit an Enemy");
                temp.IsActive = false;
            }
            else
            {
                //Debug.Log("TEST");
            }
        }

    }
}
