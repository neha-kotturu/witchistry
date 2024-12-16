using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garlic : Interactable
{
    public GameObject garlicOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Garlic";
        promptMessage = "Dig Up Garlic [E]";
        this.gameObject.SetActive(true);
        garlicOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        garlicOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}