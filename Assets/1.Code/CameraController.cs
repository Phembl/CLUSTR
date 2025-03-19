using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Controls inputActions;
    private bool isActive;
    private bool isUsed;
    private float clickValue;

    private const float RotationTime = 1.1f;
    
    void Awake()
    {
        // Instantiate the Input Actions object
        inputActions = new Controls();
    }
    
    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
             
        EventHandler.OnEnableRotation += EnableRotation;
        EventHandler.OnDisableRotation += DisableRotation;
        EventHandler.OnResetRotation += ResetRotation;

    }
    
    void Start()
    {
  
    }

    void Update()
    {
        clickValue = inputActions.Gameplay.CameraRotationActivasion.ReadValue<float>();
        
        if (isActive)
        {
            if (clickValue > 0f) //Checks if clicked
            {
                //Checks Mouse Direction
                if (inputActions.Gameplay.CameraRotation.ReadValue<float>() > 2f) RotateCamera(90);
                else if (inputActions.Gameplay.CameraRotation.ReadValue<float>() < -2f) RotateCamera(-90);
            }
            
        }
     
    }

    private void EnableRotation()
    {
        isActive = true;
    }
    
    private void DisableRotation()
    {
        isActive = false;
    }
    

    private void RotateCamera(int direction)
    {
        isActive = false;
        EventHandler.TriggerPlayerDisable();
        
        Vector3 rotationVector = new Vector3(0f, direction, 0f);
        Vector3 targetRotation = (transform.eulerAngles + rotationVector);
            
        EventHandler.TriggerRotation(rotationVector, RotationTime);
        transform.DORotate(targetRotation, RotationTime).SetEase(Ease.OutQuad)
                .OnComplete(FinishRotation);
        
    }

    private void FinishRotation()
    {
        //Enable Player
        EventHandler.TriggerPlayerEnable();
        //Enable Camera
        isActive = true;
    }

    private void ResetRotation()
    {
        transform.rotation = new Quaternion(0,0,0,0);
    }
}
