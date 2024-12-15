using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hibiscus : Interactable
{
    public GameObject hibiscusOnPlayer;
    private PlayerInteract playerInteract;
    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Pick Hibiscus [E]";
        this.gameObject.SetActive(true);
        hibiscusOnPlayer.SetActive(false);
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }


    protected override void Interact()
    {
        playerInteract.hideAllItemsOnPlayer();
        this.gameObject.SetActive(false);
        hibiscusOnPlayer.SetActive(true);
    }
}
