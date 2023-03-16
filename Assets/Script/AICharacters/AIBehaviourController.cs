using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviourController : MonoBehaviour
{
   public enum GroundCheckMethod{

        Low, High
    }

    [SerializeField]
    public int currentBehaviour;
    public int defaultBehaviour;
    public int behaviourLocked;
    public List<AIGenericBehaviour> aIGenericBehaviours;
    public List<AIGenericBehaviour> overridingaIGenericBehaviours;
    public Rigidbody mybody;
    public Animator animator;
    public CapsuleCollider capsuleCollider;
    public NavMeshAgent agents;
    public GroundCheckMethod groundCheckMethod = GroundCheckMethod.High;

    public LayerMask groundLayer = 1 << 0;

    [SerializeField]
    internal float colliderRadius, colliderHeight;
    internal Vector3 colliderCenter;
    public bool isGrounded;
    internal PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;

    internal bool isRolling;
    internal bool customAction;

    public float groundDistance;
    protected float groundMinDistance = 0.25f;
    protected float groundMaxDistance = 0.5f;
    public float slopeLimit = 75f;
    public RaycastHit groundHit;

    internal AnimatorStateInfo baseLayerInfo, upperBodyInfo;

    int baseLayer { get { return animator.GetLayerIndex("Base Layer"); } }
    int upperBodyLayer { get { return animator.GetLayerIndex("UpperBody"); } }

    public Rigidbody GetRigidbody { get { return mybody; } }

    public Animator GetAnimator { get { return animator; } }

    public CapsuleCollider GetCapsuleCollider { get { return capsuleCollider; } }

    public NavMeshAgent GetNavMeshAgent { get { return agents; } }

    void Awake(){
        aIGenericBehaviours = new List<AIGenericBehaviour>();
        overridingaIGenericBehaviours = new List<AIGenericBehaviour>();
        animator = GetComponent<Animator>();
        mybody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        agents = GetComponent<NavMeshAgent>();
        agents.updateRotation = false;
        agents.updatePosition = false;

        frictionPhysics = new PhysicMaterial();
        frictionPhysics.name = "frictionPhysics";
        frictionPhysics.staticFriction = .25f;
        frictionPhysics.dynamicFriction = .25f;
        frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

        maxFrictionPhysics = new PhysicMaterial();
        maxFrictionPhysics.name = "maxFrictionPhysics";
        maxFrictionPhysics.staticFriction = 1f;
        maxFrictionPhysics.dynamicFriction = 1f;
        maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

        slippyPhysics = new PhysicMaterial();
        slippyPhysics.name = "slippyPhysics";
        slippyPhysics.staticFriction = 0f;
        slippyPhysics.dynamicFriction = 0f;
        slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

        colliderCenter = GetComponent<CapsuleCollider>().center;
        colliderHeight = GetComponent<CapsuleCollider>().height;
        colliderRadius = GetComponent<CapsuleCollider>().radius;

        Collider[] AllColliders = this.GetComponentsInChildren<Collider>();
        for(int i=0; i <AllColliders.Length; i++){
            Physics.IgnoreCollision(capsuleCollider, AllColliders[i]);
        }
    }

    void Update(){
        LayerControl();
        AirControl();
    }

    void FixedUpdate(){
        bool isAnyBehaviourActive = false;
        if(behaviourLocked > 0 || overridingaIGenericBehaviours.Count == 0){
            foreach(AIGenericBehaviour genericBehaviour in aIGenericBehaviours){

                isAnyBehaviourActive = true;
                genericBehaviour.LocalFixedUpdate();
            }
        }
        else
        {
            foreach(AIGenericBehaviour genericBehaviour in overridingaIGenericBehaviours){
                genericBehaviour.LocalFixedUpdate();
            }
        }

        if(!isAnyBehaviourActive && overridingaIGenericBehaviours.Count == 0){

            //mybody.useGravity = true;
        }

        CheckGround();
    }

    private void LateUpdate(){
        if(behaviourLocked > 0 || overridingaIGenericBehaviours.Count == 0){
            foreach(AIGenericBehaviour genericBehaviour in aIGenericBehaviours){
                genericBehaviour.LocalLateUpdate();
            }
        }

        else{
            foreach(AIGenericBehaviour genericBehaviour in overridingaIGenericBehaviours){
                genericBehaviour.LocalLateUpdate();
            }
        }

    }

    public void LayerControl(){
        baseLayerInfo = animator.GetCurrentAnimatorStateInfo(baseLayer);
        upperBodyInfo = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
    }

    public void AirControl(){
        customAction = IsAnimatorTag("CustomAction");

        if(customAction){
            Debug.Log("CustomAction");
        }
    }

    public void CheckGround(){

        CheckGroundDistance();

        if(customAction){
            isGrounded = true;
        }

        capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;


        bool checkGroundConditions = !isRolling;

        var magVel = (float)System.Math.Round(new Vector3(mybody.velocity.x, 0, mybody.velocity.z).magnitude, 2);
        magVel = Mathf.Clamp(magVel, 0, 1);

        var groundCheckDistance = groundMinDistance;
        if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

        if (checkGroundConditions){

            if(groundDistance <= 0.5f){
                isGrounded = true;
            }
            else{
                if(groundDistance >= groundCheckDistance){
                    isGrounded = false;
                }
            }
        }
    }

    public void CheckGroundDistance(){
        if(capsuleCollider != null){
            float radius = capsuleCollider.radius * 0.9f;
            var dist = 10f;

            Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);

            if (Physics.Raycast(ray2, out groundHit, colliderHeight / 2 + 2f, groundLayer))
                dist = transform.position.y - groundHit.point.y;

            if(groundCheckMethod == GroundCheckMethod.High){

                Vector3 pos = transform.position + Vector3.up * (capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if(Physics.SphereCast(ray, radius, out groundHit, capsuleCollider.radius + 2f, groundLayer)){

                    if (dist > (groundHit.distance - capsuleCollider.radius * 0.1f)) dist = (groundHit.distance - capsuleCollider.radius * 0.1f);
                    Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.1f, out groundHit, groundLayer);
                }
            }

            groundDistance = (float)System.Math.Round(dist, 2);
        }
    }

    public virtual float GroundAngle(){
        var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
        return groundAngle;
    }

    public void SubscribeBehaviour(AIGenericBehaviour behaviour){
        aIGenericBehaviours.Add(behaviour);
    }
    public void RegisterDefaultBehaiour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }

    public void RegisterBehaviour(int behaviourCode){
        if(currentBehaviour == behaviourCode){

            currentBehaviour = behaviourCode;
        }
    }

    public void UnregisterBehaviour(int behaviourCode){
        if(currentBehaviour == behaviourCode){

            currentBehaviour = defaultBehaviour;
        }
    }

    public bool OverrideWithBehaviour(AIGenericBehaviour behaviour){

        if (!overridingaIGenericBehaviours.Contains(behaviour))
        {
            if(overridingaIGenericBehaviours.Count == 0)
            {
                foreach(AIGenericBehaviour overriddenBehaviour in aIGenericBehaviours){
                    overriddenBehaviour.OnOverridde();
                    break;
                }
            }
            overridingaIGenericBehaviours.Add(behaviour);
            return false;
        }
        return false;
    }

    public bool RevokeOverridingBehaviour(AIGenericBehaviour behaviour){

        if(overridingaIGenericBehaviours.Contains(behaviour)){
            overridingaIGenericBehaviours.Remove(behaviour);
            return true;
        }
        return false;
    }

    public bool IsOverriding(AIGenericBehaviour behaviour = null){
        if (behaviour == null)
            return overridingaIGenericBehaviours.Count > 0;
        return overridingaIGenericBehaviours.Contains(behaviour);
    }

    public bool IsCurrentBehaviour(int behaviourCode){

        return this.currentBehaviour == behaviourCode;
    }

    public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0){

        return (behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf);
    }

    public void LockTempBehaviour(int behaviourCode){

        if(behaviourLocked == 0){
            behaviourLocked = 0;
        }
    }

    public virtual void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
    {
        if (animator.isMatchingTarget || animator.IsInTransition(0))
            return;

        float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

        if (normalizeTime > normalisedEndTime)
            return;

        animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
    }

    public void DisableGravityAndCollision(){
        animator.SetBool("Run", false);
        mybody.useGravity = false;
        capsuleCollider.isTrigger = true;
    }

    public void EnableGravityAndCollision(float normalizedTime){
        // enable collider and gravity at the end of the animation
        if(baseLayerInfo.normalizedTime >= normalizedTime){
            animator.SetBool("Run", true);
            capsuleCollider.isTrigger = false;
            mybody.useGravity = true;
        }
    }

    public bool IsAnimatorTag(string tag){
        if (animator == null) return false;
        if (baseLayerInfo.IsTag(tag)) return true;
        if (upperBodyInfo.IsTag(tag)) return true;

        return false;
    }
}


public abstract class AIGenericBehaviour : MonoBehaviour{

    protected AIBehaviourController aIBehaviourController;
    public int AIbehaviourCode;

    void Awake(){
        aIBehaviourController = GetComponent<AIBehaviourController>();
    }
    
    public virtual void LocalFixedUpdate(){ }

    public virtual void LocalLateUpdate() { }

    public virtual void OnOverridde() { }

    public int GetBehaviourCode()
    {
        return AIbehaviourCode;
    }
}
