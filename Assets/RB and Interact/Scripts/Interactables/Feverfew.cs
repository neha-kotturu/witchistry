using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feverfew : Interactable
{
    public GameObject feverfewOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Feverfew";
        promptMessage = "Pick Feverfew [E]";
        this.gameObject.SetActive(true);
        feverfewOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        feverfewOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}