using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static MoleculeData.Molecule;

namespace MoleculeData
{
    public class Molecule
    {
        public static Dictionary<string, (int Number, string Name, int Radius, Color Color, int valenceE)> elements = new Dictionary<string, (int, string, int, Color, int)>();
        public static List<FunctionalGroup> functionalGroups = new List<FunctionalGroup>();
        public class Atom
        {
            public string symbol;
            public int index;
            public int charge;
            public Vector3 offset;
            public List<Bond> bonds = new List<Bond>();

            public int atomicNumber;
            public int atomicRadius;
            public Color atomicColor;
            public int valenceElectrons;

            private Guid UUID { get; set; }

            public Atom(int index, string symbol, Vector3 offset)
            {
                this.index = index;
                this.symbol = symbol;
                this.offset = offset;

                if (elements.ContainsKey(symbol))
                {
                    this.atomicNumber = elements[symbol].Number;
                    this.atomicRadius = elements[symbol].Radius;
                    this.atomicColor = elements[symbol].Color;
                    this.valenceElectrons = elements[symbol].valenceE;
                }
                else
                {
                    this.atomicNumber = 0;
                    this.atomicRadius = 10;
                    this.atomicColor = Color.magenta;
                    this.valenceElectrons = 0;
                }

                this.charge = 0;

                UUID = Guid.NewGuid();
            }

            public Atom(int index, string symbol)
            {
                this.index = index;
                this.symbol = symbol;
                this.charge = 0;
                this.offset = Vector3.zero;
            }

            public void AddBond(Bond bond)
            {
                bonds.Add(bond);
            }

            public Bond GetBondWith(Atom atom)
            {
                foreach (Bond bond in bonds)
                {
                    if (bond.atom1.index == atom.index) return bond;
                    if (bond.atom2.index == atom.index) return bond;
                }
                return null;
            }

            public override string ToString()
            {
                string str = "Atom "+this.symbol+" ("+this.index+") | Bonds [";
                foreach (Bond bond in bonds)
                {
                    str += bond.index+": {"+bond.GetOther(this).ToShortString()+"},";
                }

                return str +"]";
            }

            public string ToShortString()
            {
                return this.symbol+" ("+this.index+")";
            }
            public int CalculateFormalCharge()
            {
                // This is actually a complicated thing to calculate without lone pair data in the molfile, deal with it later
                this.charge = 0;
                return 0;
            }

            public static bool operator ==(Atom obj1, Atom obj2)
            {
                if (ReferenceEquals(obj1, null))
                    return ReferenceEquals(obj2, null);

                return obj1.Equals(obj2);
            }

            public static bool operator !=(Atom obj1, Atom obj2)
            {
                return !(obj1 == obj2);
            }

            public override bool Equals(object obj)
            {
                if (obj is Atom other)
                {
                    return this.UUID == other.UUID;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return UUID.GetHashCode();
            }
        }

        public class Bond
        {
            public int index;
            public Atom atom1;
            public Atom atom2;
            public int bondType; // 1 = single, 2 = double, 3 = triple
            public int stereo;

            public Bond(int index, Atom atom1, Atom atom2, int bondType, int stereo)
            {
                this.index = index;
                this.atom1 = atom1;
                this.atom2 = atom2;
                this.bondType = bondType;
                this.stereo = stereo;
            }

            public Atom GetOther(Atom atom) 
            {
                if (atom == atom1) return this.atom2;
                if (atom == atom2) return this.atom1;
                return null;
            }
        }

        public static void LoadAtomData(TextAsset file){
            string content = file.text;
            string[] lines = content.Split("\n");


            // Iterate through each line
            foreach (string line in lines)
            {
                // Split the line into components
                string[] parts = line.Split(',');

                if (parts.Length == 6) // Ensure the line is properly formatted
                {
                    // Parse the atomic number
                    int atomicNumber = int.Parse(parts[0]);
                    string symbol = parts[1];
                    string name = parts[2];
                    int radius = int.Parse(parts[3]);
                    string colorStr = parts[4].Substring(2, 6);
                    int valenceE = Regex.Matches(parts[5], @"\d+").Select(match => int.Parse(match.Value.Last().ToString())).Sum();
                    // Add to the dictionary
                    elements[symbol] = (atomicNumber, name, radius, HexToColor(colorStr), valenceE);
                }
            }
            //Debug.Log("Atom Data loaded!");
        }

        private static Color HexToColor(string hex)
        {
            // Remove "0x" prefix if present
            if (hex.StartsWith("0x"))
            {
                hex = hex.Substring(2);
            }

            // Convert hex to Color (default to fully opaque)
            if (UnityEngine.ColorUtility.TryParseHtmlString("#" + hex, out Color color))
            {
                return color;
            }

            // Return white if parsing fails
            return Color.white;
        }

        public List<Atom> atoms = new List<Atom>();
        public List<Bond> bonds = new List<Bond>();
        public List<int> chiralCenters = new List<int>();

        private Molecule(List<Atom> atoms, List<Bond> bonds) { 
            this.atoms = atoms;
            this.bonds = bonds;
        }

        public Molecule(TextAsset file)
        {
            var content = file.text;
            var lines = content.Split("\n");

            bool parsingAtoms = true;  // Flag to differentiate between atom and bond sections
            bool startParsing = false; // Flag to determine when to start parsing after headers
            int lineN = 0;

            foreach (var line in lines)
            {
                lineN++;
                if (!startParsing)
                {
                    if (line.Length >= 10 && line.Substring(line.Length - 10).Contains("V2000") || line.Length < 10 && line.Contains("V2000"))
                    {
                        startParsing = true;
                        continue;
                    }
                    else
                    {
                        continue; // Skip header lines
                    }
                }

                // Stop parsing when "M END" is reached
                if (line.Trim() == "M  END")
                    break;
                if (line.StartsWith("M  CHG") || line.StartsWith("M  ISO")) // TODO, handle irregular charges and isotopes
                    continue; 

                // Detect transition between atoms and bonds based on line length
                if (line.Length < 69)
                    parsingAtoms = false;

                // Atom section (coordinates and element symbol)
                if (parsingAtoms)
                {
                    // Parse atom data (x, y, z coordinates and element symbol)
                    float x = float.Parse(line.Substring(0, 10).Trim(), CultureInfo.InvariantCulture);
                    float y = float.Parse(line.Substring(10, 10).Trim(), CultureInfo.InvariantCulture);
                    float z = float.Parse(line.Substring(20, 10).Trim(), CultureInfo.InvariantCulture);
                    string symbol = line.Substring(31, 2).Trim();

                    atoms.Add(new Atom(atoms.Count,symbol, new Vector3(x,y,z)));
                }
                // Bond section
                else if (!parsingAtoms && line.Length >= 9)
                {
                    // Parse bond data (first atom index, second atom index, bond type, stereo configuration)
                    int firstAtomIndex = int.Parse(line.Substring(0, 3).Trim());
                    int secondAtomIndex = int.Parse(line.Substring(3, 3).Trim());
                    int bondType = int.Parse(line.Substring(6, 3).Trim());
                    int stereo = line.Length > 9 ? int.Parse(line.Substring(9, 3).Trim()) : 0;

                    Atom firstAtom = atoms[firstAtomIndex-1];
                    Atom secondAtom = atoms[secondAtomIndex-1];
                    Bond newBond = new Bond(bonds.Count,firstAtom,secondAtom,bondType,stereo);

                    bonds.Add(newBond);
                    firstAtom.AddBond(newBond);
                    secondAtom.AddBond(newBond);
                }
            }

            foreach (Atom atom in atoms)
            {
                if (IsChiralCenter(atom.index))
                {
                    chiralCenters.Add(atom.index);
                }
            }
        }

        public float GetXSize()
        {
            int index = 0;
            float size = 0;
            foreach (Atom atom in atoms)
            {
                if (Math.Abs(atom.offset.x) > size) {
                    size = Math.Abs(atom.offset.x);
                    index = atom.index;
                }
            }
            return size + atoms[index].atomicRadius / 100.0f;
        }

        public float GetYSize()
        {
            int index = 0;
            float size = 0;
            foreach (Atom atom in atoms)
            {
                if (Math.Abs(atom.offset.y) > size)
                {
                    size = Math.Abs(atom.offset.y);
                    index = atom.index;
                }
            }
            return size + atoms[index].atomicRadius / 100.0f;
        }

        public float GetZSize()
        {
            int index = 0;
            float size = 0;
            foreach (Atom atom in atoms)
            {
                if (Math.Abs(atom.offset.z) > size)
                {
                    size = Math.Abs(atom.offset.z);
                    index = atom.index;
                }
            }
            return size + atoms[index].atomicRadius / 100.0f;
        }

        private bool IsChiralCenter(int index)
        {
            if (index >= atoms.Count || index < 0)
            {
                return false;
            }

            Atom chiralAtom = atoms[index];
            if (chiralAtom == null) return false;

            string symbol = chiralAtom.symbol;
            if (!symbol.Equals("C") && !symbol.Equals("S") && !symbol.Equals("N") && !symbol.Equals("P")) return false; // Technically more atoms can be chiral, but that requires better charge data.
            if (symbol.Equals("C") && chiralAtom.bonds.Count != 4) return false;
            if (!symbol.Equals("C") && chiralAtom.bonds.Count != 3 && chiralAtom.bonds.Count != 4) return false;


            CIPNode chiralRoot = BuildDigraph(index);
            List<CIPNode> sortedChildren = chiralRoot.OrderChildren();


            for (int i = 0; i < sortedChildren.Count - 1; i++)
            {
                if (Molecule.CIPCompare(sortedChildren[i], sortedChildren[i + 1]) == null)
                {
                    return false;
                }
            }

            // Verify that the directly connected atoms are aplanar
            List<Vector3> points = new List<Vector3>();
            points.Add(chiralAtom.offset);
            foreach (Bond bond in chiralAtom.bonds)
            {
                points.Add(bond.GetOther(chiralAtom).offset);
            }
            var planeData = MathUtils.MathUtils.FindPlane(points);

            //Debug.Log(planeData);

            float sumDist = 0;
            foreach (Vector3 point in points)
            {
                float dist = MathUtils.MathUtils.PointToPlaneDistance(point, planeData.Item1, planeData.Item2);
                sumDist += dist;
            }

            return sumDist > .5;
        }

        public bool IsChiral(int index) {
            return chiralCenters.Contains(index);
        }

        private CIPNode BuildDigraph(int rootIndex)
        {
            if (rootIndex >= atoms.Count || rootIndex < 0) return null;
            
            List<int> terminals = new List<int>();
            CIPNode root = GetCIPNode(atoms[rootIndex], null, terminals);

            return root;
        }


        private CIPNode GetCIPNode(Atom atom, CIPNode parent, List<int> terminals)
        {
            CIPNode node = new CIPNode(atom);
            if (parent != null)
            {
                node.parent = parent;
            }
            
            foreach (Bond bond in atom.bonds) {
                Atom otherAtom = bond.GetOther(atom);
                if (terminals.Contains(otherAtom.index)) continue;

                List<int> terminalCopy = terminals.ToList();
                terminalCopy.Add(atom.index);
                CIPNode child = GetCIPNode(otherAtom, node, terminalCopy);
                child.layer = node.layer + 1;
                node.children.Add(child);

                if (bond.bondType == 2 || bond.bondType == 3)
                {
                    node.children.Add(MakePhantomNode(otherAtom, node));
                    child.children.Add(MakePhantomNode(atom, child));

                    if (bond.bondType == 3)
                    {
                        node.children.Add(MakePhantomNode(otherAtom, node));
                        child.children.Add(MakePhantomNode(atom, child));
                    }
                }

                node.AddSubNode();
                node.SetLayerDepth(1);
            }

            return node;
        }

        private CIPNode MakePhantomNode(Atom atom, CIPNode parent)
        {
            CIPNode node = new CIPNode(atom);
            node.parent = parent;
            CIPNode zeroNode1 = new CIPNode(new Atom(-1, "-"));
            CIPNode zeroNode2 = new CIPNode(new Atom(-1, "-"));
            CIPNode zeroNode3 = new CIPNode(new Atom(-1, "-"));
            node.children.Add(zeroNode1);
            node.children.Add(zeroNode2);
            node.children.Add(zeroNode3);
            node.AddSubNode();
            node.AddSubNode();
            node.AddSubNode();
            node.SetLayerDepth(1);
            return node;
        }

        private static CIPNode CIPCompare(CIPNode root1, CIPNode root2)
        {
            // Sphere 1 root comparison
            if (root1.atom.atomicNumber > root2.atom.atomicNumber) return root1;
            if (root2.atom.atomicNumber > root1.atom.atomicNumber) return root2;
            if (root1.children.Count == 0 && root2.children.Count == 0) return null;

            // Sphere 2 child comparison
            List<CIPNode> children1 = root1.OrderChildren();
            List<CIPNode> children2 = root2.OrderChildren();

            int minLength = Math.Min(children1.Count, children2.Count);

            for (int i = 0; i < minLength; i++)
            {
                if (children1[i].atom.atomicNumber > children2[i].atom.atomicNumber)
                {
                    return root1;
                }
                else if (children1[i].atom.atomicNumber < children2[i].atom.atomicNumber)
                {
                    return root2;
                }
            }

            if (children1.Count != children2.Count)
            {
                return children1.Count > children2.Count ? root1 : root2;
            }

            // Sphere 3 recursive comparison
            CIPNode recursiveResult = CIPCompare(children1[0], children2[0]);

            if (recursiveResult == null)
            {
                return null;
            } 
            else if (recursiveResult.Equals(children1[0]))
            {
                return root1;
            }
            else if (recursiveResult.Equals(children2[0]))
            {
                return root2;
            }

            return null;
        }

        private class CIPNode {
            public CIPNode parent { set; get; }
            public Atom atom { set; get; }
            public List<CIPNode> children { set; get; }
            public int layer { get; set; }
            public int maxDepth { set; get; }
            public int totalSubNodes { set; get; }

            public CIPNode(Atom atom) { 
                this.atom = atom;
                this.layer = 0;
                this.maxDepth = 0;
                this.totalSubNodes = 0;
                this.children = new List<CIPNode>();
            }

            public void AddSubNode()
            {
                totalSubNodes++;
                if (parent != null) parent.AddSubNode();
            }

            public void SetLayerDepth(int depth) {
                if (maxDepth < depth) maxDepth = depth;
                if (parent != null) parent.SetLayerDepth(maxDepth + 1);
            }

            public List<CIPNode> OrderChildren() {
                if (children.Count == 0 || children.Count == 1) return children;

                return children.OrderByDescending(node => node, Comparer<CIPNode>.Create((node1, node2) =>
                {
                    CIPNode greaterNode = Molecule.CIPCompare(node1, node2);
                    if (greaterNode == null) return 0;
                    return greaterNode == node1 ? 1 : -1;
                })).ToList();
            }
        }



        [System.Serializable]
        public class FunctionalGroup
        {
            public string name;
            public List<string> atoms;
            public List<FunctionalGroupBond> bonds;
            public List<string> children;

            public Molecule GetAsMolecule() {
                List<Atom> molAtoms = new List<Atom>();
                List<Bond> molBonds = new List<Bond>();
                int i = 0;
                foreach (var sym in atoms)
                {
                    molAtoms.Add(new Atom(i, sym));
                    i++;
                }
                i = 0;
                foreach (var bond in bonds)
                {
                    Atom atom1 = molAtoms[bond.atom1];
                    Atom atom2 = molAtoms[bond.atom2];
                    Bond newBond = new Bond(i, atom1, atom2, bond.type, 0);
                    atom1.AddBond(newBond);
                    atom2.AddBond(newBond);
                    molBonds.Add(newBond);
                }
                return new Molecule(molAtoms, molBonds);
            }
        }

        [System.Serializable]
        public class FunctionalGroupBond
        {
            public int atom1;
            public int atom2;
            public int type;
        }

        [System.Serializable]
        public class FunctionalGroupRoot
        {
            public List<FunctionalGroup> FunctionalGroups;
        }

        public static List<FunctionalGroup> LoadFunctionalGroups(TextAsset jsonAsset) 
        {
            FunctionalGroupRoot root = JsonUtility.FromJson<FunctionalGroupRoot>(jsonAsset.text);

            if (root == null || root?.FunctionalGroups == null) {
                Debug.Log("Error in parsing JSON");
                return Molecule.functionalGroups;
            }

            Molecule.functionalGroups.Clear();
            foreach (var group in root.FunctionalGroups)
            {
                functionalGroups.Add(group);
            }
            return Molecule.functionalGroups;
        }

        private HashSet<string> memoizedStates = new HashSet<string>();

        public List<Dictionary<int, int>> GetFunctionalGroup(string name)
        {
            if (name == null || functionalGroups.Find(group => group.name.Equals(name)) == null) return new List<Dictionary<int, int>>();
            FunctionalGroup group = functionalGroups.Find(group => group.name.Equals(name));

            var candidateAtoms = PrecomputeCandidates(group);
            if (candidateAtoms.Any(c => c.Value.Count == 0))
            {
                return new List<Dictionary<int, int>>();
            }

            var mappings = new List<Dictionary<int, int>>();

            memoizedStates = new HashSet<string>();
            MatchFunctionalGroup(group, new Dictionary<int, int>(), mappings, candidateAtoms);
            RemoveDuplicateDictionaries(mappings);

            foreach (string child in group.children) {
                FunctionalGroup childGroup = functionalGroups.Find(group => group.name.Equals(child));
                if (childGroup == null) continue;
                List<Dictionary<int, int>> childGroups = GetFunctionalGroup(childGroup.name);
                RemoveChildGroups(mappings, childGroups);
            }

            //Debug.Log($"Found group {name} a total of {mappings.Count} times");
            //foreach (var mapping in mappings)
            //{
            //    Debug.Log($"Mapping: {string.Join(", ", mapping.Select(kvp => $"GroupAtom {kvp.Key} -> MoleculeAtom {kvp.Value}"))}");
            //}

            return mappings;
        }

        private void MatchFunctionalGroup(FunctionalGroup group, Dictionary<int, int> mapping, List<Dictionary<int, int>> mappings, Dictionary<int, HashSet<int>> candidateAtoms)
        {
            if (mapping.Count == group.atoms.Count)
            {
                if (ValidateBonds(group, mapping)) // All functional group atoms are mapped; validate bonds
                {
                    mappings.Add(new Dictionary<int, int>(mapping)); // Store a copy of the current mapping
                }
                return;
            }

            int groupAtomIndex = mapping.Count; // Try to map the next atom in the group
            string groupAtomSymbol = group.atoms[groupAtomIndex];

            foreach (var candidateAtomIndex in candidateAtoms[groupAtomIndex])
            {
                if (!mapping.ContainsValue(candidateAtomIndex))
                {
                    mapping[groupAtomIndex] = candidateAtomIndex;

                    // Serialize the current mapping state as a unique string
                    string stateKey = string.Join(",", mapping.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
                    if (!memoizedStates.Contains(stateKey))
                    {
                        if (ValidatePartialBonds(group, mapping))
                        {
                            memoizedStates.Add(stateKey);
                            MatchFunctionalGroup(group, mapping, mappings, candidateAtoms);
                        }
                    }

                    mapping.Remove(groupAtomIndex);
                }
            }
        }

        private Dictionary<int, HashSet<int>> PrecomputeCandidates(FunctionalGroup group)
        {
            var candidateAtoms = new Dictionary<int, HashSet<int>>();

            for (int i = 0; i < group.atoms.Count; i++)
            {
                string groupAtomSymbol = group.atoms[i];
                candidateAtoms[i] = new HashSet<int>(atoms.Where(atom => AtomMatches(groupAtomSymbol, atom.symbol)).Select(atom => atom.index));
            }

            return candidateAtoms;
        }

        private bool AtomMatches(string groupSymbol, string moleculeSymbol)
        {
            if (Regex.IsMatch(groupSymbol, @"^R(\d+)?$"))
                return true; // Matches any atom
            if (groupSymbol == "X")
                return moleculeSymbol is "F" or "Cl" or "Br" or "I"; // Matches halogens
            return groupSymbol == moleculeSymbol;
        }

        private bool ValidatePartialBonds(FunctionalGroup group, Dictionary<int, int> mapping)
        {
            // Iterate over all bonds in the functional group
            foreach (var bond in group.bonds)
            {
                if (mapping.ContainsKey(bond.atom1) && mapping.ContainsKey(bond.atom2))
                {
                    int mappedAtom1 = mapping[bond.atom1];
                    int mappedAtom2 = mapping[bond.atom2];

                    // Check if bond between atoms in functional group is valid in the molecule
                    if (!AreAtomsConnected(mappedAtom1, mappedAtom2))
                    {
                        return false; // Invalid bond, prune the branch
                    }
                }
            }
            return true;
        }

        private bool AreAtomsConnected(int atom1Index, int atom2Index)
        {
            foreach (var bond in bonds)
            {
                if ((bond.atom1.index == atom1Index && bond.atom2.index == atom2Index) ||
                    (bond.atom1.index == atom2Index && bond.atom2.index == atom1Index))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ValidateBonds(FunctionalGroup group, Dictionary<int, int> mapping)
        {
            // Check that all bonds that should exist do exist
            var bondMap = new Dictionary<(int, int), int>();

            foreach (var bond in bonds)
            {
                bondMap[(bond.atom1.index, bond.atom2.index)] = bond.bondType;
                bondMap[(bond.atom2.index, bond.atom1.index)] = bond.bondType;
            }

            foreach (var groupBond in group.bonds)
            {
                int mappedAtom1 = mapping[groupBond.atom1];
                int mappedAtom2 = mapping[groupBond.atom2];

                if (!bondMap.TryGetValue((mappedAtom1, mappedAtom2), out int bondType) || bondType != groupBond.type)
                {
                    return false;
                }
            }

            // Check that no internal bonds that shouldn't be there exist
            var groupAtoms = new HashSet<int>(mapping.Values);

            foreach (var (key, value) in bondMap)
            {
                if (groupAtoms.Contains(key.Item1) && groupAtoms.Contains(key.Item2))
                {
                    // Both atoms are in the group, check if this bond is defined in group.bonds
                    if (!group.bonds.Any(bond =>
                    {
                        int mappedAtom1 = mapping[bond.atom1];
                        int mappedAtom2 = mapping[bond.atom2];
                        return (mappedAtom1 == key.Item1 && mappedAtom2 == key.Item2 && bond.type == value) ||
                               (mappedAtom1 == key.Item2 && mappedAtom2 == key.Item1 && bond.type == value);
                    }))
                    {
                        // If no matching bond is found in group.bonds, return false
                        return false;
                    }
                }
            }

            // Check that no internal-external bonds between non-R atoms exist
            foreach (var (key, value) in bondMap)
            {
                bool atom1InGroup = groupAtoms.Contains(key.Item1);
                bool atom2InGroup = groupAtoms.Contains(key.Item2);

                // If one atom is in the group and the other is not, this is an internal-external bond
                if (atom1InGroup != atom2InGroup)
                {
                    int atomToFind = atom1InGroup ? key.Item1 : key.Item2;

                    foreach (var pair in mapping)
                    {
                        if (pair.Value == atomToFind)
                        {
                            if (!Regex.IsMatch(group.atoms[pair.Key], @"^R(\d+)?$")) return false; // Found non-R bond
                        }
                    }
                }
            }

            return true;
        }

        private void RemoveDuplicateDictionaries(List<Dictionary<int, int>> mappings)
        {
            var seenValueSets = new HashSet<HashSet<int>>(new HashSetComparer<int>());

            // Iterate through the list and retain only unique value sets
            var uniqueMappings = mappings.Where(dict =>
            {
                var valueSet = new HashSet<int>(dict.Values);
                return seenValueSets.Add(valueSet); // Add returns false if it's a duplicate
            }).ToList();

            mappings.Clear();
            mappings.AddRange(uniqueMappings);
        }

        private void RemoveChildGroups(List<Dictionary<int, int>> mappings, List<Dictionary<int, int>> children)
        {
            var childValueSets = children.Select(parent => new HashSet<int>(parent.Values)).ToList();

            mappings.RemoveAll(mapping =>
            {
                var mappingValues = new HashSet<int>(mapping.Values);
                return childValueSets.Any(childValues => mappingValues.IsSubsetOf(childValues));
            });
        }
    }


    class HashSetComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            return x.SetEquals(y); // Check if the sets have the same elements
        }

        public int GetHashCode(HashSet<T> obj)
        {
            // Use XOR of hashes of all elements in the set to generate a unique hash
            return obj.Aggregate(0, (current, item) => current ^ item.GetHashCode());
        }
    }
}
