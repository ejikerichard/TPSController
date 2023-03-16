using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackControl : StateMachineBehaviour {

    [SerializeField]
    public float startDamage = 0.05f;
    public float endDamage = 0.9f;

    [SerializeField]
    public bool isRightPunchOne, isRightPunchTwo, isRightPunchThree, isKick,
                MacheteSlash, SpearSlash;
    public bool isActive;

    public MeleeBehaviour meleeBehaviour;
   
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        meleeBehaviour = animator.GetComponent<MeleeBehaviour>();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        if(stateInfo.normalizedTime >= startDamage && isRightPunchOne && !isActive || stateInfo.normalizedTime >= startDamage && isRightPunchTwo && !isActive ||
           stateInfo.normalizedTime >= startDamage && isRightPunchThree && !isActive){

            meleeBehaviour.hitboxs[0].enabled = true;
            isActive = true;
            Debug.Log("StartDamage");
        }
        else if(stateInfo.normalizedTime >= endDamage && isRightPunchOne && isActive || stateInfo.normalizedTime >= endDamage && isRightPunchTwo && isActive ||
                stateInfo.normalizedTime >= endDamage && isRightPunchThree && isActive){
            
            meleeBehaviour.hitboxs[0].enabled = false;
            isActive = false;
            Debug.Log("EndDamage");
        }

        if(stateInfo.normalizedTime >= startDamage && isKick && !isActive){
            meleeBehaviour.hitboxs[2].enabled = true;
            isActive = true;
        }
        else if(stateInfo.normalizedTime >= endDamage && isKick && isActive){
            meleeBehaviour.hitboxs[2].enabled = false;
            isActive = false;
        }


        if(stateInfo.normalizedTime >= startDamage && MacheteSlash && !isActive){
            meleeBehaviour.hitboxs[4].enabled = true;
            isActive = true;
        }
        else if(stateInfo.normalizedTime >= endDamage && MacheteSlash && isActive){
            meleeBehaviour.hitboxs[4].enabled = false;
            isActive = false;
        }

        if(stateInfo.normalizedTime >= startDamage && SpearSlash && !isActive){
            meleeBehaviour.hitboxs[5].enabled = true;
            isActive = true;
        }
        else if(stateInfo.normalizedTime >= endDamage && SpearSlash && isActive){
            meleeBehaviour.hitboxs[5].enabled = false;
            isActive = false;
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        meleeBehaviour.hitboxs[0].enabled = false;
        meleeBehaviour.hitboxs[2].enabled = false;
        meleeBehaviour.hitboxs[4].enabled = false;
        meleeBehaviour.hitboxs[5].enabled = false;
        isActive = false;
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
