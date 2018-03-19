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
    public HitManager Manager;
    public static int numberOfClicks = 0;
    private  float lastClickedTime = 0.0f;
    private  float maxComboDelay = 1.5f;
    private float lastJumpTime = 0.0f;
    private bool turnBackInTime = false;
    public bool timeStop = false;





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
        //Debug.Log(animator.GetBool("basicAttack1") + " " + animator.GetBool("basicAttack2")
        //           +" " + animator.GetBool("basicAttack3") + " " + animator.GetBool("rangedAttack")
        //+" " + animator.GetBool("jumpAttack1") + " " + animator.GetBool("jumpAttack2"));
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
            lastJumpTime = Time.time;
            body.velocity += new Vector3(0f, 5.0f, 0f);
        }

        // basic attack + basic attack jump combo
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastJumpTime < 0.5f)
            {
                if (!IsAnyAttackBoolTrue())
                {
                    animator.SetBool("jumpAttack1", true);
                }
            }
            else
            {
                basicAttack();
            }
           
        }

        // ranged attack + ranged attack jump combo
        if (Input.GetMouseButtonDown(1) && !IsAnyAttackBoolTrue())
        {
            if (Time.time - lastJumpTime < 0.5f)
            {
                animator.SetBool("jumpAttack2", true);
            }
            else
            {
                animator.SetBool("rangedAttack", true);
            } 
        }


        // Attack system manager
        if (Manager != null)
        {
            
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

    bool IsAnyAttackBoolTrue()
    {
        if (animator.GetBool("basicAttack1") || animator.GetBool("basicAttack2")
            || animator.GetBool("basicAttack3") || animator.GetBool("rangedAttack")
            || animator.GetBool("jumpAttack1") || animator.GetBool("jumpAttack2"))
        {
            return true;
        }
        return false;
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
