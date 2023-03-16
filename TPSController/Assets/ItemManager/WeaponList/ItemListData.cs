using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeaponListItem", menuName ="Inventory/Weapon")]
public class ItemListData : ScriptableObject
{
    new public string name = "";
    public Sprite icon = null;
    public bool isDefaultWeapon = false;


    //// Called when the item is pressed in the inventory
    //public virtual void Use(){
    //    // Use the item
    //    // Something may happen
    //}


    //// Call this method to remove the item from inventory
    //public void RemoveFromInventory(){
    //    Inventory.Instace.RemoveItem(this);
    //}
}
