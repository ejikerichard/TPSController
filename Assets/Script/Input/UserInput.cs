﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    #region Singleton
    public static UserInput Instance;

    void Awake(){
        Instance = this;
    }
    #endregion

    public WeaponHandler weaponHandler { get; protected set; }
    public CharacterMovement3D characterMovement3D { get; protected set; }

    [System.Serializable]
    public class InputSettings
    {
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public int mouseButtonZero = 0;
        public int mouseButtonOne = 1;
        public int mouseButtonTwo = 2;
        public KeyCode rightShift = KeyCode.RightShift;
    }

    [SerializeField]
    public InputSettings input;

    public UnityEngine.Events.UnityEvent OnLateUpdate;

    [SerializeField]
    public Camera mainCamera;

    void Start(){
        Init();
    }

    void Init(){
        characterMovement3D = GetComponent<CharacterMovement3D>();
        weaponHandler = GetComponent<WeaponHandler>();
    }

    void Update(){
        WeaponLogic();
    }

    void LateUpdate(){

        if (!weaponHandler.aim){
            OnLateUpdate.Invoke();
        }
    }
    void WeaponLogic(){
        if(!weaponHandler)
            return;

        if (Input.GetKeyDown(input.rightShift)){
            weaponHandler.SwitchWeapons();
            Debug.Log("amount");
        }
    }
}

