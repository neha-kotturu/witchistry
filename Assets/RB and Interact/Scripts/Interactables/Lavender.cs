using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lavender : Interactable
{
    public GameObject lavenderOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Pick Lavender [E]";
        this.gameObject.SetActive(true);
        lavenderOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        lavenderOnPlayer.SetActive(true);
    }
}
