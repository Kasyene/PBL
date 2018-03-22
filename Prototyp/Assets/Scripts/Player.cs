using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngineInternal;
using Mathf = UnityEngine.Mathf;

public class Player : Pawn {

    private Rigidbody body;
    private float distanceToGround;
    private RaycastHit hit;
    public Animator animator;
    public HitManager Manager;
    public static int numberOfClicks = 0;
    private  float lastClickedTime = 0.0f;
    private  float maxComboDelay = 1.5f;
    private float lastJumpTime = 0.0f;
    private bool turnBackInTime = false;
    public bool timeStop = false;
    private int timeEnergy = 10;
    private bool timeEnergyRegeneration = false;
    private float timeToGameRestart = 3f;
    public GameObject ui_gameStatusInfo;


    public ResourceBar HPBar;
    public ResourceBar TimeBar;

    public List<Vector3> LastPositions;
    public List<int> lastHPs;


    // Use this for initialization
    void Start ()
    {
        Cursor.visible = false;
        body = GetComponentInChildren<Rigidbody>();
        distanceToGround = GetComponentInChildren<CapsuleCollider>().bounds.extents.y;
        animator = GetComponentInChildren<Animator>();
        HPBar.maxValue = hp;
        TimeBar.maxValue = 10f;

        

        for(int x = 0; x < 8; x++)
        {
            LastPositions.Add(transform.position);
            lastHPs.Add(hp);

        }
        StartCoroutine(SaveLastPos());

    }
	
	// Update is called once per frame
	void Update ()
    {
        HPBar.value = hp;
        TimeBar.value = timeEnergy;
        // if player dead
        if (this.hp <= 0)
        {
            timeToGameRestart -= Time.deltaTime;
            int timetoRestartDelta = (int)timeToGameRestart;

            if (Input.GetKeyDown(KeyCode.R))
            {
                timeToGameRestart = -1;
            }

            StartCoroutine(ShowMessage("Press \"r\" for restart. \n Auto restart in " + (timetoRestartDelta + 1)));

            if (timeToGameRestart < 0)
            {
                timeToGameRestart = 3;
                RestartGame();
            }
            else
            {
                return;
            }
        }
        else
        {
            // number of click for attack chain
            if ((Time.time - lastClickedTime > maxComboDelay))
            {
                numberOfClicks = 0;
            }


            // poruszanie i kamera
            transform.Rotate(0f, Input.GetAxis("Mouse X") * Time.deltaTime * 225f, 0f);
            transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 5f;
            transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 5f;

            // skok
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                lastJumpTime = Time.time;
                body.velocity += new Vector3(0f, 5.0f, 0f);
            }

            if (IsGrounded())
            {
                animator.SetBool("isGrounded", true);
            }
            else
            {
                animator.SetBool("isGrounded", false);
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

            //special abilities energy management
            if (timeEnergy == 0)
            {
                timeStop = false;
            }
            if (!timeStop && !timeEnergyRegeneration && timeEnergy < 10)
            {
                timeEnergyRegeneration = true;
                StartCoroutine(RegenerateTimeEnergy());
                
            }

            // special abilities
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (timeStop || timeEnergy == 0)
                {
                    timeStop = false;
                }
                else if (!timeStop && timeEnergy > 1)
                {
                    timeStop = true;
                    StartCoroutine(TimeIsStopped());
                }


            }

            if (Input.GetKeyDown(KeyCode.E) && timeEnergy >= 5)
                {
                timeEnergy -= 5;
                transform.position = LastPositions[LastPositions.Count/2];
                hp = lastHPs[lastHPs.Count/2];
                ReLocateIndicies(LastPositions);
                ReLocateIndicies(lastHPs);

            }



            // Attack system manager
            if (Manager != null)
            {

            }
        }
    }


    IEnumerator SaveLastPos()
    {
        while (true)
        {
            LastPositions.RemoveAt(0);
            LastPositions.Add(transform.position);
            lastHPs.RemoveAt(0);
            lastHPs.Add(hp);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator TimeIsStopped()
    {
       while (timeStop)
       {
           timeEnergyRegeneration = false;
           timeEnergy -= 1;
           yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator RegenerateTimeEnergy()
    {
        while (timeEnergyRegeneration && timeEnergy < 10)
        {
            timeEnergy += 1;
            yield return new WaitForSeconds(2f);
        }
        timeEnergyRegeneration = false;
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
        return Physics.Raycast(body.transform.position, -Vector3.up, distanceToGround + 0.01f);
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

    IEnumerator ShowMessage(string msg)
    {
        ui_gameStatusInfo.GetComponent<Text>().text = msg;
        ui_gameStatusInfo.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        ui_gameStatusInfo.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }


    private void ReLocateIndicies <T> (List<T> list)
    {
        for (int x = 0; x<= list.Count / 2 - 1; x++)
        {
            list[list.Count / 2 + x] = list[x];
        }
    }
}
