using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neem : Interactable
{
    public GameObject neemOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Neem";
        promptMessage = "Collect Neem Leaves [E]";
        this.gameObject.SetActive(true);
        neemOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        neemOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}