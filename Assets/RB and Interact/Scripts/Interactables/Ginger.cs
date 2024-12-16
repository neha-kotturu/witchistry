using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ginger : Interactable
{
    public GameObject gingerOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Ginger";
        promptMessage = "Dig Up Ginger [E]";
        this.gameObject.SetActive(true);
        gingerOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        gingerOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}