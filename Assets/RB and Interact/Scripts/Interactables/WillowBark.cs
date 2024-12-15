using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WillowBark : Interactable
{
    public GameObject willowBarkOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Pickup Willow Branch [E]";
        this.gameObject.SetActive(true);
        willowBarkOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        willowBarkOnPlayer.SetActive(true);
    }
}
