using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turmeric : Interactable
{
    public GameObject turmericOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Dig Up Turmeric [E]";
        this.gameObject.SetActive(true);
        turmericOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        turmericOnPlayer.SetActive(true);
    }
}
