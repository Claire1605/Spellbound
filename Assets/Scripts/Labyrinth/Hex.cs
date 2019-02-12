using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

    public float x = 100.0f;
    public float z = 100.0f;
    public List<Hex> exits = new List<Hex>();
    public List<Hex> entrances = new List<Hex>();
    public Biome biome = new Biome();

    public void Initialise(bool editMode, GameObject parent, float xCoord, float zCoord, int xIndex, int zIndex, float scale, Biome biome)
    {
        transform.parent = parent.transform;
        transform.localScale *= scale;
        x = xCoord;
        z = zCoord;
        transform.position = new Vector3(x, 0, z);
        name = "Hex-" + (xIndex).ToString() + "-" + (zIndex).ToString();
        if (editMode)
        {
            GetComponent<MeshRenderer>().sharedMaterial = biome.mat;
        }
        else
        {
            GetComponent<MeshRenderer>().material = biome.mat;
        }
    }
}
