using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamomile : Interactable
{
    public GameObject chamomileOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Chamomile";
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
        inventory.updateItem(plantID, 1);
    }
}