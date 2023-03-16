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
        public Transform HandIKTarget;
        public Transform RightequipHand, LeftequipHand, SpearEquipHand;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class Animations{

        public string weaponTypeInt = "WeaponType";
        public string aimbool = "IsAiming";
        public string equipItem = "EquipItem";
        public string rightMask = "IsRightMask";
    }
    public Animations animations;

    public GameObject currentWeapon;
    public int maxWeapons = 5;
    public bool aim;
    public bool isAiming;
    public bool axeMode, fistMode, spearMode, rifleMode;
    public int weaponType;
    public bool settingWeapon, bowEquip, bowUnEquiped, rifleEquip, rifleUnEquiped, Equip_Melee;
    public bool IsBowPicked, IsRiflePicked, IsMeleePicked;

    public Transform targetOne;
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

        if(!bowUnEquiped ){
            Bow.Instance.SetEquipped(true);
            Bow.Instance.SetOwner(this);
            Bow.Instance.arrow.SetActive(true);
        }else{
            Bow.Instance.SetEquipped(false);
            Bow.Instance.SetOwner(this);
            Bow.Instance.arrow.SetActive(false);
        }

        if (!rifleUnEquiped){
            Rifle.Instance.SetEquipped(true);
            Rifle.Instance.SetOwner(this);
        }else{
            Rifle.Instance.SetEquipped(false);
            Rifle.Instance.SetOwner(this);
        }
    }

    void SwitchLocomotion(){
        if(states.locomotions == CharacterStates.Locomotions.CombatLocomotion){
            if(currentWeapon == null && states.combat == CharacterStates.CombatStates.Unarmed){
                fistMode = true;
            }
            else if(states.combat == CharacterStates.CombatStates.Axe){

                axeMode = true;
            }
            if(states.combat == CharacterStates.CombatStates.Spear){
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
            states.combat = CharacterStates.CombatStates.Axe;
        }
        if(currentWeapon != null && currentWeapon.gameObject.name == "Spear"){
            states.combat = CharacterStates.CombatStates.Spear;
        }
    }

    void Animate(){

        if (!animator)
            return;

        animator.SetInteger(animations.weaponTypeInt, weaponType);


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

        if(!currentWeapon && IsBowPicked && bowUnEquiped){
            bowUnEquiped = false;
            currentWeapon = Bow.Instance.gameObject;
            animator.SetTrigger(animations.equipItem);
            weaponType = 1;
            isAiming = true;
            aim = true;
        }
        else if(currentWeapon && IsBowPicked && !bowUnEquiped){
            bowUnEquiped = true;
            currentWeapon = null;
            animator.SetTrigger(animations.equipItem);
            weaponType = 0;
            isAiming = false;
            aim = false;
        }



        //if(!currentWeapon && IsRiflePicked && rifleUnEquiped){
        //    rifleUnEquiped = false;
        //    currentWeapon = Rifle.Instance.gameObject;
        //    animator.SetTrigger(animations.equipItem);
        //    weaponType = 1;
        //    isAiming = true;
        //    aim = true;
        //}
        //else if(currentWeapon && IsRiflePicked && !rifleUnEquiped){
        //    rifleUnEquiped = true;
        //    currentWeapon = null;
        //    animator.SetTrigger(animations.equipItem);
        //    weaponType = 0;
        //    isAiming = false;
        //    aim = false;
        //}


        if(currentWeapon && IsBowPicked && bowUnEquiped){
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
                weaponType = 1;
                isAiming = true;
                aim = true;
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
                weaponType = 1;
                isAiming = true;
                aim = true;
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
                weaponType = 1;
                isAiming = true;
                aim = true;
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
                weaponType = 1;
                isAiming = true;
                aim = true;
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
            Equip_Melee = true;
        }
        else {
            Debug.Log("Nothing");
        }

        if(Inventory.Instance.slotTwo != null && Inventory.Instance.slotTwo.name == "Spear"){
            Spear.Instance.SetEquipped(true);
            Spear.Instance.SetOwner(this);
            currentWeapon = Spear.Instance.gameObject;
            Equip_Melee = true;
        }
        else{
            Debug.Log("Nothing");
        }
    }

    void OnAnimatorIK(){

        if (!animator)
            return;

        if(currentWeapon && userSettings.HandIKTarget && weaponType == 1 && !bowUnEquiped){

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.HandIKTarget;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);
            //Debug.Log("working");
        }
        else if(currentWeapon && userSettings.HandIKTarget && weaponType == 1 && !rifleUnEquiped){
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.HandIKTarget;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);
            //Debug.Log("working");
        }
        else{

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            //Debug.Log("not working");
        }
    }
}
