using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class AIGenericAction : MonoBehaviour {

    #region Variables
    //[Tooltip("Input to make the action")]
    ////public GenericInput actionInput = new GenericInput("E", "A", "A");
    [Tooltip("Tag of the object you want to access")]
    public string actionTag = "Action";
    [Tooltip("Use root motion of the animation")]
    public bool useRootMotion = true;
    public bool SetNaveMeshActive = false;

    [Header("--- Debug Only ---")]
    public TriggerGenericAction triggerAction;
    [Tooltip("Check this to enter the debug mode")]
    public bool debugMode;
    public bool canTriggerAction;
    public bool isPlayingAnimation;
    public bool triggerActionOnce;
    public bool Ismatching;

    private Vector3 matchTargetEndPos;
    private Vector3 matchTargetStartPos;
    private Quaternion matchTargetRot;

    public UnityEvent OnStartAction;
    public UnityEvent OnEndAction;

    protected AIBehaviourController behaviourController;

    #endregion

    protected virtual void Start(){
        behaviourController = GetComponent<AIBehaviourController>();
    }

    void Update(){
        TriggerActionInput();
        CheckOffMeshLink();
    }

    //void OnAnimatorMove()
    //{
    //    AnimationBehaviour();

    //    if (!playingAnimation) return;
    //    if (!character.customAction)
    //    {
    //        //enable movement using root motion
    //        transform.rotation = character.animator.rootRotation;
    //    }
    //    transform.position = character.animator.rootPosition;
    //}

    protected virtual void TriggerActionInput(){

        if(triggerAction == null) return;

        if(canTriggerAction){
            if((triggerAction.autoAction && actionConditions) || !triggerAction.autoAction && actionConditions){
                if(!triggerActionOnce){
                    //OnDoAction.Invoke(triggerAction);
                    TriggerAnimation();
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

    protected virtual void TriggerAnimation()
    {
        if (debugMode) Debug.Log("TriggerAnimation");

        // trigger the animation behaviour & match target
        if (!string.IsNullOrEmpty(triggerAction.playAnimation))
        {
            isPlayingAnimation = true;
            behaviourController.animator.CrossFadeInFixedTime(triggerAction.playAnimation, 0.1f);    // trigger the action animation clip
        }

        // trigger OnDoAction Event, you can add a delay in the inspector   
        //StartCoroutine(triggerAction.OnDoActionDelay(gameObject));

        // bool to limit the autoAction run just once
        if(triggerAction.autoAction || !triggerAction.autoAction || triggerAction.destroyAfter) triggerActionOnce = true;
       
        // destroy the triggerAction if checked with destroyAfter
        if (triggerAction.destroyAfter)
            StartCoroutine(DestroyDelay(triggerAction));
    }

    public virtual IEnumerator DestroyDelay(TriggerGenericAction triggerAction)
    {
        var _triggerAction = triggerAction;
        yield return new WaitForSeconds(_triggerAction.destroyDelay);
        ResetPlayerSettings();
        //Destroy(_triggerAction.gameObject);
        isPlayingAnimation = false;
    }

    protected virtual void AnimationBehaviour()
    {
        if (playingAnimation)
        {
            OnStartAction.Invoke();

            if (triggerAction.matchTarget != null && Ismatching)
            {
                if (debugMode) Debug.Log("Match Target...");
                // use match target to match the y and z target 
                behaviourController.MatchTarget(triggerAction.matchTarget.transform.position, triggerAction.matchTarget.transform.rotation, triggerAction.avatarTarget,
                    new MatchTargetWeightMask(triggerAction.matchTargetMask, 0), triggerAction.startMatchTarget, triggerAction.endMatchTarget);
                if (triggerAction.disableCollision && triggerAction.disableGravity){
                    behaviourController.DisableGravityAndCollision();
                }
                else{
                    Debug.Log("Don't disable");
                }
                Debug.Log("Entry");
            }

            if (triggerAction.useTriggerRotation)
            {
                if (debugMode) Debug.Log("Rotate to Target...");
                // smoothly rotate the character to the target
                transform.rotation = Quaternion.Lerp(transform.rotation, triggerAction.transform.rotation, behaviourController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            if (triggerAction.resetPlayerSettings && behaviourController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
            {
                if (debugMode) Debug.Log("Finish Animation");
                // after playing the animation we reset some values
                ResetPlayerSettings();
            }
        }
    }

    protected virtual bool playingAnimation
    {
        get
        {
            if (triggerAction == null)
            {
                isPlayingAnimation = false;
                return false;
            }

            if (!string.IsNullOrEmpty(triggerAction.playAnimation) && behaviourController.baseLayerInfo.IsName(triggerAction.playAnimation))
            {
                isPlayingAnimation = true;
                if (triggerAction != null) triggerAction.OnPlayerExit.Invoke();
                if(triggerAction.disableCollision && triggerAction.disableGravity){
                    //behaviourController.DisableGravityAndCollision();
                }else{
                    Debug.Log("Don't disable");
                }
            }
            else if (isPlayingAnimation && !string.IsNullOrEmpty(triggerAction.playAnimation) && !behaviourController.baseLayerInfo.IsName(triggerAction.playAnimation))
                isPlayingAnimation = false;

            return isPlayingAnimation;
        }
    }

    public void OnTriggerEnter(Collider other){

        if(other.gameObject.CompareTag(actionTag)){

            if (triggerAction != null) triggerAction.OnPlayerEnter.Invoke();
        }
    }

    public void OnTriggerStay(Collider other){

        if (other.gameObject.CompareTag(actionTag) && !isPlayingAnimation)
        {
            CheckForTriggerAction(other);
        }
    }

    public void OnTriggerExit(Collider other){

        if (other.gameObject.CompareTag(actionTag))
        {
            if (debugMode) Debug.Log("Exit vTriggerAction");
            if (triggerAction != null) triggerAction.OnPlayerExit.Invoke();
        }
    }

    protected virtual void CheckForTriggerAction(Collider other)
    {
        var _triggerAction = other.GetComponent<TriggerGenericAction>();
        if (!_triggerAction) return;

        var dist = Vector3.Distance(transform.forward, _triggerAction.transform.forward);

        if (!_triggerAction.activeFromForward || dist <= 0.8f)
        {
            triggerAction = _triggerAction;
            canTriggerAction = true;
            Ismatching = true;
            triggerAction.OnPlayerEnter.Invoke();
        }
        else
        {
            //if (triggerAction != null) triggerAction.OnPlayerExit.Invoke();
            //canTriggerAction = false;
        }
    }

    protected virtual void ApplyPlayerSettings()
    {
        //if (debugMode) Debug.Log("ApplyPlayerSettings");

        //if (triggerAction.disableGravity)
        //{
        //    //character.mybody.useGravity = false;          // disable gravity of the player    
        //}
        //if (triggerAction.disableCollision)
        //    //character.capsuleCollider.isTrigger = true;          // disable the collision of the player if necessary 
    }


    private void CheckOffMeshLink(){

        if(behaviourController.agents != null && behaviourController.agents.isOnOffMeshLink && behaviourController.agents.updatePosition){

            triggerAction.matchTarget.transform.position = behaviourController.agents.currentOffMeshLinkData.endPos;
            transform.position = behaviourController.agents.currentOffMeshLinkData.startPos;

            triggerAction.transform.rotation = Quaternion.LookRotation((triggerAction.matchTarget.transform.position + Vector3.up * (transform.position.y - triggerAction.matchTarget.transform.position.y) ) - transform.position);

            var link = behaviourController.agents.navMeshOwner as NavMeshLink;
            if(link && !behaviourController.animator.GetCurrentAnimatorStateInfo(0).IsTag( "CustomAction")){

                var linkCast = (NavMeshLink_TBS)link;
                string OffMeshLinkAnimName = linkCast.GetAnimName(transform.position);

                behaviourController.agents.ActivateCurrentOffMeshLink(false);
                behaviourController.agents.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                transform.rotation = triggerAction.transform.rotation;

                behaviourController.GetAnimator.Play(OffMeshLinkAnimName);
            }
        }
    }

    public void CompleteOffMeshLink(){

        triggerAction.matchTarget.transform.position = Vector3.zero;

        if(behaviourController.agents.enabled && behaviourController.agents.isOnNavMesh){
            behaviourController.agents.ActivateCurrentOffMeshLink(true);
            behaviourController.agents.CompleteOffMeshLink();
            behaviourController.agents.isStopped = false;

            behaviourController.agents.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
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
