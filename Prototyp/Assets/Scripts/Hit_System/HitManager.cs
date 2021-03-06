﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side { Enemy, Player, Other };

public class HitManager : MonoBehaviour
{
    public HitTrigger HitTrigger;

    public bool GetAttackState()
    {
        return HitTrigger.IsActive;
    }

    public void StartAttack()
    {
        HitTrigger.IsActive = true;
    }

    public void StopAttack()
    {
        HitTrigger.IsActive = false;
        HitTrigger.ClearBoxList();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
