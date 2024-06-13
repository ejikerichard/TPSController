using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemManager : MonoBehaviour
{
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
    }
    void SetWeapon(){
        if(IsMeleeWeapon && owner.currentWeapon == null && owner.weaponType <= 0  && inventory.weapons[0].weaponType == Weapon.WeaponType.Melee){
            equipped = true;
            owner.currentWeapon = inventory.weapons[0].prefab;
            owner.weaponType += 1;
            Debug.Log("Weapon one Equiped");
        }
        else if(IsMeleeWeapon && owner.currentWeapon == null && owner.weaponType <= 0 && inventory.weapons[1].weaponType == Weapon.WeaponType.Melee){
            equipped = true;
            owner.currentWeapon = inventory.weapons[1].prefab;
            owner.weaponType += 1;

            Debug.Log("Weapon two Equiped");
        }
        else if(IsMeleeWeapon && owner.currentWeapon == null && owner.weaponType <= 0 &&  inventory.weapons[2].weaponType == Weapon.WeaponType.Melee){
            equipped = true;
            owner.currentWeapon = inventory.weapons[2].prefab;
            owner.weaponType += 1;

            Debug.Log("Weapon three Equiped");
        }
    }
    void Equip(){
        if (!owner)
            return;
        else if (!owner.userSettings.LeftequipHand)
            return;

        if (IsProjectileWeapon){
            for(int i = 0; i < inventory.weapons.Length; i++){
                if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Pistol){
                    myObject.transform.SetParent(owner.userSettings.EquipSpot[1]);
                    myObject.transform.position = owner.userSettings.EquipSpot[1].position;
                    myObject.transform.rotation = owner.userSettings.EquipSpot[1].rotation;
                    userSettings.equipedHand = true;
                    break;
                }
                else if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Rifle){
                    myObject.transform.SetParent(owner.userSettings.EquipSpot[0]);
                    myObject.transform.position = owner.userSettings.EquipSpot[0].position;
                    myObject.transform.rotation = owner.userSettings.EquipSpot[0].rotation;
                    userSettings.equipedHand = true;
                    break;
                }
            }
        }

        if(IsMeleeWeapon){
            for(int i=0; i < inventory.weapons.Length; i++){
                if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Melee){
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
            meleeWeapons_Properties.reCreate = true;
        }

        if(meleeWeapons_Properties.reCreate){
            meleeWeapons_Properties.reCreate = false;
            meleeWeapons_Properties.droped = false;
            StartCoroutine(OnReset_MeleeWeaponProperties());
        }
    }
    
    IEnumerator OnReset_MeleeWeaponProperties(){
        yield return new WaitForSeconds(0.2f);

        if(IsMeleeWeapon && inventory.weapons[0].name == "Axe"){

            meleeWeapons_Properties.weaponBox.enabled = false;
            myObject.GetComponent<Rigidbody>().useGravity = false;
            myObject.GetComponent<Rigidbody>().isKinematic = true;

            GameObject collectableObject = new GameObject("AxeCollectable");
            collectableObject.transform.position = transform.position;
            myObject.transform.parent = collectableObject.transform;
            BoxCollider Boxcollider = collectableObject.AddComponent<BoxCollider>();

            Boxcollider.center = new Vector3(-0.13f, 0.62f, -0.08f);
            Boxcollider.size = new Vector3(2.22f, 1.72f, 2.16f);
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

            IsMeleeWeapon = false;
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
