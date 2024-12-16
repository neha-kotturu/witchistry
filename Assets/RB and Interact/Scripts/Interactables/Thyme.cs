using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thyme : Interactable
{
    public GameObject thymeOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Thyme";
        promptMessage = "Collect Thyme [E]";
        this.gameObject.SetActive(true);
        thymeOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        thymeOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}