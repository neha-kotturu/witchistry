using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ginkgo : Interactable
{
    public GameObject ginkgoOnPlayer;
    private PlayerInteract playerInteract;
    public Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        plantID = "Ginkgo";
        promptMessage = "Pick Up Ginkgo Leaves [E]";
        this.gameObject.SetActive(true);
        ginkgoOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        ginkgoOnPlayer.SetActive(true);
        inventory.updateItem(plantID, 1);
    }
}