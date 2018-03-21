using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceStat {

    public ResourceBar bar;
    private float maxValue;
    private float currentValue;

    public ResourceStat(float currentValue)
    {
        CurrentValue = currentValue;
    }

    public float MaxValue
    {
        get
        {
            return maxValue;
        }

        set
        {
            maxValue = value;
            bar.maxValue = maxValue;
        }
    }

    public float CurrentValue
    {
        get
        {
            return currentValue;
        }

        set
        {
            currentValue = value;
            bar.value = currentValue;
        }
    }


    public void Initialize()
    {
        this.MaxValue = maxValue;
        this.CurrentValue = currentValue;
    }
}
