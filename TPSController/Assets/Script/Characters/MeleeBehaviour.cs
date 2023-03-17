using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : GenericBehaviour
{
    public CharacterStates states { get; protected set; }

    [SerializeField]
    public string VerticalVelocity = "VerticalInput";
    public string HorizontalVelocity = "HorizontalInput";
    public string moveBool = "Moving";
    public string macheteStrafe = "MacheteStrafing";
    public string spearStrafe = "SpearStrafing";
    public string unarmedStrafe = "UnarmedStrafing";
    public string attackOneTrigger = "Attack1Trigger";
    public string attackTwoTrigger = "Attack2Trigger";
    public string attackThreeTrigger = "Attack3Trigger";
    public string attackFourTrigger = "Attack4Trigger";
    public string attackFiveTrigger = "Attack5Trigger";
    public string attackSixTrigger = "Attack6Trigger";
    public string attackSevenTrigger = "Attack7Trigger";
    public string attackEightTrigger = "Attack8Trigger";
    public string attackNineTrigger = "Attack9Trigger";
    public string attackTenTrigger = "Attack10Trigger";
    public string attackID = "AttackID";

    public bool MacheteStrafe;
    public bool SpearStrafe;
    public bool fistStrafe;

    [SerializeField]
    public Vector3 meleePivotOffset = new Vector3(0, 1, 0);
    public Vector3 meleeCamOffset = new Vector3(0.7f, 0.3f, -0.8f);
    public Vector2 vectorInput;
    public float lookDistance = 30.0f;
    public float turnSpeed;
    public float speed, direction;
    public bool meleeMode, spearMode, fistMode, canAttack;
    public int attackid, randMin, randMax;

    public Collider[] hitboxs;

    void Start(){
        Init();
    }

    void Init() {
        states = GetComponent<CharacterStates>();
    }

    void Update() {
        SetUpAnimator();
        AttackInput();
        Attack();
    }

    #region AnimatorSetup
    void SetUpAnimator(){
        if(WeaponHandler.Instance.axeMode && !meleeMode){
            StartCoroutine(ToggleMeleeOn());
        }
        else if(meleeMode && WeaponHandler.Instance.axeMode != true){
            StartCoroutine(ToggleMeleeOff());
        }

        if(WeaponHandler.Instance.fistMode && !fistMode){
            StartCoroutine(ToggleFistOn());
        }
        else if(fistMode && WeaponHandler.Instance.fistMode != true){
            StartCoroutine(ToggleFistOff());
        }


        if(fistStrafe){
            behaviourController.GetAnimator.SetBool(unarmedStrafe, fistStrafe);
        }
        else if(!fistStrafe){
            behaviourController.GetAnimator.SetBool(unarmedStrafe, fistStrafe);
        }


        if(MacheteStrafe){
            behaviourController.GetAnimator.SetBool(macheteStrafe, MacheteStrafe);
        }
        else if(!MacheteStrafe){
            behaviourController.GetAnimator.SetBool(macheteStrafe, MacheteStrafe);
        }


        if(SpearStrafe){
            behaviourController.GetAnimator.SetBool(spearStrafe, SpearStrafe);
        }
        else if(!SpearStrafe){
            behaviourController.GetAnimator.SetBool(spearStrafe, SpearStrafe);
        }
    }

    private IEnumerator ToggleMeleeOn() {
        yield return new WaitForSeconds(0.05f);

        if (behaviourController.GetTempLockStatus(this.behaviourCode) || behaviourController.IsOverriding(this))
            yield return false;

        else {
            meleeMode = true;
            MacheteStrafe = true;
            int signal = 1;
            meleeCamOffset.x = Mathf.Abs(meleeCamOffset.x) * signal;
            meleePivotOffset.x = Mathf.Abs(meleePivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(SpeedFloat, 0);
            behaviourController.OverrideWithBehaviour(this);
        }
    }

    private IEnumerator ToggleSpearOn(){
        yield return new WaitForSeconds(0.05f);

        if (behaviourController.GetTempLockStatus(this.behaviourCode) || behaviourController.IsOverriding(this))
            yield return false;

        else{

            spearMode = true;
            SpearStrafe = true;
            int signal = 1;
            meleeCamOffset.x = Mathf.Abs(meleeCamOffset.x) * signal;
            meleePivotOffset.x = Mathf.Abs(meleePivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(SpeedFloat, 0);
            behaviourController.OverrideWithBehaviour(this);
        }
    }

    private IEnumerator ToggleFistOn(){
        yield return new WaitForSeconds(0.05f);

        if (behaviourController.GetTempLockStatus(this.behaviourCode) || behaviourController.IsOverriding(this))
            yield return false;
        else{

            fistMode = true;
            fistStrafe = true;
            int signal = 1;
            meleeCamOffset.x = Mathf.Abs(meleeCamOffset.x) * signal;
            meleePivotOffset.x = Mathf.Abs(meleePivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(SpeedFloat, 0);
            behaviourController.OverrideWithBehaviour(this);
        }
    }

    private IEnumerator ToggleMeleeOff(){

        meleeMode = false;
        MacheteStrafe = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCameraRig.ResetTargetOffsets();
        behaviourController.GetCameraRig.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
        behaviourController.RevokeOverridingBehaviour(this);
    }

    private IEnumerator ToggleSpearOff(){

        spearMode = false;
        SpearStrafe = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCameraRig.ResetTargetOffsets();
        behaviourController.GetCameraRig.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
        behaviourController.RevokeOverridingBehaviour(this);
    }

    private IEnumerator ToggleFistOff(){

        yield return new WaitForSeconds(0.3f);
        fistMode = false;
        fistStrafe = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCameraRig.ResetTargetOffsets();
        behaviourController.GetCameraRig.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
        behaviourController.RevokeOverridingBehaviour(this);
    }

    void Animate(){
        var limitInput = 1f;
        var _input = vectorInput * limitInput;
        var _speed = Mathf.Clamp(_input.y, -limitInput, limitInput);
        var _direction = Mathf.Clamp(_input.x, -limitInput, limitInput);
        speed = _speed;
        direction = _direction;
        var newInput = new Vector2(speed, direction);

    }
    public override void LocalFixedUpdate() {
        if (WeaponHandler.Instance.axeMode || WeaponHandler.Instance.fistMode){
            behaviourController.GetCameraRig.SetTargetOffsets(meleePivotOffset, meleeCamOffset);
        }
        
        if(behaviourController.IsMoving()){
            Rotating();
        }
    }
    #endregion

    #region RotationHandler
    void Rotating(){
        Transform mainCamT = behaviourController.camTransform;
        Vector3 pivotPos = mainCamT.position;
        Vector3 lookTarget = pivotPos + (mainCamT.forward * lookDistance);
        Vector3 thisPos = transform.position;
        Vector3 lookdir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookdir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        transform.rotation = newRotation;
    }
    #endregion

    #region Attack Input And System
    void AttackInput(){
        //if(states.combat == CharacterStates.CombatStates.Unarmed){
        //    if(Input.GetMouseButtonDown(userInput.input.mouseButtonZero) && canAttack){
        //        RandAttack(randMin, randMax);
        //        canAttack = false;
        //    }
        //}
        //else if(states.combat == CharacterStates.CombatStates.Axe){
        //    if(Input.GetMouseButtonDown(userInput.input.mouseButtonZero) && canAttack){
        //        RandAttack(randMin, randMax);
        //        canAttack = false;
        //    }
        //}
        //if(states.combat == CharacterStates.CombatStates.Spear){
        //    if(Input.GetMouseButtonDown(userInput.input.mouseButtonZero) && canAttack){
        //        RandAttack(randMin, randMax);
        //        canAttack = false;
        //    }
        //}
    }
    void Attack(){
        //if(states.combat == CharacterStates.CombatStates.Unarmed){
        //    if(attackid == 1){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackOneTrigger);
        //        attackid = 0;
        //    }
        //    else if(attackid == 2){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackTwoTrigger);
        //        attackid = 0;
        //    }
        //    if(attackid == 3){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackThreeTrigger);
        //        attackid = 0;
        //    }
        //    else if(attackid == 4){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackFourTrigger);
        //        attackid = 0;
        //    }
        //}

        //else if(states.combat == CharacterStates.CombatStates.Axe){

        //    if(attackid == 5){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackFiveTrigger);
        //        attackid = 0;
        //    }
        //    else if (attackid == 6){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackSixTrigger);
        //        attackid = 0;
        //    }
        //    if(attackid == 7){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackSevenTrigger);
        //        attackid = 0;
        //    }
        //}

        //if(states.combat == CharacterStates.CombatStates.Spear){
        //    if(attackid == 8){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackEightTrigger);
        //        attackid = 0;
        //    }
        //    else if(attackid == 9){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackNineTrigger);
        //        attackid = 0;
        //    }
        //    if(attackid == 10){
        //        behaviourController.GetAnimator.SetInteger(attackID, attackid);
        //        behaviourController.GetAnimator.SetTrigger(attackTenTrigger);
        //        attackid = 0;
        //    }
        //}
    }

    void RandAttack(int min, int max){
        if(states.combat == CharacterStates.CombatStates.Unarmed){
            attackid = Random.Range(min, max);
        }
        else if(states.combat == CharacterStates.CombatStates.Armed && WeaponHandler.Instance.weaponType == WeaponHandler.WeaponType.AXE){
            attackid = Random.Range(5, 7);
        }
        if(states.combat == CharacterStates.CombatStates.Armed && WeaponHandler.Instance.weaponType == WeaponHandler.WeaponType.SPEAR){
            attackid = Random.Range(8, 10);
        }
    }

    public void ResetAttack(){
        canAttack = true;
    }
    #endregion
}