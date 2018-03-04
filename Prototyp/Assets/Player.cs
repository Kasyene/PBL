using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody body;
    private float distanceToGround;
    private RaycastHit hit;
    float x;
    float z;
    float h;
    // Use this for initialization
    void Start ()
    {
        body = GetComponentInChildren<Rigidbody>();
        distanceToGround = GetComponentInChildren<CapsuleCollider>().bounds.extents.y;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0f, Input.GetAxis("Mouse X") * Time.deltaTime * 50f, 0f);
        transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 5f;
        transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 5f;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            body.velocity += new Vector3(0f, 5.0f, 0f);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(body.transform.position, -Vector3.up, distanceToGround + 0.1f);
    }
}
