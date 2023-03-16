using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType{
        None, Bow, Axe, Dagger, Spear 
    }
    public WeaponType weaponType;

    [System.Serializable]
    public class UserSettings
    {
        public Transform HandIKTarget;
        public Transform equipHand;
        public bool equipedHand;
    }
    [SerializeField]
    public UserSettings userSettings;


    [System.Serializable]
    public class WeaponSettings
    {
        [Header("-Positioning-")]
        public Vector3 equipPosition;
        public Vector3 equipRotation;
        public Vector3 unequipPosition;
        public Vector3 unequipRotation;
    }


    [SerializeField]
    public WeaponSettings weaponSettings;

    public GameObject owner;
    bool equipped;
    bool resettingCartridge;

    void Update(){

        if(owner){
            if(equipped){
                if(userSettings.equipHand){
                    Equip();
                }
            }else{
                Unequip(weaponType);
            }
        }else{ 
            //transform.SetParent(null);
        }
    }

       void Equip(){
        if (!owner)
            return;
        else if (!userSettings.equipHand)
            return;

        transform.SetParent(userSettings.equipHand);
        transform.localPosition = weaponSettings.equipPosition;
        Quaternion equipRot = Quaternion.Euler(weaponSettings.equipRotation);
        transform.localRotation = equipRot;
        userSettings.equipedHand = true;
    }

    void Unequip(WeaponType wpType){
        if(!owner)
            return;

        switch(wpType){
            case WeaponType.Bow:
                transform.SetParent(owner.GetComponent<WeaponHandler>().userSettings.UnequipSpot);
                break;
            case WeaponType.Axe:
                //transform.SetParent(owner.userSettings.UnequipSpot);
                break;
            case WeaponType.Spear:
                //transform.SetParent(owner.userSettings.UnequipSpot);
                break;
            case WeaponType.Dagger:
                //transform.SetParent(owner.userSettings.UnequipSpot);
                break;
        }
        transform.localPosition = weaponSettings.unequipPosition;
        Quaternion unEquipRot = Quaternion.Euler(weaponSettings.unequipRotation);
        transform.localRotation = unEquipRot;
    }

    public void SetEquipped(bool equip){
        equipped = equip;
    }

    public void SetOwner(GameObject wp){
        owner = wp;
    }
}
