using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.GetComponent<HS_Attack_Trigger>() != null)
        {
            var temp = other.gameObject.GetComponent<HS_Attack_Trigger>();
            if (temp.GetAttackMode())
            {
                Debug.Log("Player has hit an Enemy");
                temp.SetAttackMode(false);
            }
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
