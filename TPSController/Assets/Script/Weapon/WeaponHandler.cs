using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponHandler : MonoBehaviour
{
    #region Singleton
    public static WeaponHandler Instance;

    void Awake(){
        Instance = this;
    }

    #endregion

    public Animator animator;
    public CharacterStates states;

    [System.Serializable]
    public class UserSettings
    {
        public Transform UnequipSpot;
        public Transform rifleUnequipSpot;
        public Transform RightHandIKTarget, LeftHandIKTarget, rifle_RightHandIK, rifle_LeftHandIK, 
                        rifle_AimLeftIK, rifle_AimRightIK, bow_AimleftIK, bow_AimrightIK;
        public Transform RightequipHand, LeftequipHand, SpearEquipHand, rifle_LefteuipedHand;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class Animations{

        public string weaponTypeInt = "WeaponType";
        public string aimbool = "IsAiming";
        public string bowMode = "bowMode";
        public string rifleMode = "rifleMode";
        public string equipItem = "EquipItem";
        public string rightMask = "IsRightMask";
    }

    public enum WeaponType
    {
        BOW, RIFLE, AXE, SPEAR
    }

    public WeaponType weaponType;
    public Animations animations;

    public GameObject currentWeapon;
    public int maxWeapons = 2;
    public bool aim;
    public bool isAiming;
    public bool axeMode, fistMode, spearMode, rifleMode, bowMode;
    public int weaponCount;
    public bool settingWeapon, bowEquip, bowUnEquiped, rifleEquip, rifleUnEquiped, Equip_Melee;
    public bool IsBowPicked, IsRiflePicked, IsMeleePicked;

    public Transform targetOne;
    public Transform targetTwo;
    void Start(){
        animator = GetComponent<Animator>();
        states = GetComponent<CharacterStates>();
    }

    void Update(){

        Animate();
        SwitchLocomotion();
        SwitchCombatState();
        BowEquiped();
        RifleEquiped();
        EquipedMeleeWeapon();
    }

    public void SetupWeapons(){

        if(IsBowPicked){
            if(!bowUnEquiped){
                Bow.Instance.SetEquipped(true);
                Bow.Instance.SetOwner(this);
                Bow.Instance.arrow.SetActive(true);
            }else{
                Bow.Instance.SetEquipped(false);
                Bow.Instance.SetOwner(this);
                Bow.Instance.arrow.SetActive(false);
            }
        }else{
           // Debug.Log("Bow is not picked yet");
        }

        if(IsRiflePicked){
            if(!rifleUnEquiped){
                Rifle.Instance.SetEquipped(true);
                Rifle.Instance.SetOwner(this);
            }else{
                Rifle.Instance.SetEquipped(false);
                Rifle.Instance.SetOwner(this);
            }
        }else{
           // Debug.Log("Rifle is not picked yet");
        }
    }

    void SwitchLocomotion(){
        if(states.locomotions == CharacterStates.Locomotions.CombatLocomotion){
            if(currentWeapon == null && states.combat == CharacterStates.CombatStates.Unarmed){
                fistMode = true;
            }
            else if(states.combat == CharacterStates.CombatStates.Armed && weaponType == WeaponType.AXE){

                axeMode = true;
            }
            if(states.combat == CharacterStates.CombatStates.Armed && weaponType == WeaponType.SPEAR){
                spearMode = true;
            }
        }
        else if(states.locomotions == CharacterStates.Locomotions.FreeLocomotion){

            fistMode = false;
            axeMode = false;
            spearMode = false;
        }
    }

    void SwitchCombatState(){
        if(currentWeapon == null){
            states.combat = CharacterStates.CombatStates.Unarmed;
        }
        else if(currentWeapon != null && currentWeapon.gameObject.name == "Axe"){
            states.combat = CharacterStates.CombatStates.Armed;
        }
        if(currentWeapon != null && currentWeapon.gameObject.name == "Spear"){
            states.combat = CharacterStates.CombatStates.Armed;
        }
    }

    void Animate(){

        if (!animator)
            return;

        animator.SetInteger(animations.weaponTypeInt, weaponCount);


        if(isAiming){
            animator.SetBool(animations.aimbool, isAiming);
            Bow.Instance.animator.SetTrigger("Spring");
            Bow.Instance.animator.SetFloat("PowerCharger", 1.0f);
        }
        else if(!isAiming){
            animator.SetBool(animations.aimbool, isAiming);
            Bow.Instance.animator.SetTrigger("UnSpring");
            Bow.Instance.animator.SetFloat("PowerCharger", 0.0f);
        }


        if (Equip_Melee){
            animator.SetBool(animations.rightMask, Equip_Melee);
        }
        else if (!Equip_Melee){
            animator.SetBool(animations.rightMask, Equip_Melee);
        }
    }

    public void SwitchWeapons(){

        if(!currentWeapon && IsBowPicked && bowUnEquiped && weaponType == WeaponType.BOW && weaponCount <= 0){
            bowUnEquiped = false;
            bowMode = true;
            currentWeapon = Bow.Instance.gameObject;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.bowMode, bowMode);
            weaponCount += 1;
           // isAiming = true;
           // aim = true;
        }
        else if(!currentWeapon && IsRiflePicked && rifleUnEquiped && weaponType == WeaponType.RIFLE && weaponCount <= 0){
            rifleUnEquiped = false;
            rifleMode = true;
            currentWeapon = Rifle.Instance.gameObject;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.rifleMode, rifleMode);
            weaponCount += 1;
            //isAiming = true;
            //aim = true;
        }
        else if(currentWeapon && IsBowPicked && bowUnEquiped && !rifleUnEquiped && weaponType == WeaponType.RIFLE && weaponCount <= 1){
            bowUnEquiped = false;
            rifleUnEquiped = true;
            rifleMode = false;
            bowMode = true;
            weaponType = WeaponType.BOW;
            currentWeapon = Bow.Instance.gameObject;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.bowMode, bowMode);
            animator.SetBool(animations.rifleMode, rifleMode);

            weaponCount +=1;
            //isAiming = true;
           // aim = true;
        }
        else if(currentWeapon && IsRiflePicked && rifleUnEquiped && !bowUnEquiped && weaponType == WeaponType.BOW && weaponCount <= 1){
            rifleUnEquiped = false;
            bowUnEquiped = true;
            bowMode = false;
            rifleMode = true;
            weaponType = WeaponType.RIFLE;
            currentWeapon = Rifle.Instance.gameObject;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.bowMode, bowMode);
            animator.SetBool(animations.rifleMode, rifleMode);

            weaponCount += 1;
            //isAiming = false;
            //aim = false;
            //    //isAiming = true;
            //    //aim = true;
        }
        else if(currentWeapon && IsBowPicked && bowUnEquiped && !rifleUnEquiped && weaponType == WeaponType.RIFLE && weaponCount >= 2){
            bowUnEquiped = true;
            rifleUnEquiped = true;
            currentWeapon = null;
            bowMode = false;
            rifleMode = false;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.bowMode, bowMode);
            animator.SetBool(animations.rifleMode, rifleMode);
            weaponCount = 0;
        }
        else if(currentWeapon && IsRiflePicked && rifleUnEquiped && !bowUnEquiped && weaponType == WeaponType.BOW && weaponCount >= 2){
            bowUnEquiped = true;
            rifleUnEquiped = true;
            currentWeapon = null;
            bowMode = false;
            rifleMode = false;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.bowMode, bowMode);
            animator.SetBool(animations.rifleMode, rifleMode);
            weaponCount = 0;
        }
        else if(currentWeapon && IsBowPicked && !bowUnEquiped && weaponType == WeaponType.BOW){
            bowUnEquiped = true;
            currentWeapon = null;
            bowMode = false;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.bowMode, bowMode);
            weaponCount = 0;
           // isAiming = false;
           // aim = false;
        }
        else if(currentWeapon && IsRiflePicked && !rifleUnEquiped && weaponType == WeaponType.RIFLE){
            rifleUnEquiped = true;
            rifleMode = false;
            currentWeapon = null;
            animator.SetTrigger(animations.equipItem);
            animator.SetBool(animations.rifleMode, rifleMode);
            //animator.SetBool(animations.bowMode, false);
            weaponCount = 0;
        }



        if (currentWeapon && IsBowPicked && bowUnEquiped){
            if(currentWeapon.gameObject.name == "Axe"){

                Axe.Instance.SetEquipped(false);
                Axe.Instance.SetOwner(null);
                Axe.Instance.droped = true;
                Axe.Instance.WeaponDrop = false;
                Inventory.Instance.slotTwo = null;
                IsMeleePicked = false;
                Equip_Melee = false;
                bowUnEquiped = false;
                currentWeapon = Bow.Instance.gameObject;
                animator.SetTrigger(animations.equipItem);
                weaponCount = 1;
                //isAiming = true;
                //aim = true;
            }
            else if(currentWeapon.gameObject.name == "Spear"){

                Spear.Instance.SetEquipped(false);
                Spear.Instance.SetOwner(null);
                Spear.Instance.droped = true;
                Spear.Instance.WeaponDrop = false;
                Inventory.Instance.slotTwo = null;
                IsMeleePicked = false;
                Equip_Melee = false;
                bowUnEquiped = false;
                currentWeapon = Bow.Instance.gameObject;
                animator.SetTrigger(animations.equipItem);
                weaponCount = 1;
                //isAiming = true;
                //aim = true;
            }
        }
        else if(currentWeapon && IsRiflePicked && rifleUnEquiped){
            if(currentWeapon.gameObject.name == "Axe"){
                Axe.Instance.SetEquipped(false);
                Axe.Instance.SetOwner(null);
                Axe.Instance.droped = true;
                Axe.Instance.WeaponDrop = false;
                Inventory.Instance.slotTwo = null;
                IsMeleePicked = false;
                Equip_Melee = false;
                bowUnEquiped = false;
                currentWeapon = Bow.Instance.gameObject;
                animator.SetTrigger(animations.equipItem);
                weaponCount = 1;
               // isAiming = true;
               // aim = true;
            }
            else if(currentWeapon.gameObject.name == "Spear"){
                Spear.Instance.SetEquipped(false);
                Spear.Instance.SetOwner(null);
                Spear.Instance.droped = true;
                Spear.Instance.WeaponDrop = false;
                Inventory.Instance.slotTwo = null;
                IsMeleePicked = false;
                Equip_Melee = false;
                bowUnEquiped = false;
                currentWeapon = Bow.Instance.gameObject;
                animator.SetTrigger(animations.equipItem);
                weaponCount = 1;
                //isAiming = true;
                //aim = true;
            }
        }

        settingWeapon = true;
        StartCoroutine(StopSettingWeapon());
    }
    void BowEquiped(){

        if(IsBowPicked && !bowEquip){

            StartCoroutine(BowEqiupedTimer());
            bowEquip = true;
        }
    }
    void RifleEquiped(){
        if(IsRiflePicked && !rifleEquip){
            StartCoroutine(RifleEquipedTimer());
            rifleEquip = true;
        }
    }
    void EquipedMeleeWeapon(){
        if(IsMeleePicked && Inventory.Instance.slotTwo.name == "Axe" && Axe.Instance.droped != true){
            if(currentWeapon == null){
                StartCoroutine(MeleeWeaponEquip());
            }
            else if(currentWeapon != null && currentWeapon.gameObject.name == "Spear" && Axe.Instance.WeaponDrop != true){
                Spear.Instance.SetEquipped(false);
                Spear.Instance.SetOwner(null);
                Spear.Instance.droped = true;
                Spear.Instance.WeaponDrop = false;
                Equip_Melee = false;
                StartCoroutine(MeleeWeaponEquip());
                Axe.Instance.WeaponDrop = true;
            }
        }

        if(IsMeleePicked && Inventory.Instance.slotTwo.name == "Spear" && Spear.Instance.droped != true){
            if(currentWeapon == null){
                StartCoroutine(MeleeWeaponEquip());
            }
            else if(currentWeapon != null && currentWeapon.gameObject.name == "Axe" && Spear.Instance.WeaponDrop != true){
        
                Axe.Instance.SetEquipped(false);
                Axe.Instance.SetOwner(null);
                Axe.Instance.droped = true;
                Axe.Instance.WeaponDrop = false;
                Equip_Melee = false;
                StartCoroutine(MeleeWeaponEquip());
                Spear.Instance.WeaponDrop = true;
            }
        }
    }

    IEnumerator StopSettingWeapon(){

        yield return new WaitForSeconds(0.7f);
        settingWeapon = false;
    }
    IEnumerator BowEqiupedTimer(){

        yield return new WaitForSeconds(0.7f);
        Bow.Instance.DisableCollider();
        Bow.Instance.SetEquipped(false);
        Bow.Instance.SetOwner(this);
        bowUnEquiped = true;
    }

    IEnumerator RifleEquipedTimer(){

        yield return new WaitForSeconds(0.7f);
        Rifle.Instance.SetEquipped(false);
        Rifle.Instance.SetOwner(this);
        rifleUnEquiped = true;
    }
    IEnumerator MeleeWeaponEquip(){

        yield return new WaitForSeconds(0.7f);

        if(Inventory.Instance.slotTwo != null && Inventory.Instance.slotTwo.name == "Axe"){
            Axe.Instance.SetEquipped(true);
            Axe.Instance.SetOwner(this);
            currentWeapon = Axe.Instance.gameObject;
            weaponType = WeaponType.AXE;
            Equip_Melee = true;
        }
        else {
            Debug.Log("Nothing");
        }

        if(Inventory.Instance.slotTwo != null && Inventory.Instance.slotTwo.name == "Spear"){
            Spear.Instance.SetEquipped(true);
            Spear.Instance.SetOwner(this);
            currentWeapon = Spear.Instance.gameObject;
            weaponType = WeaponType.SPEAR;
            Equip_Melee = true;
        }
        else{
            Debug.Log("Nothing");
        }
    }

    void OnAnimatorIK(){

        if (!animator)
            return;

        if(currentWeapon && userSettings.RightHandIKTarget && weaponType == WeaponType.BOW){

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.RightHandIKTarget;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            targetTwo = userSettings.LeftHandIKTarget;
            Vector3 targetPosTwo = targetTwo.position;
            Quaternion targerRotTwo = targetTwo.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosTwo);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targerRotTwo);
            Debug.Log("BowIK");
        }
        else if(currentWeapon && userSettings.rifle_RightHandIK  && weaponType == WeaponType.RIFLE){
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.rifle_RightHandIK;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            targetTwo = userSettings.rifle_LeftHandIK;
            Vector3 targetPosTwo = targetTwo.position;
            Quaternion targerRotTwo = targetTwo.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosTwo);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targerRotTwo);
            Debug.Log("RifleIK");

        }
        if(currentWeapon && userSettings.rifle_AimRightIK && isAiming && weaponType == WeaponType.RIFLE){
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.rifle_AimRightIK;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            targetTwo = userSettings.rifle_AimLeftIK;
            Vector3 targetPosTwo = targetTwo.position;
            Quaternion targerRotTwo = targetTwo.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosTwo);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targerRotTwo);
            Debug.Log("RifleIK");
        }
        else if (currentWeapon && userSettings.bow_AimrightIK && isAiming && weaponType == WeaponType.BOW){
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.bow_AimrightIK;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            targetTwo = userSettings.bow_AimleftIK;
            Vector3 targetPosTwo = targetTwo.position;
            Quaternion targerRotTwo = targetTwo.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosTwo);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targerRotTwo);
            Debug.Log("RifleIK");
        }       
        else{
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            targetOne = null;
            targetTwo = null;
            //Debug.Log("not working");
        }
    }
}
