using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("jumpAttack1", false);
        animator.SetBool("jumpAttack2", false);
        animator.SetBool("basicAttack1", false);
        animator.SetBool("basicAttack2", false);
        animator.SetBool("basicAttack3", false);
        animator.SetBool("rangedAttack", false);

    }

}
