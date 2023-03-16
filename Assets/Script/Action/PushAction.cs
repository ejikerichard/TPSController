using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PushAction : MonoBehaviour{

    public Animator animator { get; protected set; }
    public Rigidbody mybody { get; protected set; }
    public UserInput userInput { get; protected set; }
    public CharacterStates states { get; protected set; }

    [SerializeField]
    public string pushBool = "IsPushing";
    public string pushTag = "PushAction";
    public bool isPushing, pushTriggered;

    public Transform pushTarget;

    void Start(){
        Init();
    }

    void Init(){
        animator = GetComponent<Animator>();
        mybody = GetComponent<Rigidbody>();
        userInput = GetComponent<UserInput>();
        states = GetComponent<CharacterStates>();
    }

    void FixedUpdate(){
        PushAnimation();
    }
    void PushAnimation(){
        if(Input.GetMouseButtonDown(userInput.input.mouseButtonZero) && pushTriggered && states.locomotions == CharacterStates.Locomotions.FreeLocomotion){
            isPushing = true;
            animator.SetBool(pushBool, isPushing);
            pushTarget.GetComponent<Rigidbody>().isKinematic = false;
            mybody.mass = 10000;
        }
        else if(Input.GetMouseButtonUp(userInput.input.mouseButtonZero) && pushTriggered && states.locomotions == CharacterStates.Locomotions.FreeLocomotion){
            isPushing = false;
            animator.SetBool(pushBool, isPushing);
            pushTarget.GetComponent<Rigidbody>().isKinematic = true;
            mybody.mass = 1;
        }
    }

    #region TriggerChecker
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag(pushTag)){
            var dist = Vector3.Distance(transform.forward, other.transform.forward);
            if(dist <= 0.8f){
                other.gameObject.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
                pushTarget = other.gameObject.transform.parent;
                pushTriggered = true;
            }
        }
    }
    void OnTriggerStay(Collider other){
        if(other.gameObject.CompareTag(pushTag)){
            var dist = Vector3.Distance(transform.forward, other.transform.forward);
            if(dist <= 0.8f){
                pushTarget = other.gameObject.transform.parent;
                pushTriggered = true;
            }
        }
    }
    void OnTriggerExit(Collider other){
        pushTriggered = false;
        //other.gameObject.transform.parent.GetComponent<Rigidbody>().isKinematic = false;
    }
    #endregion
}
