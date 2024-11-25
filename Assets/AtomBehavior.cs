using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomBehavior : MonoBehaviour
{
    public Material glowMaterial;
    public Material normalMaterial;
    [Layer]
    public int atomLayer;

    public Color hoverColor;
    public Color selectedColor;
    public Color correctColor;
    public Color incorrectColor;
    public Color missedColor;
    public Color defaultColor;

    private GameObject moleculeContainer;
    private MoleculeObject moleculeObject;
    private MoleculeData.Molecule molecule;
    private MoleculeData.Molecule.Atom atom;

    public bool isHovered = false;
    public bool isSelected = false;
    private Color currentColor;
    private Color savedColor;

    private float lastSelTime = -100f;

    // Start is called before the first frame update
    void Start()
    {
        currentColor = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hitThis = false;

        if (Physics.Raycast(ray, out hit, 100, (1 << atomLayer)))
        {
            if (hit.transform == transform)
            {
                hitThis = true;

                if (Input.GetMouseButtonDown(0) && Time.time >= lastSelTime + 0.5f)
                {
                    lastSelTime = Time.time;
                    isSelected = !isSelected;

                    if (isSelected)
                    {
                        //Debug.Log($"Atom ({atom.index}): Selected");
                    }
                    else
                    {
                        //Debug.Log($"Atom ({atom.index}): De-Selected");
                    }
                }
            }
            else
            {
                
            }
        }

        if (hitThis)
        {
            if (!isHovered)
            {
                isHovered = true;
            }
        }
        else
        {
            if (isHovered)
            {
                isHovered = false;
            }
        }
       

        ApplyOutline(GetHighlightColor());
    }

    Color GetHighlightColor() {
        if (isHovered)
        {
            return hoverColor;
        }
        else if (isSelected)
        {
            return selectedColor;
        }
        else if (savedColor != null)
        {
            return savedColor;
        }

        return defaultColor;
    }

    public void Initialize(MoleculeData.Molecule molecule, MoleculeData.Molecule.Atom atom, GameObject moleculeContainer, MoleculeObject moleculeObject) 
    { 
        this.molecule = molecule;
        this.atom = atom;
        this.moleculeContainer = moleculeContainer;
        this.moleculeObject = moleculeObject;

        Vector3 position = atom.offset;
        transform.position = position;
        float scale = atom.atomicRadius * moleculeObject.scaleFactor;
        transform.localScale = new Vector3(scale, scale, scale);
        var renderer = GetComponent<Renderer>();
        renderer.material = normalMaterial;
        renderer.material.color = atom.atomicColor;
        transform.SetParent(moleculeContainer.transform);
        gameObject.layer = atomLayer;
    }

    public void SetColor(Color color)
    { 
        savedColor = color;
        ApplyOutline(color);
    }

    // Function to apply glowing outline to the atom
    void ApplyOutline(Color glowColor)
    {
        if (glowColor.Equals(currentColor)) {
            return;
        }

        //Debug.Log($"Atom ({atom.index}): Trying to change outline color from {currentColor} to {glowColor}");

        var renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (glowColor.a == 0)
            {
                currentColor = glowColor;
                RemoveOutline();
            }
            else
            {
                Color atomColor = renderer.material.color;
                renderer.material = glowMaterial;
                renderer.material.color = atomColor;
                renderer.material.SetColor("_OutlineColor", glowColor);
                currentColor = glowColor;
            }
            
        }
    }

    // Function to remove glowing outline from the atom
    void RemoveOutline()
    {
        var renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            renderer.material = normalMaterial;
            renderer.material.color = color;
        }
    }
}
