using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Interactable
{
    public GameObject mushroomOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Pick Mushroom [E]";
        this.gameObject.SetActive(true);
        mushroomOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        mushroomOnPlayer.SetActive(true);
    }
}
