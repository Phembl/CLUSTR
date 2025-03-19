using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VInspector;


public class Switch : Interactable
{
    public GameObject bridge;
    [ReadOnly]
    public Sprite activeSprite;
    [ReadOnly]
    public Sprite inactiveSprite;
    
    private SpriteRenderer switchRenderer;
    private Collider blockCollider;
    private Renderer blockRenderer;
    private Color blockColor;
    
    
    protected override void Initialize()
    {
        //Set Switch
        switchRenderer = GetComponent<SpriteRenderer>();
        switchRenderer.sprite = inactiveSprite;
        
        blockCollider = bridge.GetComponent<Collider>();
        blockRenderer = bridge.GetComponent<Renderer>();
        blockCollider.enabled = false;
        blockRenderer.enabled = false;
        
    }

    protected override void Interact(GameObject player)
    {
        if (!blockCollider.enabled)
        {
            switchRenderer.sprite = activeSprite; 
            blockCollider.enabled = true;
            blockRenderer.enabled = true;
           // blockRenderer.material.DOFade(1, 0.3f);
        }
        
        else
        {
            switchRenderer.sprite = inactiveSprite;
            blockCollider.enabled = false;
            blockRenderer.enabled = false;
            //blockRenderer.material.DOFade(0, 0.3f);
        }
    }
}