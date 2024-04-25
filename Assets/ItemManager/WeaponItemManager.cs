using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemManager : MonoBehaviour
{
    [SerializeField]
    public Items item;

    [System.Serializable]
    public class UserSettings{

        public bool equipedHand;
    }

    [SerializeField]
    private GameObject myObject;

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
    }

    public void SwitchWeapons(){
       
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
                else if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Melee){
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
                //else if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Melee && item.name == "Axe"){
                //    myObject.transform.SetParent(owner.userSettings.UnequipSpot[0]);
                //    myObject.transform.position = owner.userSettings.UnequipSpot[0].position;
                //    myObject.transform.rotation = owner.userSettings.UnequipSpot[0].rotation;
                //}
                //else if(inventory.weapons[i] != null && inventory.weapons[i].weaponType == Weapon.WeaponType.Melee && item.name == "Sword"){
                //    myObject.transform.SetParent(owner.userSettings.UnequipSpot[2]);
                //    myObject.transform.position = owner.userSettings.UnequipSpot[2].position;
                //    myObject.transform.rotation = owner.userSettings.UnequipSpot[2].rotation;
                //}
            }
        }
    }
    public void SetEquipped(bool equip){
        equipped = equip;
    }

    public void SetOwner(WeaponHandler wp){
        owner = wp;
    }
}
[System.Serializable]
public class MeleeWeapons_Properties{

    public bool droped, reCreate, WeaponDrop;

    public WeaponHandler owner;
    public BoxCollider trigger;
    public BoxCollider weaponBox;

}
