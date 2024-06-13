using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] public Weapon[] weapons;


    private void Start(){
        InitVariables();
    }
    public void AddItem(Weapon newItem){
        //int newItemIndex = (int)newItem.weaponCatagory;
        //Debug.Log(newItem.weaponCatagory);
        //if(weapons[newItemIndex] != null) {
        //    RemoveItem(newItemIndex);
        //}
        //weapons[newItemIndex] = newItem;
        for(int i=0; i < weapons.Length; i++){
            if(weapons[i] == null){
                weapons[i] = newItem;
                break;
                Debug.Log("not here");
            }
            else{
                Debug.Log("Not empty");
            }
        }

    }
    public void RemoveItem(int index){
        weapons[index] = null;
    }

    public Weapon GetItem(int Index){
        return weapons[Index];
    }
    void InitVariables()
    {
        weapons = new Weapon[3];
    }
}
