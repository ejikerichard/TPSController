﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class GenericAction : MonoBehaviour
{
    public UserInput userInput { get; protected set; }

    #region Variables
    //[Tooltip("Input to make the action")]
    ////public GenericInput actionInput = new GenericInput("E", "A", "A");
    [Tooltip("Tag of the object you want to access")]
    public string actionTag = "Action";
    [Tooltip("Use root motion of the animation")]
    public bool useRootMotion = true;

    [Header("--- Debug Only ---")]
    public TriggerGenericAction triggerAction;
    private Inventory inventory; 
    [Tooltip("Check this to enter the debug mode")]
    public bool debugMode;
    public bool canTriggerAction;
    public bool isPlayingAnimation;
    public bool triggerActionOnce;
    public bool Ismatching;

    public UnityEvent OnStartAction;
    public UnityEvent OnEndAction;

    protected BehaviourController behaviourController;


    #endregion

    protected virtual void Start(){
        behaviourController = GetComponent<BehaviourController>();
        userInput = GetComponent<UserInput>();
        inventory = GetComponent<Inventory>();
    }

    void Update(){
        TriggerActionInput();

    }
    protected virtual void TriggerActionInput(){

        if(triggerAction == null) return;

        if(canTriggerAction){
            if(triggerAction.autoAction && actionConditions || Input.GetMouseButtonDown(userInput.input.mouseButtonTwo) && actionConditions){
                if(!triggerActionOnce){
                    //OnDoAction.Invoke(triggerAction);
                    if(WeaponHandler.Instance.isAiming != true){
                        TriggerAnimation();

                        if(triggerAction.ProjectileWeapon && !triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().IsProjectileWeapon){
                            Weapon newItem = triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().item as Weapon;
                            newItem.prefab = triggerAction.transform.GetChild(0).gameObject;
                            inventory.AddItem(newItem);
                            triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().IsProjectileWeapon = true;
                        }else{

                            //Debug.Log("Picked a bow before");
                        }

                        if(triggerAction.MeleeWeapon && !triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().IsMeleeWeapon && inventory.weapons[0] == null |
                            triggerAction.MeleeWeapon && !triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().IsMeleeWeapon && inventory.weapons[1] == null |
                            triggerAction.MeleeWeapon && !triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().IsMeleeWeapon && inventory.weapons[2] == null){

                            Weapon newItem = triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().item as Weapon;
                            newItem.prefab = triggerAction.transform.GetChild(0).gameObject;
                            inventory.AddItem(newItem);
                            triggerAction.transform.GetChild(0).GetComponent<WeaponItemManager>().IsMeleeWeapon = true;
                            WeaponHandler.Instance.meleeSwitching = true;
                        }
                        else{
                            //Debug.Log("Picked a melee weapon before");
                        }
                    }
                    // destroy the triggerAction if checked with destroyAfter
                }
            }
        }
        AnimationBehaviour();

        //if (triggerAction == null){
        //    isPlayingAnimation = false;
        //}
    }

    public virtual bool actionConditions{

        get{

            return !(!canTriggerAction || isPlayingAnimation) && !behaviourController.animator.IsInTransition(0);
        }
    }

    protected virtual void TriggerAnimation(){
        if (debugMode) Debug.Log("TriggerAnimation");

        // trigger the animation behaviour & match target
        if(!string.IsNullOrEmpty(triggerAction.playAnimation)){
            isPlayingAnimation = true;
            behaviourController.animator.CrossFadeInFixedTime(triggerAction.playAnimation, 0.1f);    // trigger the action animation clip
        }

        // trigger OnDoAction Event, you can add a delay in the inspector   
        //StartCoroutine(triggerAction.OnDoActionDelay(gameObject));

        // bool to limit the autoAction run just once
        if(triggerAction.autoAction || triggerAction.destroyAfter) triggerActionOnce = true;

        if(triggerAction.destroyAfter){
            StartCoroutine(DestroyDelay(triggerAction));
        }
    }

    public virtual IEnumerator DestroyDelay(TriggerGenericAction triggerAction){
        var _triggerAction = triggerAction;
        yield return new WaitForSeconds(_triggerAction.destroyDelay);
        ResetPlayerSettings();
        Destroy(_triggerAction.gameObject);

        isPlayingAnimation = false;
    }

    protected virtual bool IsInForward(Transform target)
    {
        var dist = Vector3.Distance(transform.forward, target.forward);
        return dist <= 0.8f;
    }

    protected virtual void AnimationBehaviour(){

        if(playingAnimation){
            OnStartAction.Invoke();

            if(triggerAction.matchTarget != null && Ismatching){
                if (debugMode) Debug.Log("Match Target...");
                // use match target to match the y and z target 
                behaviourController.MatchTarget(triggerAction.matchTarget.transform.position, triggerAction.matchTarget.transform.rotation, triggerAction.avatarTarget,
                    new MatchTargetWeightMask(triggerAction.matchTargetMask, triggerAction.matchRot), triggerAction.startMatchTarget, triggerAction.endMatchTarget);
                if (triggerAction.disableCollision && triggerAction.disableGravity){
                    behaviourController.DisableGravityAndCollision();
                }
                else{
                    //Debug.Log("Don't disable");
                }
                //Debug.Log("Entry");
            }

            if(triggerAction.useTriggerRotation){
                if (debugMode) Debug.Log("Rotate to Target...");
                // smoothly rotate the character to the target
                transform.rotation = Quaternion.Lerp(transform.rotation, triggerAction.transform.rotation, behaviourController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            if(triggerAction.resetPlayerSettings && behaviourController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation){
                if (debugMode) Debug.Log("Finish Animation");
                // after playing the animation we reset some values
                ResetPlayerSettings();
            }
        }
    }
    protected virtual bool playingAnimation{
        get{

            if(triggerAction == null){

                isPlayingAnimation = false;
                return false;
            }

            if(!string.IsNullOrEmpty(triggerAction.playAnimation) && behaviourController.baseLayerInfo.IsName(triggerAction.playAnimation)){

                isPlayingAnimation = true;
                //if (triggerAction != null) triggerAction.OnPlayerExit.Invoke();
                if(triggerAction.disableCollision && triggerAction.disableGravity){
                    //behaviourController.DisableGravityAndCollision();
                }else{
                    //Debug.Log("Don't disable");
                }
            }
            else if(isPlayingAnimation && !string.IsNullOrEmpty(triggerAction.playAnimation) && !behaviourController.baseLayerInfo.IsName(triggerAction.playAnimation))
                isPlayingAnimation = false;

            return isPlayingAnimation;
        }
    }

    public void OnTriggerEnter(Collider other){

        if(other.gameObject.CompareTag(actionTag)){

            //if (triggerAction != null) triggerAction.OnPlayerEnter.Invoke();
        }
    }

    public void OnTriggerStay(Collider other){

        if(other.gameObject.CompareTag(actionTag) && !isPlayingAnimation){

            CheckForTriggerAction(other);
        }
    }

    public void OnTriggerExit(Collider other){

        if (other.gameObject.CompareTag(actionTag))
        {
            if (debugMode) Debug.Log("Exit vTriggerAction");
            //if (triggerAction != null) //triggerAction.OnPlayerExit.Invoke();
            ResetPlayerSettings();
        }
    }

    protected virtual void CheckForTriggerAction(Collider other){

        var _triggerAction = other.GetComponent<TriggerGenericAction>();
        if (!_triggerAction) return;

        //var dist = Vector3.Distance(transform.forward, _triggerAction.transform.forward);

        if(IsInForward(_triggerAction.transform)){

            triggerAction = _triggerAction;
            canTriggerAction = true;
            Ismatching = true;
            Debug.Log("matchtargeting");
            //triggerAction.OnPlayerEnter.Invoke();
        }else{

            //if (triggerAction != null) //triggerAction.OnPlayerExit.Invoke();
            canTriggerAction = false;
        }
    }

    protected virtual void ApplyPlayerSettings(){
        //if (debugMode) Debug.Log("ApplyPlayerSettings");

        //if (triggerAction.disableGravity)
        //{
        //    //character.mybody.useGravity = false;          // disable gravity of the player    
        //}
        //if (triggerAction.disableCollision)
        //    //character.capsuleCollider.isTrigger = true;          // disable the collision of the player if necessary 
    }

    protected virtual void ResetPlayerSettings(){
        if (debugMode) Debug.Log("Reset Player Settings");
        if (!playingAnimation || behaviourController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
        {
            behaviourController.EnableGravityAndCollision(0);           // enable again the gravity and collision
            Ismatching = false;
        }
    //OnEndAction.Invoke();
       canTriggerAction = false;
       triggerActionOnce = false;
    }
}
