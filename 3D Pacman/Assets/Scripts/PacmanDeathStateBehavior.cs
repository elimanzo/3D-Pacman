using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanDeathStateBehavior : StateMachineBehaviour
{
	private PacmanController pacmanController = null;
	private static int death = Animator.StringToHash("Death");
	
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		if(!pacmanController)
			pacmanController = GameObject.Find("Pacman").GetComponent<PacmanController>();
		
		if(stateInfo.shortNameHash == death) 
			animator.applyRootMotion = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		if(stateInfo.shortNameHash == death && stateInfo.normalizedTime > stateInfo.length) {
			pacmanController.Reset();
		}
			
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		if(stateInfo.shortNameHash == death) {
			animator.applyRootMotion = true;
		}
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
