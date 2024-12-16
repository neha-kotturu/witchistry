using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();
    string[] plants =   {"Aloe", "Chamomile", "Lavender", "Peppermint", "Ginger", "Turmeric",
                        "Echinacea", "Willow Bark", "Garlic", "Ginseng", "Cinnamon", "Neem",
                        "Thyme", "Basil", "Hibiscus", "Cranberry", "Feverfew", "Ginkgo"};

    [SerializeField] private TextMeshProUGUI inventoryContents;
    void Start() 
    {
        // initialize all plant amounts in inventory to 0
        foreach(string plant in plants)
        {
            inventory.Add(plant, 0);
        }
        UpdateInventoryGUI();
        Debug.Log("here");
    }

    // Returns inventory contents as a Dictionary
    public Dictionary<string, int> GetInventory()
    {
        return inventory;
    }

    // Adds ${amount} to inventory element ${key}
    public void updateItem(string key, int amount)
    {
        if (inventory.ContainsKey(key))
        {
            inventory[key] = inventory[key] + amount;
        }
        Debug.Log("Added 1 Item: " + inventory[key]);

        // Update text on inventory screen
        UpdateInventoryGUI();
    }

    void UpdateInventoryGUI()
    {
        inventoryContents.text = "";
        // Update text on inventory screen
        foreach(KeyValuePair<string, int> item in inventory)
        {
            inventoryContents.text += item.Key;
            inventoryContents.text += ": ";
            inventoryContents.text += item.Value;
            inventoryContents.text += "\n";
        }
    }
}
