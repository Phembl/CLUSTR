using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Key : Interactable
{
    public GameObject goal;
    private Collider goalCollider;
    private SpriteRenderer goalRenderer;
    
    protected override void Initialize()
    {
        goalCollider = goal.GetComponent<Collider>();
        goalRenderer = goal.GetComponent<SpriteRenderer>();
    }

    protected override void Interact(GameObject player)
    {
        //Deactivate Key
        GetComponent<Collider>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        
        //Acvtivate Goal
        goalCollider.enabled = true;
        goalRenderer.DOFade(1, 0.3f);
    }
}