using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowers : Interactable
{
    public GameObject flowersOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Pick Flowers [E]";
        this.gameObject.SetActive(true);
        flowersOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        flowersOnPlayer.SetActive(true);
    }
}
