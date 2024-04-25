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
        public Transform[] UnequipSpot;
        public Transform[] EquipSpot;
        public Transform HandIKTarget;
        public Transform LeftequipHand;
    }
    [SerializeField]
    public UserSettings userSettings;
    [SerializeField]
    private Inventory inventory;

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

    public Transform targetOne;
    void Start(){
        GetReference();
    }

    void GetReference(){
        animator = GetComponent<Animator>();
        states = GetComponent<CharacterStates>();

        inventory = GetComponent<Inventory>();
    }

    void Update(){

        Animate();
    }

    //public void SwitchWeapons() {

    //   // if(currentWeapon == null && weaponType <= 0 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
    //   //     currentWeapon = inventory.weapons[0].prefab;
    //   //     currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
    //   //     weaponType += 1;
    //   // }
    //   // else if (currentWeapon == null && weaponType <= 0 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
    //   //     currentWeapon = inventory.weapons[0].prefab;
    //   //     currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
    //   //     weaponType += 1;
    //   // }

    //   //else if(currentWeapon != null && weaponType == 1 && inventory.weapons[1] != null){
    //   //     //currentWeapon.GetComponent<WeaponItemManager>().enabled = false;
    //   //     currentWeapon = inventory.weapons[1].prefab;
    //   //     weaponType += 1;
    //   // }
    //}

    public IEnumerator SwitchWeapons(){

        yield return new WaitForSeconds(0.1f);

        if(currentWeapon == null && weaponType <= 0 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            weaponType += 1;
        }
        else if(currentWeapon == null && weaponType <= 0 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            weaponType += 1;
        }

        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[0].name && weaponType == 1 && inventory.weapons[1] != null){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;

            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[1].name && weaponType == 2){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            weaponType = 0;
            currentWeapon = null;
        }
    }

        void Animate(){

        if (!animator)
            return;

        animator.SetInteger(animations.weaponTypeInt, weaponType);


        //if(isAiming){
        //    animator.SetBool(animations.aimbool, isAiming);
        //    Bow.Instance.animator.SetTrigger("Spring");
        //    Bow.Instance.animator.SetFloat("PowerCharger", 1.0f);
        //}
        //else if(!isAiming){
        //    animator.SetBool(animations.aimbool, isAiming);
        //    Bow.Instance.animator.SetTrigger("UnSpring");
        //    Bow.Instance.animator.SetFloat("PowerCharger", 0.0f);
        //}


        if (Equip_Melee){
            animator.SetBool(animations.rightMask, Equip_Melee);
        }
        else if (!Equip_Melee){
            animator.SetBool(animations.rightMask, Equip_Melee);
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
