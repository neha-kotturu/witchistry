using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ginseng : Interactable
{
    public GameObject ginsengOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Ginseng";
        promptMessage = "Dig Up Ginseng [E]";
        this.gameObject.SetActive(true);
        ginsengOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        ginsengOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}