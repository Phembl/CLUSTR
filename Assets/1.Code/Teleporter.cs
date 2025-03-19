using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VInspector;

public class Teleporter : Interactable
{
    
    [ReadOnly]
    public Sprite entrySprite;
    [ReadOnly]
    public Sprite exitSprite;
    [ReadOnly]
    public GameObject playerPlaceholder;

    public bool isEntry;
    public GameObject target;
   
    private SpriteRenderer teleporterRenderer;
    
    protected override void Initialize()
    {
       teleporterRenderer = GetComponent<SpriteRenderer>();
       if (isEntry) teleporterRenderer.sprite = entrySprite;
       else teleporterRenderer.sprite = exitSprite;
    }

    protected override void Interact(GameObject player)
    {
        Vector3 targetPos = target.transform.position;
        //Create Placeholder
        GameObject placeholder = Instantiate(playerPlaceholder, targetPos, Quaternion.identity);
        SpriteRenderer placeholderRenderer = placeholder.GetComponent<SpriteRenderer>();
        SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
        
        placeholderRenderer.sprite = playerRenderer.sprite;
        placeholderRenderer.sortingOrder = playerRenderer.sortingOrder;
        
        player.GetComponent<SpriteRenderer>().DOFade(0, 0.4f);
        placeholder.GetComponent<SpriteRenderer>().DOFade(1, 0.4f)
            .OnComplete(() =>
        {
            player.transform.position = targetPos;
            playerRenderer.color = new Color(playerRenderer.color.r, playerRenderer.color.g, playerRenderer.color.b, 1f);
            Destroy(placeholder);
        });

        
    }
}