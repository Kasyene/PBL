using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour {


    [SerializeField]
    private float fillAmount;

    [SerializeField]
    private Image fillImage;

    public float maxValue { get; set; }

    public float value
    {
        set
        {
            fillAmount = MapResourceToPossibleFillAmount(value, 0, maxValue, 0, 1);
        }

    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        HandleBar();
	}

    private void HandleBar()
    {
        if(fillAmount != fillImage.fillAmount)
        {
            fillImage.fillAmount = fillAmount;
        }
        
    }

    private float MapResourceToPossibleFillAmount(float value, float minValue, float maxValue, float minResult, float maxResult)
    {
        return (value - minValue) * (maxResult - minResult) / (maxValue - minValue) + minResult;
    }
}
