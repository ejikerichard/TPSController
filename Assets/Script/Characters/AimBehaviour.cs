using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBehaviour : GenericBehaviour{
        public UserInput userInput { get; protected set; }

        [SerializeField]
        public string VerticalVelocity = "VerticalInput";
        public string HorizontalVelocity = "HorizontalInput";
        public string strafeBool = "IsStrafing";
        public string crouchBool = "IsCrouching";
        public string moveBool = "Moving";
        public string crouchTag = "AutoCrouch";
        public bool isStrafing;
        public bool SpearStrafing;
        public bool isCrouching;

        [SerializeField]
        public float aimTurnSmoothing = 0.15f;
        public Vector3 aimPivotOffset = new Vector3(0, 1, 0);
        public Vector3 aimCamOffset = new Vector3(0.52f, 0.55f, -0.89f);
        public Vector3 crouchaimCamOffset = new Vector3(0.5f, 0.2f, -1.1f);

        public Transform lockTarget;

        public bool aim;
        public float speedDamp;

        Dictionary<Weapon, GameObject> crosshairPrefabMap = new Dictionary<Weapon, GameObject>();

        void Start(){
            Init();
        }

        void Init(){
            userInput = GetComponent<UserInput>();
            SetupCrosshairs();
        }
        void SetupCrosshairs(){
            //foreach(Weapon wep in weaponHandler.weapons){
            //GameObject prefab = wep.weaponSettings.crosshairPrefab;
            //if (prefab != null){

            //    GameObject clone = (GameObject)Instantiate(prefab);
            //    crosshairPrefabMap.Add(wep, clone);
            //}
        }
    void Update(){

        if(WeaponHandler.Instance.isAiming && !aim){
            StartCoroutine(ToggleAimOn());
        }
        else if(aim && WeaponHandler.Instance.isAiming != true){
            StartCoroutine(ToggleAimOff());
        }

        if (isStrafing){
            behaviourController.GetAnimator.SetBool(strafeBool, isStrafing);
        }
        else if(!isStrafing){
            behaviourController.GetAnimator.SetBool(strafeBool, isStrafing);
        }

        if(aim){
            if(isCrouching){
                behaviourController.GetAnimator.SetBool(crouchBool, isCrouching);
                behaviourController.GetCameraRig.SetTargetOffsets(aimPivotOffset, crouchaimCamOffset);
            }
            else if (!isCrouching){
                behaviourController.GetAnimator.SetBool(crouchBool, isCrouching);
                behaviourController.GetCameraRig.SetTargetOffsets(aimPivotOffset, aimCamOffset);
            }
        }
    }
    void FixedUpdate(){
        Animate(behaviourController.GetV, behaviourController.GetH);
        MoveController(behaviourController.GetV, behaviourController.GetH);
    }
    public override void LocalLateUpdate(){

        AimManagement();
        if(aim){
            userInput.OnLateUpdate.Invoke();
        }
    }

    // Handle aim parameters when aiming is active.
    void AimManagement(){
        // Deal with the player orientation when aiming.
        Rotating();
    }

    void Animate(float forward, float strafe){

        behaviourController.GetAnimator.SetFloat(VerticalVelocity, forward);
        behaviourController.GetAnimator.SetFloat(HorizontalVelocity, strafe);

    }

    void MoveController(float vertical, float horizontal){
        if(vertical > 0 || horizontal > 0){
            behaviourController.GetAnimator.SetBool(moveBool, true);
        }
        else if(vertical < 0 || horizontal < 0){
            behaviourController.GetAnimator.SetBool(moveBool, true);
        }else{
            behaviourController.GetAnimator.SetBool(moveBool, false);
        }
    }

    private IEnumerator ToggleAimOn(){
        yield return new WaitForSeconds(0.05f);
        // Aiming is not possible.
        if (behaviourController.GetTempLockStatus(this.behaviourCode) || behaviourController.IsOverriding(this))
            yield return false;

        // Start aiming.
        else{
            aim = true;
            isStrafing = true;
            int signal = 1;
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(SpeedFloat, 0);
            // This state overrides the active one.
            behaviourController.OverrideWithBehaviour(this);
        }
    }

    private IEnumerator ToggleAimOff(){
        aim = false;
        isStrafing = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCameraRig.ResetTargetOffsets();
        behaviourController.GetCameraRig.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
        behaviourController.RevokeOverridingBehaviour(this);
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate(){
        // Set camera position and orientation to the aim mode parameters.
        if (WeaponHandler.Instance.aim)
            behaviourController.GetCameraRig.SetTargetOffsets(aimPivotOffset, aimCamOffset);
    }

    void Rotating(){
        Vector3 forward = behaviourController.camTransform.TransformDirection(Vector3.forward);
        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Always rotates the player according to the camera horizontal rotation in aim mode.
        Quaternion targetRotation = Quaternion.Euler(0, behaviourController.GetCameraRig.GetH, 0);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        // Rotate entire player to face camera.
        behaviourController.SetLastDirection(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);
    }

    void PositionCrosshair(Ray ray, Weapon wep){
        //Weapon curWeapon = weaponHandler.currentWeapon;
        //if (curWeapon == null)
        //    return;
        //if (!crosshairPrefabMap.ContainsKey(wep))
        //    return;

        //GameObject crosshairPrefab = crosshairPrefabMap[wep];
        //RaycastHit hit;
        //Transform bSpawn = curWeapon.weaponSettings.bulletSpawn;
        //Vector3 bSpawnPoint = bSpawn.position;
        //Vector3 dir = ray.GetPoint(curWeapon.weaponSettings.range) - bSpawnPoint;

        //if(Physics.Raycast(bSpawnPoint, dir, out hit, curWeapon.weaponSettings.range, curWeapon.weaponSettings.bulletLayers)){
        //    if(crosshairPrefab != null)
        //    {
        //        ToogleCrosshair(true, curWeapon);
        //        crosshairPrefab.transform.position = hit.point;
        //        crosshairPrefab.transform.LookAt(Camera.main.transform);
        //    }
        //}
        //else{
        //    //ToogleCrosshair(false, curWeapon);
        //}

    }

    void TurnOffAllCrosshairs(){
        foreach(Weapon wep in crosshairPrefabMap.Keys){
            //ToogleCrosshair(false, wep);
        }
    }

    void CreateCrosshair(Weapon wep){
        //GameObject prefab = wep.weaponSettings.crosshairPrefab;
        //if(prefab != null){
        //    prefab = Instantiate(prefab);
        //    ToogleCrosshair(false, wep);
        //}
    }
    //void DeleteCrosshair(Weapon wep){
    //    if (!crosshairPrefabMap.ContainsKey(wep))
    //        return;
    //    Destroy(crosshairPrefabMap[wep]);
    //    crosshairPrefabMap.Remove(wep);
    //}

    //void ToogleCrosshair(bool enabled, Weapon wep)
    //{
    //    if (!crosshairPrefabMap.ContainsKey(wep))
    //        return;
    //    crosshairPrefabMap[wep].SetActive(enabled);
    //}

    #region Trigger Checks
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag(crouchTag)){
            isCrouching = true;
        }
    }
    void OnTriggerStay(Collider other){
        if(other.gameObject.CompareTag(crouchTag)){
            isCrouching = true;
        }
    }
    void OnTriggerExit(Collider other){
        isCrouching = false;
    }
    #endregion
}


