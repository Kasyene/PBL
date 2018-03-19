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

    //DAMAGE METHOD GOES HERE!
    void Damage()
    {
        Debug.Log("Player has hit an Enemy");
        this.GetComponent<Enemy>().Damage();
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.GetComponent<HitTrigger>() != null)
        {
            var hitTrigger = other.gameObject.GetComponent<HitTrigger>();

            if (hitTrigger.IsActive && (hitTrigger.HitBoxs.Count < hitTrigger.MAX_HITS - 1))
            {
                bool isListed = false;

                foreach (var VARIABLE in hitTrigger.HitBoxs)
                {
                    if (VARIABLE.Equals(this))
                    {
                        isListed = true;
                    }
                }
                if (!isListed)
                {
                    hitTrigger.HitBoxs.Add(this);

                    // Do damage stuff
                    Damage();

                    if (hitTrigger.MAX_HITS - hitTrigger.HitBoxs.Count == 0)
                    {
                        hitTrigger.IsActive = false;
                    }
                  
                }
                
            }
            else
            {
                //Debug.Log("TEST");
            }
        }

    }
}
