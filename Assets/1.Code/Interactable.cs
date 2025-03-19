using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private int currentRotation = 0;
    
    private void OnEnable()
    {
        EventHandler.OnPlayerInteracted += HandlePlayerInteraction;
        EventHandler.OnGravitySwitch += GravitySwitch;
        EventHandler.OnRotate += Rotate;
        Initialize();
    }

    private void OnDisable()
    {
        EventHandler.OnPlayerInteracted -= HandlePlayerInteraction;
        EventHandler.OnGravitySwitch -= GravitySwitch;
        EventHandler.OnRotate -= Rotate;
    }

    private void HandlePlayerInteraction(Interactable interactable, GameObject player)
    {
        // Check if this is the interactable that was triggered
        if (interactable == this)
        {
            Interact(player);
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = SetSortingOrder();
    }


    protected abstract void Initialize();
    protected abstract void Interact(GameObject player);
    


    private void Rotate(Vector3 rotationVector, float rotationTime) //Send by CameraController
    {
        if (rotationVector.y > 0)
        {
            if (currentRotation == 3) currentRotation = 0;
            else currentRotation++;
        }
        else
        {
            if (currentRotation == 0) currentRotation = 3;
            else currentRotation--;
        }

        spriteRenderer.sortingOrder = SetSortingOrder();
        
        Vector3 targetRotation = (transform.eulerAngles + rotationVector);
        transform.DORotate(targetRotation, rotationTime);
    }
    
    private int SetSortingOrder()
    {
        int distance = 0;
        int pos = 0;

        switch (currentRotation)
        {
            case 0:
                pos = Mathf.RoundToInt(transform.position.z);
                distance = (pos * 10);
                break;
            
            case 1:
                pos = Mathf.RoundToInt(transform.position.x);
                distance = (pos * 10);
                break;
            
            case 2:
                pos = Mathf.RoundToInt(-(transform.position.z));
                distance = (pos * 10);
                break;
            
            case 3:
                pos = Mathf.RoundToInt(-(transform.position.x));
                distance = (pos * 10);
                break;
        }
        return -(distance);
  
    }
    
    private void GravitySwitch()
    {
        Vector3 targetRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180f);
        transform.DORotate(targetRotation, 1f);
    }
    
}
