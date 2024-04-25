using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    #region Singleton
    public static Spear Instance;

    void Awake(){
        Instance = this;
    }
    #endregion

    public Rigidbody mybody { get; protected set; }

    [System.Serializable]
    public class UserSetting{

        public bool equipedHand;
    }
    [SerializeField]
    public UserSetting userSettings;

    public bool equipped;
    public bool Used, droped, reCreate, WeaponDrop;

    public WeaponHandler owner;
    public BoxCollider trigger;
    public BoxCollider weaponBox;

    void Start(){
        mybody = GetComponent<Rigidbody>();
    }

    void Update(){

        //if(owner){

        //    if(equipped){

        //        if(owner.userSettings.SpearEquipHand){
        //            Equip();
        //        }
        //    }
        //    else{
        //        Unequip();
        //    }
        //}else{
        //    //transform.SetParent(null);
        //}

        DropWeapon();
    }

    void Equip(){

        //if (!owner)
        //    return;
        //else if (!owner.userSettings.SpearEquipHand)
        //    return;

        //transform.parent = owner.userSettings.SpearEquipHand.transform;
        //transform.position = owner.userSettings.SpearEquipHand.position;
        //transform.rotation = owner.userSettings.SpearEquipHand.rotation;
        //userSettings.equipedHand = true;
    }

    void Unequip(){

        if (!owner)
            return;

        transform.SetParent(null);
        //transform.position = owner.userSettings.UnequipSpot.position;
        //transform.rotation = owner.userSettings.UnequipSpot.rotation;
    }

    public void SetEquipped(bool equip){

        equipped = equip;
    }

    public void SetOwner(WeaponHandler wp){

        owner = wp;
    }

    void DropWeapon(){

        if(droped){
            weaponBox.enabled = true;
            mybody.useGravity = true;
            mybody.isKinematic = false;
            transform.SetParent(null);
            reCreate = true;
        }

        if(reCreate){
            reCreate = false;
            droped = false;
            StartCoroutine(OnDestroyWeapon());
        }
    }

    IEnumerator OnDestroyWeapon(){

        yield return new WaitForSeconds(0.9f);

        weaponBox.enabled = false;
        mybody.useGravity = false;
        mybody.isKinematic = true;

        GameObject collectableObject = new GameObject("SpearCollectable");
        collectableObject.transform.position = transform.position;
        transform.parent = collectableObject.transform;
        BoxCollider Boxcollider = collectableObject.AddComponent<BoxCollider>();

        Boxcollider.center = new Vector3(-0.13f, 0.62f, -0.08f);
        Boxcollider.size = new Vector3(2.22f, 1.72f, 2.16f);
        Boxcollider.isTrigger = true;
        trigger = Boxcollider;


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
    }
}
