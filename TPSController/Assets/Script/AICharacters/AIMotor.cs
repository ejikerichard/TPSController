using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMotor : AIGenericBehaviour {

    [SerializeField]
    public string isRun = "Run";
    public string groundBool = "IsGrounded";
    public string distanceFloat = "GroundDistance";

    [SerializeField]
    public float lookRadius, lookSpeed;
    public Transform target;

    void Start(){
        Init();
    }

    void Init(){
        aIBehaviourController.SubscribeBehaviour(this);
        aIBehaviourController.RegisterDefaultBehaiour(this.AIbehaviourCode);
        target = CharacterManager.instance.player.transform;
    }

    public override void LocalFixedUpdate(){
        ChasePlayer();
        GroundCheck();
    }
    public override void LocalLateUpdate(){
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    void ChasePlayer(){
        float dist = Vector3.Distance(target.position, transform.position);

        if(dist <= lookRadius){
            aIBehaviourController.GetNavMeshAgent.SetDestination(target.position);
            aIBehaviourController.GetAnimator.SetBool(isRun, true);
            Rotating();

            if (dist <=aIBehaviourController.GetNavMeshAgent.stoppingDistance){
                aIBehaviourController.GetAnimator.SetBool(isRun, false);
                Rotating();
            }
        }
    }

    void GroundCheck(){
        if(aIBehaviourController.customAction != true){
            aIBehaviourController.GetAnimator.SetFloat(distanceFloat, aIBehaviourController.groundDistance);
        }

        if(aIBehaviourController.isGrounded && aIBehaviourController.customAction != true){
            aIBehaviourController.GetAnimator.SetBool(groundBool, aIBehaviourController.isGrounded);
        }
        else if(!aIBehaviourController.isGrounded && aIBehaviourController.customAction != true){
            aIBehaviourController.GetAnimator.SetBool(groundBool, aIBehaviourController.isGrounded);
        }
    }

    void Rotating(){
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * lookSpeed);
    }
}
