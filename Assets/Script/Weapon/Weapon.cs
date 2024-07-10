using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Weapon", menuName = "Items/Weapon")]
public class Weapon : Items
{
    public GameObject prefab;
    public WeaponType weaponType;
    public WeaponCatagories weaponCatagory;

    public enum WeaponType { Axe, Sword, Pistol, Rifle}
    public enum WeaponCatagories { Primary, Secondary, Melee}

}
