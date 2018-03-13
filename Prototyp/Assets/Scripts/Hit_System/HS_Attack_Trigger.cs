using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_Attack_Trigger : MonoBehaviour {

    private bool isInAttackMode = false;

    public void SetAttackMode(bool state)
    {
        isInAttackMode = state;
    }

    public bool GetAttackMode()
    {
        return isInAttackMode;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
