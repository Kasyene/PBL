using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWeapon : MonoBehaviour
{
    private Vector3 basePosition;
    private int firstAttack = 0;
    private int secondAttack = 0;
    public int dmg = 2;

    // Use this for initialization
    void Start ()
    {
        basePosition = this.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
	{

    }

}
