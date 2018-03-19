using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HitTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool IsActive { get; set; }

    [HideInInspector]
    public List<HitBox> HitBoxs;

    public int MAX_HITS = 5;

    //This is what you need to show in the inspector.
    public Side ObjectSide;

    // Use this for initialization
    void Start ()
	{
	    IsActive = false;
        HitBoxs = new List<HitBox>();
	}

    public void ClearBoxList()
    {
        HitBoxs.Clear();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
