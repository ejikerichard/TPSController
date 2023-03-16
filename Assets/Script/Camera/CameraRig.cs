using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour{

    public WeaponHandler weaponHandler { get; protected set; }

    [System.Serializable]
    public class CameraSetting
    {
        [Header("-Zoom-")]
        public float fieldofView = 70.0f;
        public float zoomFieldOfView = 30.0f;
        public float zoomspeed;

        [Header("-Camera Option-")]
        public Transform target;
        public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);
        public float Smooth = 10f;
        public float horizontalAimingSpeed = 6f;
        public float verticalAimingSpeed = 6f;
        public float maxVerticalAngle = 30f;
        public float minVerticalAngle = -60f;
        public string XAxis = "Analog X";
        public string YAxis = "Analog Y";

        public float angleH = 0;
        public float angleV = 0;
        public Transform cam;
        public Vector3 relCameraPos;                                      
        public float relCameraPosMag;
        public Vector3 smoothPivotOffset;                                
        public Vector3 smoothCamOffset;                                   
        public Vector3 targetPivotOffset;                                
        public Vector3 targetCamOffset;
        public Quaternion aimingRotation;
        public Quaternion normalRotation;
        public float defaultFOV;                                          
        public float targetFOV;                                          
        public float targetMaxVerticalAngle;
        public LayerMask CamOcclusion;
    }

    [SerializeField]
    public CameraSetting cameraSetting;
    Vector3 camMask;


    [SerializeField]
    public Camera mainCamera;

    public float GetH { get { return cameraSetting.angleH; } }

    public bool rotated;

    void Start(){
        Init();
    }

    void Init(){
        mainCamera = Camera.main;
        weaponHandler = GameObject.Find("Player").GetComponent<WeaponHandler>();

        cameraSetting.cam = transform;

        cameraSetting.cam.position = cameraSetting.target.position + Quaternion.identity * cameraSetting.pivotOffset + Quaternion.identity * cameraSetting.camOffset;
        cameraSetting.cam.rotation = Quaternion.identity;

        cameraSetting.relCameraPos = transform.position - cameraSetting.target.position;
        cameraSetting.relCameraPosMag = cameraSetting.relCameraPos.magnitude - 0.5f;

        cameraSetting.smoothPivotOffset = cameraSetting.pivotOffset;
        cameraSetting.smoothCamOffset = cameraSetting.camOffset;
        cameraSetting.defaultFOV = mainCamera.fieldOfView;
        cameraSetting.angleH = cameraSetting.target.position.y;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();
    }
    void Update(){
        RotateCamera();
        Zooming(weaponHandler.aim);
    }

    void LateUpdate(){
        occludeRay(ref cameraSetting.camOffset);
    }

    void RotateCamera(){
        cameraSetting.angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * cameraSetting.horizontalAimingSpeed;
        cameraSetting.angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * cameraSetting.verticalAimingSpeed;

        cameraSetting.angleH += Mathf.Clamp(Input.GetAxis(cameraSetting.XAxis), -1, 1) * 60 * cameraSetting.horizontalAimingSpeed * Time.deltaTime;
        cameraSetting.angleV += Mathf.Clamp(Input.GetAxis(cameraSetting.YAxis), -1, 1) * 60 * cameraSetting.verticalAimingSpeed * Time.deltaTime;

        cameraSetting.angleV = Mathf.Clamp(cameraSetting.angleV, cameraSetting.minVerticalAngle, cameraSetting.targetMaxVerticalAngle);

        Quaternion camYRotation = Quaternion.Euler(0, cameraSetting.angleH, 0);
        Quaternion aimRotation = Quaternion.Euler(-cameraSetting.angleV, cameraSetting.angleH, 0);
        cameraSetting.cam.rotation = aimRotation;

        Vector3 baseTemPosition = cameraSetting.target.position + camYRotation * cameraSetting.targetPivotOffset;
        Vector3 noCollisionOffset = cameraSetting.targetCamOffset;
        for (float zOffset = cameraSetting.targetCamOffset.z; zOffset <= 0; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTemPosition + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0)
            {
                break;
            }
        }

        cameraSetting.smoothPivotOffset = Vector3.Lerp(cameraSetting.smoothPivotOffset, cameraSetting.targetPivotOffset, cameraSetting.Smooth * Time.deltaTime);
        cameraSetting.smoothCamOffset = Vector3.Lerp(cameraSetting.smoothCamOffset, noCollisionOffset, cameraSetting.Smooth * Time.deltaTime);
        cameraSetting.cam.position = cameraSetting.target.position + camYRotation * cameraSetting.smoothPivotOffset + aimRotation * cameraSetting.smoothCamOffset;
    }

    public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset){
        cameraSetting.targetPivotOffset = newPivotOffset;
        cameraSetting.targetCamOffset = newCamOffset;
    }

    // Reset camera offsets to default values.
    public void ResetTargetOffsets(){
        cameraSetting.targetPivotOffset = cameraSetting.pivotOffset;
        cameraSetting.targetCamOffset = cameraSetting.camOffset;
    }

    // Reset the camera vertical offset.
    public void ResetYCamOffset()
    {
        cameraSetting.targetCamOffset.y = cameraSetting.camOffset.y;
    }

    // Set camera vertical offset.
    public void SetYCamOffset(float y)
    {
        cameraSetting.targetCamOffset.y = y;
    }

    // Set camera horizontal offset.
    public void SetXCamOffset(float x){
        cameraSetting.targetCamOffset.x = x;
    }

    // Set custom Field of View.
    public void SetFOV(float customFOV){
        this.cameraSetting.targetFOV = customFOV;
    }

    // Reset Field of View to default value.
    public void ResetFOV(){
        this.cameraSetting.targetFOV = cameraSetting.defaultFOV;
    }

    // Set max vertical camera rotation angle.
    public void SetMaxVerticalAngle(float angle){
        this.cameraSetting.targetMaxVerticalAngle = angle;
    }

    // Reset max vertical camera rotation angle to default value.
    public void ResetMaxVerticalAngle(){
        this.cameraSetting.targetMaxVerticalAngle = cameraSetting.maxVerticalAngle;
    }

    bool DoubleViewingPosCheck(Vector3 checkPos, float offset){
        float playerFocusHeight = cameraSetting.target.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight){
        // Cast target.
        Vector3 target = cameraSetting.target.position + (Vector3.up * deltaPlayerHeight);
        RaycastHit hit;
        //// If a raycast from the check position to the player hits something...
        if(Physics.SphereCast(checkPos, 0.2f, target - checkPos, out hit, cameraSetting.relCameraPosMag)){
            // ... if it is not the player...
            if(hit.transform != cameraSetting.target && !hit.transform.GetComponent<Collider>().isTrigger){
                // This position isn't appropriate.
                return false;
            }
        }
        // If we haven't hit anything or we've hit the player, this is an appropriate position.
        return true;
    }

    // Check for collision from player to camera.
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance){
        // Cast origin.
        Vector3 origin = cameraSetting.target.position + (Vector3.up * deltaPlayerHeight);
        RaycastHit hit;
        if(Physics.SphereCast(origin, 0.2f, checkPos - origin, out hit, maxDistance)){
            if(hit.transform != cameraSetting.target && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger){
                return false;
            }
        }
        return true;
    }

    void occludeRay(ref Vector3 targetFollow){
        RaycastHit hit = new RaycastHit();
        if(Physics.Linecast(targetFollow, camMask, out hit, cameraSetting.CamOcclusion)){
            cameraSetting.Smooth = 10f;

            cameraSetting.cam.position = new Vector3(hit.point.x + hit.normal.x * 0.5f, cameraSetting.cam.position.y, hit.point.z + hit.normal.z * 0.5f);
        }
    }

    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset){
        return Mathf.Abs((finalPivotOffset - cameraSetting.smoothPivotOffset).magnitude);
    }

    void Zooming(bool IsZooming){
        if (!mainCamera)
            return;

        if(IsZooming){
            cameraSetting.targetFOV = 30;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSetting.targetFOV, Time.deltaTime * cameraSetting.zoomspeed);
        }
        else{
            cameraSetting.targetFOV = 60;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSetting.targetFOV, Time.deltaTime * cameraSetting.zoomspeed);
        }
    }
}
