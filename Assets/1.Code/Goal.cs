using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Goal : Interactable
{
    protected override void Initialize()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<SpriteRenderer>().DOFade(0, 0f);
    }

    protected override void Interact(GameObject player)
    {
        GetComponent<Collider>().enabled = false;
    }
}
