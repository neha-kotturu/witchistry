using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cranberry : Interactable
{
    public GameObject cranberryOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Cranberry";
        promptMessage = "Uproot Cranberry Bush [E]";
        this.gameObject.SetActive(true);
        cranberryOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        cranberryOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 3);
    }
}