using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basil : Interactable
{
    public GameObject basilOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Basil";
        promptMessage = "Pick Basil [E]";
        this.gameObject.SetActive(true);
        basilOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        basilOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}