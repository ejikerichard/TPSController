using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    public GameObject player;

    void Awake(){
        instance = this;
    }
}
