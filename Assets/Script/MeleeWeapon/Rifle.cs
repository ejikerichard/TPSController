using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    #region Singleton
    public static Rifle Instance;

    void Awake(){
        Instance = this;
    }
    #endregion

    [System.Serializable]
    public class UserSettings{

        public bool equipedHand;
    }
    [SerializeField]
    public UserSettings userSettings;

    public bool equipped;

    public WeaponHandler owner;
    public BoxCollider trigger;


    void Start(){
        
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
    void Equip(){

        if (!owner)
            return;
        else if (!owner.userSettings.LeftequipHand)
            return;

        transform.SetParent(owner.userSettings.LeftequipHand);
        transform.position = owner.userSettings.LeftequipHand.position;
        transform.rotation = owner.userSettings.LeftequipHand.rotation;
        userSettings.equipedHand = true;
    }
    void Unequip(){

        if (!owner)
            return;

        transform.SetParent(owner.userSettings.rifleUnequipSpot);
        transform.position = owner.userSettings.rifleUnequipSpot.position;
        transform.rotation = owner.userSettings.rifleUnequipSpot.rotation;
    }

    public void SetEquipped(bool equip){
        equipped = equip;
    }

    public void SetOwner(WeaponHandler wp){
        owner = wp;
    }

}
