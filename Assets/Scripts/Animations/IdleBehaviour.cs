using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo refactor this
public class IdleBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // PlayerController.instance.DeactivateAttackPoint(0);
        // PlayerController.instance.DeactivateAttackPoint(3);
        // PlayerController.instance.AttackDone();
        AlphabetController.instance.DeactivateAttackPoint(0);
        AlphabetController.instance.DeactivateAttackPoint(3);
        AlphabetController.instance.AttackDone();
        ArrowController.instance.DeactivateAttackPoint(0);
        ArrowController.instance.DeactivateAttackPoint(3);
        ArrowController.instance.AttackDone();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(ArrowController.instance.inputReceived && animator.name == "Arrow") {
            animator.SetTrigger("Attack1");
            ArrowController.instance.ActivateAttackPoint(1);
            ArrowController.instance.ChangeReceiveInputStatus();
            ArrowController.instance.inputReceived = false;
        } else if(AlphabetController.instance.inputReceived && animator.name == "Alphabet") {
            animator.SetTrigger("Attack1");
            // PlayerController.instance.ActivateAttackPoint(1);
            // PlayerController.instance.ChangeReceiveInputStatus();
            // PlayerController.instance.inputReceived = false;
            AlphabetController.instance.ActivateAttackPoint(1);
            AlphabetController.instance.ChangeReceiveInputStatus();
            AlphabetController.instance.inputReceived = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     PlayerController.instance.DeactivateAttackPoint(1);
    // }

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
