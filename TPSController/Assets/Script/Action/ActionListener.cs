﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionListiner : MonoBehaviour
{
    public bool actionEnter;
    public bool actionStay;
    public bool actionExit;
    public vOnActionHandle OnDoAction = new vOnActionHandle();

    public virtual void OnActionEnter(Collider other){

    }

    public virtual void OnActionStay(Collider other){

    }

    public virtual void OnActionExit(Collider other){

    }

    [System.Serializable]
    public class vOnActionHandle : UnityEngine.Events.UnityEvent<TriggerGenericAction>{

    }
}
