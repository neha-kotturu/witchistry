using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamomile : Interactable
{
    public GameObject chamomileOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Pick Chamomile [E]";
        this.gameObject.SetActive(true);
        chamomileOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        chamomileOnPlayer.SetActive(true);
    }
}
