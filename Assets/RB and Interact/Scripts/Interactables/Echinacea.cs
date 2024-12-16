using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echinacea : Interactable
{
    public GameObject echinaceaOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Echinacea";
        promptMessage = "Pick Echinacea [E]";
        this.gameObject.SetActive(true);
        echinaceaOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        echinaceaOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}