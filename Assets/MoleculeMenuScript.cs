using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class MoleculeMenuScript : MonoBehaviour
{
    public TMP_Dropdown moleculeDropdown;
    public TMP_Dropdown functionalGroupDropdown;
    public TextAsset functionalGroupsJSON;

    public string moleculesPath = "Molecules"; // Relative path to your folder

    // Start is called before the first frame update
    void Start()
    {
        LoadDropdowns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    void LoadDropdowns()
    {
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(moleculesPath);
        if (textAssets.Length == 0)
        {
            Debug.LogError($"No .txt files found in Resources/{moleculesPath}");
        }
        else
        {
            var files = textAssets.Select(asset => asset.name).ToArray();
            moleculeDropdown.ClearOptions();
            moleculeDropdown.AddOptions(files.ToList());
        }

        var groups = MoleculeData.Molecule.LoadFunctionalGroups(functionalGroupsJSON).Select(group => group.name).ToArray();
        functionalGroupDropdown.ClearOptions();
        functionalGroupDropdown.AddOptions(groups.ToList());
    }
}
