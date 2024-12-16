using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aloe : Interactable //
{
    public GameObject aloeOnPlayer; //
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Aloe"; //
        promptMessage = "Collect Aloe [E]"; //
        this.gameObject.SetActive(true);
        aloeOnPlayer.SetActive(false); //
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        aloeOnPlayer.SetActive(true); //
        inventory.updateItem(plantID, 1);
    }
}