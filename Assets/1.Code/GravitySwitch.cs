using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GravitySwitch : Interactable
{
    
    protected override void Initialize()
    {
 
    }

    protected override void Interact(GameObject player)
    {
        EventHandler.TriggerGravitySwitch();
    }
}