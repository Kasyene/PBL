using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Player : MonoBehaviour {

    private Rigidbody body;
    private float distanceToGround;
    private RaycastHit hit;
    private int hp = 10;
    public Animator animator;
    public HS_Attack_Trigger AttackTrigger;
    float x;
    float z;
    float h;
    

    

    // Use this for initialization
    void Start ()
    {
        body = GetComponentInChildren<Rigidbody>();
        distanceToGround = GetComponentInChildren<CapsuleCollider>().bounds.extents.y;
        animator = GetComponentInChildren<Animator>();
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

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }

        if (AttackTrigger != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack"))
            {
                AttackTrigger.SetAttackMode(true);
                //Debug.Log("ATTACK!");
            }
            else
            {
                AttackTrigger.SetAttackMode(false);
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(body.transform.position, -Vector3.up, distanceToGround + 0.1f);
    }
    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
