using MoleculeData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static MoleculeData.Molecule;

public class MoleculeObject : MonoBehaviour
{
    [SerializeField] private UnityEngine.TextAsset atomData;
    [SerializeField] private UnityEngine.TextAsset functionalGroups;
    private bool isLoading = false;
    private Molecule molecule;

    public float rotationSpeed = 1000f; // Adjust the speed of rotation
    public float scrollSpeed = 5f; // Adjust the speed of camera movement
    private bool rotating = false;

    private Dictionary<GameObject, Molecule.Atom> atomObjects = new Dictionary<GameObject, Molecule.Atom>();
    private Dictionary<GameObject, Molecule.Bond> bondObjects = new Dictionary<GameObject, Molecule.Bond>();

    public GameObject atomPrefab;
    public GameObject bondPrefab;
    public float scaleFactor = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        Molecule.LoadAtomData(atomData);
        Molecule.LoadFunctionalGroups(functionalGroups);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton((int)UnityEngine.UIElements.MouseButton.RightMouse))
        {
            if (!rotating)
            {
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition); // Change to mainCamera.transform.forward for a forward raycast
                RaycastHit[] hits = Physics.RaycastAll(cameraRay, 100);
                GameObject boundsObject = GameObject.Find("MoleculeBounds");

                // Perform the raycast and check if it hits this object
                foreach (RaycastHit hitObject in hits)
                {
                    if (boundsObject != null && hitObject.transform == boundsObject.transform)
                    {
                        rotating = true;
                    }
                }
            }
            else
            {
                Vector3 yRotVec = Vector3.up;
                Vector3 cameraVec = Camera.main.transform.forward;
                Vector3 rotVec = Vector3.Cross(yRotVec, cameraVec);

                // Get mouse movement input
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");


                GameObject containerObject = GameObject.Find("MoleculeContainer");
                containerObject.transform.Rotate(transform.InverseTransformDirection(rotVec), mouseY * rotationSpeed * Time.deltaTime);
                containerObject.transform.Rotate(transform.InverseTransformDirection(yRotVec), Math.Sign(Vector3.Cross(yRotVec, cameraVec).magnitude) * -mouseX * rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            rotating = false;
        }
    }

    void GeneratePlaneCyl(GameObject atom)
    {
        //bool isChiral = molecule.IsChiral(atomObjects[atom].index);
        //Debug.Log($"Checking chirality of atom {atomObjects[atom].index} @ {atomObjects[atom].offset}: {isChiral}");


        //Atom chiralAtom = atomObjects[atom];
        //List<Vector3> points = new List<Vector3>();
        //points.Add(chiralAtom.offset);
        //foreach (Bond bond in chiralAtom.bonds)
        //{
        //    points.Add(bond.GetOther(chiralAtom).offset);
        //}

        //var result = MathUtils.MathUtils.FindPlane(points);

        //GameObject newCyl = CreateBondCylinder(result.Item2, result.Item1 + result.Item2);
        //newCyl.transform.localScale = new Vector3(0.15f, 2f, 0.15f);

        //float sumDist = 0;
        //foreach (Vector3 point in points)
        //{
        //    float dist = MathUtils.MathUtils.PointToPlaneDistance(point, result.Item1, result.Item2);
        //    Debug.Log($"Dist to plane from {point} is {dist}");
        //    sumDist += dist;
        //}
        //Debug.Log($"Sum of Coplanar Difference: {sumDist}");

        //var renderer = newCyl.GetComponent<Renderer>();
        //renderer.material.color = Color.magenta;
    }


    public void LoadMoleculeFromFile() {
        if (isLoading) return;
        TMP_Dropdown moleculeSelector = GameObject.Find("MoleculeSelector").GetComponent<TMP_Dropdown>();
        if (moleculeSelector == null) return;
        string selectedOption = moleculeSelector.options[moleculeSelector.value].text;
        UnityEngine.TextAsset moleculeFile = Resources.Load<UnityEngine.TextAsset>("Molecules/" + selectedOption);
        if (moleculeFile == null)
        {
            Debug.Log("File not found: " + selectedOption);
            return;
        }
        isLoading = true;

        GameObject containerObject = GameObject.Find("MoleculeContainer");
        foreach (Transform child in containerObject.transform)
        {
            Destroy(child.gameObject);
        }
        atomObjects = new Dictionary<GameObject, Molecule.Atom> ();

        molecule = new Molecule(moleculeFile);

        GameObject boundObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boundObject.transform.position = Vector3.zero;
        boundObject.transform.SetParent(containerObject.transform);
        boundObject.transform.localScale = new Vector3(2 * molecule.GetXSize(), 2 * molecule.GetYSize(), 2 * molecule.GetZSize());
        boundObject.GetComponent<Renderer>().enabled = false;
        boundObject.name = "MoleculeBounds";
        

        foreach (var atom in molecule.atoms)
        {
            GameObject atomSphere = Instantiate(atomPrefab, atom.offset, Quaternion.identity, containerObject.transform);
            AtomBehavior atomBehavior = atomSphere.GetComponent<AtomBehavior>();
            atomBehavior.Initialize(molecule, atom, containerObject, this);
            atomObjects.Add(atomSphere, atom);
        }


        foreach (var bond in molecule.bonds)
        {
            GameObject bondCyl = Instantiate(bondPrefab, Vector3.zero, Quaternion.identity, containerObject.transform);
            BondBehavior bondBehavior = bondCyl.GetComponent<BondBehavior>();
            bondBehavior.Initialize(molecule, bond, containerObject, this);
            bondObjects.Add(bondCyl, bond);
        }
        isLoading = false;
    }

    public void CheckChirality()
    {
        if (molecule == null) return;

        foreach (var atomEntry in atomObjects)
        {
            GameObject atomObject = atomEntry.Key;
            Atom atom = atomEntry.Value;
            AtomBehavior atomBehavior = atomObject.GetComponent<AtomBehavior>();

            bool isChiral = molecule.IsChiral(atom.index);
            bool isSelected = atomBehavior.isSelected;

            if (isChiral && isSelected)
            {
                atomBehavior.SetColor(atomBehavior.correctColor);
            }
            else if (isChiral && !isSelected)
            {
                atomBehavior.SetColor(atomBehavior.missedColor);
            }
            else if (!isChiral && isSelected)
            {
                atomBehavior.SetColor(atomBehavior.incorrectColor);
            }
            else
            {
                atomBehavior.SetColor(atomBehavior.defaultColor);
            }

            atomBehavior.isSelected = false;
        }
    }

    public void CheckFunctionalGroup()
    {
        if (molecule == null) return;
        TMP_Dropdown moleculeSelector = GameObject.Find("GroupSelector").GetComponent<TMP_Dropdown>();
        if (moleculeSelector == null) return;
        string selectedOption = moleculeSelector.options[moleculeSelector.value].text;
        List<Dictionary<int,int>> mappings = molecule.GetFunctionalGroup(selectedOption);

        List<int> allAtoms = new List<int>();
        foreach (var dictionary in mappings)
        {
            allAtoms.AddRange(dictionary.Values);
        }

        foreach (var atomEntry in atomObjects)
        {
            GameObject atomObject = atomEntry.Key;
            Atom atom = atomEntry.Value;
            AtomBehavior atomBehavior = atomObject.GetComponent<AtomBehavior>();

            bool isInGroup = allAtoms.Contains(atom.index);
            bool isSelected = atomBehavior.isSelected;

            if (isInGroup && isSelected)
            {
                atomBehavior.SetColor(atomBehavior.correctColor);
            }
            else if (isInGroup && !isSelected)
            {
                atomBehavior.SetColor(atomBehavior.missedColor);
            }
            else if (!isInGroup && isSelected)
            {
                atomBehavior.SetColor(atomBehavior.incorrectColor);
            }
            else
            {
                atomBehavior.SetColor(atomBehavior.defaultColor);
            }

            atomBehavior.isSelected = false;
        }
    }
}
