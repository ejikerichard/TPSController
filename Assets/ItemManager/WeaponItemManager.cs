using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemManager : MonoBehaviour
{
    public enum WeaponTypes
    {
        RIFLE, PISTOL, SWORD, AXE
    }

    [SerializeField]
    private WeaponTypes weaponTypes;

    [SerializeField]
    public Items item;

    [SerializeField]
    private GameObject myObject;

    [SerializeField]
    public MeleeWeapons_Properties meleeWeapons_Properties;

    [SerializeField]
    public UserSettings userSettings;

    public WeaponHandler owner;
    public Inventory inventory;


    public bool equipped;
    public bool IsProjectileWeapon, IsMeleeWeapon;

    private void Start(){
        GetReference();
    }
    void GetReference(){
        owner = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponHandler>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void Update(){
        if(owner){
            if(equipped){
                if(owner.userSettings.LeftequipHand){
                    Equip();
                }
            }else{
                Unequip();
            }
        }else{
            //transform.SetParent(null);
        }

        SetWeapon();
        DropWeapon();
    }
    void SetWeapon() {
        if(IsMeleeWeapon && owner.currentWeapon == null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Axe){
            owner.currentWeapon = inventory.weapons[0].prefab;
        }
        else if(IsMeleeWeapon && owner.currentWeapon == null && inventory.weapons[0] != null && inventory.weapons[0].weaponType == Weapon.WeaponType.Sword){
            owner.currentWeapon = inventory.weapons[0].prefab;
        }

        if(IsMeleeWeapon && owner.currentWeapon == null && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Axe){
            owner.currentWeapon = inventory.weapons[1].prefab;
        }
        else if(IsMeleeWeapon && owner.currentWeapon == null && inventory.weapons[1] != null && inventory.weapons[1].weaponType == Weapon.WeaponType.Sword){
            owner.currentWeapon = inventory.weapons[1].prefab;
        }


        if(IsMeleeWeapon && owner.currentWeapon == null && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Axe){
            owner.currentWeapon = inventory.weapons[2].prefab;
        }
        else if(IsMeleeWeapon && owner.currentWeapon == null && inventory.weapons[2] != null && inventory.weapons[2].weaponType == Weapon.WeaponType.Sword){
            owner.currentWeapon = inventory.weapons[2].prefab;
        }

    }
    void Equip(){
        if (!owner)
            return;
        else if (!owner.userSettings.LeftequipHand)
            return;

        if (IsProjectileWeapon){
            for(int i = 0; i < inventory.weapons.Length; i++){
                if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Pistol  && weaponTypes == WeaponTypes.PISTOL){
                    myObject.transform.SetParent(owner.userSettings.EquipSpot[1]);
                    myObject.transform.position = owner.userSettings.EquipSpot[1].position;
                    myObject.transform.rotation = owner.userSettings.EquipSpot[1].rotation;
                    userSettings.equipedHand = true;
                    Debug.Log("Equiped in Pistol");
                    break;
                }
                
                if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Rifle && weaponTypes == WeaponTypes.RIFLE){
                    myObject.transform.SetParent(owner.userSettings.EquipSpot[0]);
                    myObject.transform.position = owner.userSettings.EquipSpot[0].position;
                    myObject.transform.rotation = owner.userSettings.EquipSpot[0].rotation;
                    userSettings.equipedHand = true;
                    Debug.Log("Equiped in Rifle");
                    break;
                }
            }
        }

        if(IsMeleeWeapon){
            for(int i=0; i < inventory.weapons.Length; i++){
                if(inventory.weapons[i] != null && inventory.weapons[i].weaponCatagory == Weapon.WeaponCatagories.Melee){
                    myObject.transform.SetParent(owner.userSettings.EquipSpot[2]);
                    myObject.transform.position = owner.userSettings.EquipSpot[2].position;
                    myObject.transform.rotation = owner.userSettings.EquipSpot[2].rotation;
                    userSettings.equipedHand = true;
                    break;
                }
            }
        }
    }
    void Unequip(){

        if (!owner)
            return;

        if (IsProjectileWeapon){
            for(int i = 0; i < inventory.weapons.Length; i++){
                if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Pistol && item.name == "Pistol"){
                    myObject.transform.SetParent(owner.userSettings.UnequipSpot[1]);
                    myObject.transform.position = owner.userSettings.UnequipSpot[1].position;
                    myObject.transform.rotation = owner.userSettings.UnequipSpot[1].rotation;
                    break;
                }
                else if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Rifle && item.name == "Rifle"){
                    myObject.transform.SetParent(owner.userSettings.UnequipSpot[0]);
                    myObject.transform.position = owner.userSettings.UnequipSpot[0].position;
                    myObject.transform.rotation = owner.userSettings.UnequipSpot[0].rotation;
                    break;
                }
            }
        }

    }
    public void SetEquipped(bool equip){
        equipped = equip;
    }

    public void SetOwner(WeaponHandler wp){
        owner = wp;
    }

    public void DropWeapon(){
        if (meleeWeapons_Properties.droped){
            meleeWeapons_Properties.weaponBox.enabled = true;
            myObject.GetComponent<Rigidbody>().useGravity = true;
            myObject.GetComponent<Rigidbody>().isKinematic = false;
            myObject.transform.SetParent(null);
            userSettings.equipedHand = false;
            owner.meleeMode = false;
            meleeWeapons_Properties.reCreate = true;
        }

        if(meleeWeapons_Properties.reCreate){
            meleeWeapons_Properties.reCreate = false;
            meleeWeapons_Properties.droped = false;
            StartCoroutine(OnReset_MeleeWeaponProperties());
        }
    }
    
    IEnumerator OnReset_MeleeWeaponProperties(){
        yield return new WaitForSeconds(0.9f);

        #region AxeTrigger Recreation
        if (IsMeleeWeapon && weaponTypes == WeaponTypes.AXE){

            GameObject collectableObject = new GameObject("AxeCollectable");
            collectableObject.transform.position = myObject.transform.position;
            myObject.transform.parent = collectableObject.transform;
            BoxCollider Boxcollider = collectableObject.AddComponent<BoxCollider>();

            Boxcollider.center = new Vector3(0f, 0.06f, 0f);
            Boxcollider.size = new Vector3(1.19f, 0.66f, 1.22f);
            Boxcollider.isTrigger = true;
            meleeWeapons_Properties.trigger = Boxcollider;


            var triggerAction = collectableObject.AddComponent<TriggerGenericAction>();

            triggerAction.autoAction = false;
            triggerAction.disableCollision = false;
            triggerAction.disableGravity = false;
            triggerAction.resetPlayerSettings = false;
            triggerAction.playAnimation = "Pick_Down";
            triggerAction.endExitTimeAnimation = 0.8f;
            triggerAction.avatarTarget = AvatarTarget.RightHand;
            triggerAction.activeFromForward = false;
            triggerAction.useTriggerRotation = false;
            triggerAction.destroyAfter = true;
            triggerAction.isWeapon = true;
            triggerAction.MeleeWeapon = true;
            triggerAction.destroyDelay = 0.9f;

            yield return new WaitForSeconds(0.3f);
            meleeWeapons_Properties.weaponBox.enabled = false;
            myObject.GetComponent<Rigidbody>().useGravity = false;
            myObject.GetComponent<Rigidbody>().isKinematic = true;

            IsMeleeWeapon = false;
        }
        #endregion

        #region SwordTrigger Recreate

        else if (IsMeleeWeapon && weaponTypes == WeaponTypes.SWORD){
            GameObject collectableObject = new GameObject("SwordCollectable");
            collectableObject.transform.position = myObject.transform.position;
            myObject.transform.parent = collectableObject.transform;
            BoxCollider Boxcollider = collectableObject.AddComponent<BoxCollider>();

            Boxcollider.center = new Vector3(0f, 0.06f, 0f);
            Boxcollider.size = new Vector3(1.19f, 0.43f, 0.94f);
            Boxcollider.isTrigger = true;
            meleeWeapons_Properties.trigger = Boxcollider;


            var triggerAction = collectableObject.AddComponent<TriggerGenericAction>();

            triggerAction.autoAction = false;
            triggerAction.disableCollision = false;
            triggerAction.disableGravity = false;
            triggerAction.resetPlayerSettings = false;
            triggerAction.playAnimation = "Pick_Down";
            triggerAction.endExitTimeAnimation = 0.8f;
            triggerAction.avatarTarget = AvatarTarget.RightHand;
            triggerAction.activeFromForward = false;
            triggerAction.useTriggerRotation = false;
            triggerAction.destroyAfter = true;
            triggerAction.isWeapon = true;
            triggerAction.MeleeWeapon = true;
            triggerAction.destroyDelay = 0.9f;

            yield return new WaitForSeconds(0.3f);
            meleeWeapons_Properties.weaponBox.enabled = false;
            myObject.GetComponent<Rigidbody>().useGravity = false;
            myObject.GetComponent<Rigidbody>().isKinematic = true;

            IsMeleeWeapon = false;
        }
        #endregion
        else{
            Debug.Log("Non");
        }
    }
}
[System.Serializable]
public class MeleeWeapons_Properties{

    public bool droped, reCreate, WeaponDrop;

    public BoxCollider trigger;
    public BoxCollider weaponBox;

}
[System.Serializable]
public class UserSettings{

    public bool equipedHand;
}
