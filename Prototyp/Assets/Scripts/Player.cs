using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngineInternal;
using Mathf = UnityEngine.Mathf;

public class Player : MonoBehaviour {

    private Rigidbody body;
    private float distanceToGround;
    private RaycastHit hit;
    private int hp = 10;
    public Animator animator;
    public HS_Attack_Trigger AttackTrigger;
    public static int numberOfClicks = 0;
    private  float lastClickedTime = 0.0f;
    private  float maxComboDelay = 1.5f;
    private float minComboDelay = 0.2f;
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
        if ((Time.time - lastClickedTime > maxComboDelay))
        {
            numberOfClicks = 0;
        }


        // poruszanie i kamera
        transform.Rotate(0f, Input.GetAxis("Mouse X") * Time.deltaTime * 50f, 0f);
        transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 5f;
        transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 5f;

        // skok
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            body.velocity += new Vector3(0f, 5.0f, 0f);
        }

        // ataki
        if (Input.GetMouseButtonDown(0))
        {
            basicAttack();
        }

        if (Input.GetMouseButtonDown(1) && !IsAnyAttackBoolTrue())
        {
            animator.SetBool("rangedAttack", true);
        }


        //triggery ataków
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

    void basicAttack()
    {
        lastClickedTime = Time.time;
        numberOfClicks++;

        if (numberOfClicks == 1)
        {
            animator.SetBool("basicAttack1", true);
        }
        numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
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

    bool IsAnyAttackBoolTrue()
    {
        if (animator.GetBool("basicAttack1") || animator.GetBool("basicAttack2")
            || animator.GetBool("basicAttack3") || animator.GetBool("rangedAttack"))
        {
            return true;
        }
        return false;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
