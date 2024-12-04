using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Interactable
{
    public GameObject grassOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Gather Grass [E]";
        this.gameObject.SetActive(true);
        grassOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        grassOnPlayer.SetActive(true);
    }
}
