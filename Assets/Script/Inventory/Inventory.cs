using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public int spaceSlot;

    public static Inventory Instance;

    public void Awake(){

        if(Instance != null){
            Debug.Log("InventoryManager is Destoryed");
        }

        Instance = this;
    }
    #endregion

    public ItemListData slotOne;
    public ItemListData slotTwo;

    public void AddItem(ItemListData itemListData){


        if (itemListData.isDefaultWeapon){
            slotOne = itemListData;
        }
        else if(!itemListData.isDefaultWeapon){
            if(slotTwo == null){
                slotTwo = itemListData;
            }
            else if(slotTwo != null){
                slotTwo = null;
                slotTwo = itemListData;
            }
        }
    }
}
