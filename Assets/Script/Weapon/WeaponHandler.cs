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
        public Transform leftHandIK, rightHandIK, rifleLeftIK, rifleRightIK;
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
        public string equpied = "MeleeEquiped";
        public string pistolEquiped = "PistolEquiped";
        public string rifleEquiped = "RifleEquiped";
    }
    public Animations animations;

    public GameObject currentWeapon;
    public int maxWeapons = 5;
    public bool aim;
    public bool isAiming;
    public bool pistolMode, rifleMode, meleeMode;
    public int weaponType;
    public bool settingWeapon, Equip_Melee;
    public bool meleeSwitching;

    public Transform targetOne;
    public Transform targetTwo;
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
        DropMelee_Weapon();
    }
    void LateUpdate() {
        StartCoroutine(MeleeSwitchDelay());
    }

    void DropMelee_Weapon(){
        if (Input.GetKeyDown(UserInput.Instance.input.dropButton)){

            if(currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe ||
                currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword){

                currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
                currentWeapon = null;
                meleeMode = false;
                inventory.RemoveItem(0);
            }
            else if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe ||
                currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword){

                currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
                currentWeapon = null;
                meleeMode = false;
                inventory.RemoveItem(1);
            }
            else if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe ||
                currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Sword){

                currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
                currentWeapon = null;
                meleeMode = false;
                inventory.RemoveItem(2);
            }
        }
    }

    IEnumerator MeleeSwitchDelay(){
        yield return new WaitForSeconds(0.2f);

        if(meleeSwitching){

            StartCoroutine(SetUpMeleeWeapon());
            meleeSwitching = false;
        }
    }

    public IEnumerator SetUpMeleeWeapon(){
        yield return new WaitForSeconds(0.1f);

        #region First Slot Axe
        if(currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[1] == null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        else if(currentWeapon != null && weaponType <= 0 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe && inventory.weapons[1] != null
            && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(0);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 1 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword && inventory.weapons[0] != null
            && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true; 
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 2 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe && inventory.weapons[1] != null
            && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(0);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 3 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword && inventory.weapons[0] != null
            && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType = 0;

        }
        #endregion


        #region First Slot Sword
        if(currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[1] == null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        else if (currentWeapon != null && weaponType <= 0 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword && inventory.weapons[1] != null
           && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(0);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 1 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe && inventory.weapons[0] != null
          && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 2 && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword && inventory.weapons[1] != null
          && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(0);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 3 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe && inventory.weapons[0] != null
           && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType = 0;

        }
        #endregion


        #region Second Slot Axe
        if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[0] != null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe &&
            inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        else if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[0] != null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe &&
            inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }


        if(currentWeapon != null && weaponType <= 0 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe && inventory.weapons[2] != null
          && inventory.weapons[2].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);
            Debug.Log("Entered Coroutine");


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[2].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if (currentWeapon != null && weaponType == 1 && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Sword && inventory.weapons[1] != null
          && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe){


            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 2 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe && inventory.weapons[2] != null
          && inventory.weapons[2].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[2].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 3 && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Sword && inventory.weapons[1] != null
          && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType = 0;
        }
        #endregion

        #region Second Slot Sword
        if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[0] != null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword &&
            inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        else if (currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[0] != null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword &&
            inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        
        
        if (currentWeapon != null && weaponType <= 0 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword && inventory.weapons[2] != null
          && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[2].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 1 && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe && inventory.weapons[1] != null
          && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword){


            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if(currentWeapon != null && weaponType == 2 && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword && inventory.weapons[2] != null
          && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[2].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType += 1;
        }
        else if (currentWeapon != null && weaponType == 3 && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe && inventory.weapons[1] != null
          && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);


            yield return new WaitForSeconds(0.1f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
            weaponType = 0;
        }
        #endregion

        if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol && inventory.weapons[1] !=null &&
           inventory.weapons[1].weaponType == Weapon.WeaponType.Rifle && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){
            
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        else if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle && inventory.weapons[1] != null &&
           inventory.weapons[1].weaponType == Weapon.WeaponType.Pistol && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }



        if (currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol && inventory.weapons[1] != null &&
           inventory.weapons[1].weaponType == Weapon.WeaponType.Rifle && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
        else if (currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle && inventory.weapons[1] != null &&
           inventory.weapons[1].weaponType == Weapon.WeaponType.Pistol && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){

            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            meleeMode = true;
        }
    }

    #region Weapon Switching
    public IEnumerator SwitchWeapons(){

        yield return new WaitForSeconds(0.1f);

        #region Rifle and Pistol Switch

        if (currentWeapon == null && inventory.weapons[0] != null && inventory.weapons[1] == null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }
        else if(currentWeapon == null && inventory.weapons[0] != null && inventory.weapons[1] == null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }


        else if(currentWeapon == null && inventory.weapons[0] != null && inventory.weapons[1] != null && inventory.weapons[2] ==null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }
        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[0].name && inventory.weapons[0] != null && inventory.weapons[1] != null && inventory.weapons[2] == null && 
            inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){

            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            pistolMode = false;

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }

        else if(currentWeapon == null && inventory.weapons[0] != null && inventory.weapons[1] != null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }
        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[0].name && inventory.weapons[0] != null && inventory.weapons[1] != null && inventory.weapons[2] == null &&
            inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){

            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            rifleMode = false;

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }


        else if(currentWeapon == null && inventory.weapons[1] != null && inventory.weapons[0] == null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }
        else if(currentWeapon == null && inventory.weapons[1] != null && inventory.weapons[0] == null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }
        #endregion

        //else if(currentWeapon == null && inventory.weapons[2] != null && inventory.weapons[0] == null && inventory.weapons[1] == null && inventory.weapons[2].weaponType == Weapon.WeaponType.Pistol){
        //    currentWeapon = inventory.weapons[2].prefab;
        //    currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
        //}
        //else if(currentWeapon == null && inventory.weapons[2] != null && inventory.weapons[0] == null && inventory.weapons[1] == null && inventory.weapons[2].weaponType == Weapon.WeaponType.Rifle){
        //    currentWeapon = inventory.weapons[2].prefab;
        //    currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
        //}


        #region Switching from meleeWeapon

        else if (currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[0].weaponCatagory == Weapon.WeaponCatagories.Melee &&inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(0);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
            
        }
        else if(currentWeapon != null &&  inventory.weapons[0] != null && inventory.weapons[0].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[1] !=null && inventory.weapons[1].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(0);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }

        else if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[0] == null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            rifleMode = false;
            currentWeapon = null;
        }
        else if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[0] == null && inventory.weapons[2] == null && inventory.weapons[1].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            pistolMode = false;
            currentWeapon = null;
        }


        else if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[1].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }
        else if(currentWeapon != null && inventory.weapons[1] != null && inventory.weapons[1].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(1);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }
        else if(currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[1] == null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            rifleMode = false;
            currentWeapon = null;
        }
        else if(currentWeapon != null && inventory.weapons[0] != null && inventory.weapons[1] == null && inventory.weapons[2] == null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            pistolMode = false;
            currentWeapon = null;
        }



        else if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[2].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Rifle){
            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }
        else if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[2].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Pistol){
            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[1].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }



        else if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[2].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[1] == null && 
            inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Pistol){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            pistolMode = true;
        }
        else if(currentWeapon != null && inventory.weapons[2] != null && inventory.weapons[2].weaponCatagory == Weapon.WeaponCatagories.Melee && inventory.weapons[1] == null &&
            inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Rifle){

            currentWeapon.GetComponent<WeaponItemManager>().meleeWeapons_Properties.droped = true;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            meleeMode = false;
            currentWeapon = null;
            inventory.RemoveItem(2);

            yield return new WaitForSeconds(0.08f);
            currentWeapon = inventory.weapons[0].prefab;
            currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
            rifleMode = true;
        }
        #endregion




        //else if(currentWeapon != null && currentWeapon.name == inventory.weapons[0].name && inventory.weapons[1] != null){
        //    currentWeapon.GetComponent<WeaponItemManager>().equipped = false;

        //    yield return new WaitForSeconds(0.1f);
        //    currentWeapon = inventory.weapons[1].prefab;
        //    currentWeapon.GetComponent<WeaponItemManager>().equipped = true;
        //}
        ////else if(currentWeapon != null && currentWeapon.name == inventory)

        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[1].name){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            currentWeapon = null;
            pistolMode = false;
            rifleMode = false;
        }
        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[2].name)
        {
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            currentWeapon = null;
            pistolMode = false;
            rifleMode = false;
        }
        else if(currentWeapon != null && currentWeapon.name == inventory.weapons[3].name){
            currentWeapon.GetComponent<WeaponItemManager>().equipped = false;
            currentWeapon = null;
            pistolMode = false;
            rifleMode = false;
        }
    }
    #endregion

    void Animate(){

        if (!animator)
            return;

        animator.SetInteger(animations.weaponTypeInt, weaponType);

        if(meleeMode){
            animator.SetBool(animations.equpied, meleeMode);
        }
        else{
            animator.SetBool(animations.equpied, false);
        }

        if (pistolMode){
            animator.SetBool(animations.pistolEquiped, pistolMode);
            isAiming = true;
        }
        else if (rifleMode){
            animator.SetBool(animations.rifleEquiped, rifleMode);
            isAiming = true;
        }
        else{
            animator.SetBool(animations.rifleEquiped, false);
            animator.SetBool(animations.pistolEquiped, false);
            isAiming = false;
        }


        if(isAiming){
            animator.SetBool(animations.aimbool, isAiming);
        }
        else if(!isAiming){
            animator.SetBool(animations.aimbool, isAiming);
           
        }


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

        if(currentWeapon && userSettings.HandIKTarget && !meleeMode && pistolMode ==false && rifleMode == false){

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.HandIKTarget;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);
            Debug.Log("IKing");
        }
        
        else if(currentWeapon && userSettings.HandIKTarget && pistolMode){
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.rightHandIK;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            targetTwo = userSettings.leftHandIK;
            Vector3 targetPosTwo = targetTwo.position;
            Quaternion targetRotTwo = targetTwo.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosTwo);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRotTwo);
            Debug.Log("Pistol IK");
        }
        else if(currentWeapon && userSettings.HandIKTarget && rifleMode){
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            targetOne = userSettings.rifleRightIK;
            Vector3 targetPos = targetOne.position;
            Quaternion targetRot = targetOne.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRot);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            targetTwo = userSettings.rifleLeftIK;
            Vector3 targetPosTwo = targetTwo.position;
            Quaternion targetRotTwo = targetTwo.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosTwo);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRotTwo);
        }
        else{

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            //Debug.Log("not working");
        }
    }
}
