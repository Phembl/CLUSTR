using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    public static event Action<Interactable, GameObject> OnPlayerInteracted;
    
    public static event Action<bool> OnLevelFinished; 

    public static event Action OnPlayerEnable;
    public static event Action OnPlayerDisable;
    public static event Action<bool, Vector3>OnPlayerSetPositionAndShow;
    public static event Action OnPlayerHideAndReset; 
    
    public static event Action OnEnableRotation;
    public static event Action OnDisableRotation;
    public static event Action OnResetRotation;

    public static event Action<Vector3, float> OnRotate;
    
    public static event Action OnGravitySwitch;
    
    // Method to invoke the event
    public static void TriggerPlayerInteracted(Interactable interactable, GameObject player)
    {
        OnPlayerInteracted?.Invoke(interactable, player);
    }


    public static void TriggerLevelFinished(bool success)
    {
        OnLevelFinished?.Invoke(success);
    }

    public static void TriggerPlayerEnable()
    {
        OnPlayerEnable?.Invoke();
    }

    public static void TriggerPlayerDisable()
    {
        OnPlayerDisable?.Invoke();
    }
    
    public static void TriggerPlayerSetPositionAndShow(bool fadeIn, Vector3 newPosition)
    {
        OnPlayerSetPositionAndShow?.Invoke(fadeIn, newPosition);
    }

    public static void TriggerPlayerHideAndReset()
    {
        OnPlayerHideAndReset?.Invoke();
    }

    public static void TriggerEnableRotation()
    {
        OnEnableRotation?.Invoke();
    }

    public static void TriggerDisableRotation()
    {
        OnDisableRotation?.Invoke();
    }

    public static void TriggerResetRotation()
    {
        OnResetRotation?.Invoke();
    }
    
    public static void TriggerRotation(Vector3 rotationVector, float rotationTime)
    {
        OnRotate?.Invoke(rotationVector, rotationTime);
    }

    public static void TriggerGravitySwitch()
    {
        OnGravitySwitch?.Invoke();
    }
}
