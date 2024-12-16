using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinnamon : Interactable
{
    public GameObject cinnamonOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Cinnamon";
        promptMessage = "Collect Cinnamon Branch [E]";
        this.gameObject.SetActive(true);
        cinnamonOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        cinnamonOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}