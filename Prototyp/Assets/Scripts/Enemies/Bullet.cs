using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
      if (collider.tag == "Player")
      {
            Destroy(gameObject);
            collider.gameObject.GetComponentInChildren<Pawn>().Damage();
            Debug.Log("Bullet Hit!");
        }
    }
}
