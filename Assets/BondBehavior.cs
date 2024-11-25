using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondBehavior : MonoBehaviour
{
    public Material normalMaterial;
    [Layer]
    public int bondLayer;

    public Color defaultColor;

    private GameObject moleculeContainer;
    private MoleculeObject moleculeObject;
    private MoleculeData.Molecule molecule;
    private MoleculeData.Molecule.Bond bond;

    private Color currentColor;

    private float bondLength = 1f;

    // Start is called before the first frame update
    void Start()
    {
        currentColor = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Initialize(MoleculeData.Molecule molecule, MoleculeData.Molecule.Bond bond, GameObject moleculeContainer, MoleculeObject moleculeObject)
    {
        this.molecule = molecule;
        this.bond = bond;
        this.moleculeContainer = moleculeContainer;
        this.moleculeObject = moleculeObject;

        Vector3 atom1Pos = bond.atom1.offset;
        Vector3 atom2Pos = bond.atom2.offset;

        int numberOfBonds = bond.bondType;
        if (numberOfBonds > 3)
        {
            numberOfBonds = 1;
        }

        Vector3 start = atom1Pos;
        Vector3 end = atom2Pos;
        Vector3 midPoint = (start + end) / 2f;
        // Calculate direction perpendicular to the bond for parallel placement
        Vector3 bondDirection = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(bondDirection, Vector3.forward).normalized * 0.1f; // Adjust spacing between bonds

        transform.position = midPoint;
        transform.up = end - start;

        if (perpendicular == Vector3.zero) // Fallback if parallel to the world up vector
        {
            perpendicular = Vector3.Cross(bondDirection, Vector3.right).normalized * 0.1f;
        }

        bondLength = Vector3.Distance(start, end);
        Vector3 relativeStart = start - midPoint;
        Vector3 relativeEnd = end - midPoint;

        // Create additional parallel bonds
        if (numberOfBonds == 1)
        {
            CreateBondCylinder(Vector3.zero);
        }
        else if (numberOfBonds == 2)
        {
            CreateBondCylinder(perpendicular);
            CreateBondCylinder(-perpendicular);
        }
        else if (numberOfBonds == 3)
        {
            perpendicular *= 2;
            CreateBondCylinder(Vector3.zero);
            CreateBondCylinder(perpendicular);
            CreateBondCylinder(-perpendicular);
        }

        transform.SetParent(moleculeContainer.transform);
        gameObject.layer = bondLayer;
    }

    private GameObject CreateBondCylinder(Vector3 offset)
    {
        GameObject bondCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bondCylinder.transform.SetParent(transform);
        bondCylinder.transform.position = transform.position + offset;
        bondCylinder.transform.up = transform.up;
        bondCylinder.transform.localScale = new Vector3(5 * moleculeObject.scaleFactor, bondLength / 2f, 5 * moleculeObject.scaleFactor); // Adjust the radius (X and Z) and height (Y)
        bondCylinder.layer = bondLayer;
        var renderer = bondCylinder.GetComponent<Renderer>();
        renderer.material = normalMaterial;
        renderer.material.color = defaultColor;
        return bondCylinder;
    }
}
