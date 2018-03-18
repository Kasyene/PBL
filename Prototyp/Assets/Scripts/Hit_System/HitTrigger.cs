using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool IsActive { get; set; }

    

	// Use this for initialization
	void Start ()
	{
	    IsActive = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
