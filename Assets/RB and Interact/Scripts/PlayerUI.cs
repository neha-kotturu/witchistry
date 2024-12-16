using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI inventoryContents;
    private Inventory inventory;


    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        inventoryContents.enabled = false;
    }

    void Update()
    {
        // Show inventory if tab pressed down
        if (Input.GetKey(KeyCode.Tab))
        {
            inventoryContents.enabled = true;
        }
        else {
            inventoryContents.enabled = false;
        }
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
}
