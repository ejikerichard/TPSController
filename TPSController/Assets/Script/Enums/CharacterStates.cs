using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    public enum Locomotions{
        FreeLocomotion, CombatLocomotion
    }
    public enum Movement{
        WALK, RUNNING
    }
    public enum Actions{
        None, ClimbUp, Vault, Push, Pulling
    }

    public enum CombatStates{
        Unarmed, Armed
    }
    public enum AttackID{
        AttackOne, AttackTwo, AttackThree, AttackFour, AttackFive
    }

    public Locomotions locomotions;
    public Movement movement;
    public Actions actions;
    public CombatStates combat;
    public AttackID attackID;
}

