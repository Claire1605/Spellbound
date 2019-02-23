using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

    public float x = 100.0f;
    public float z = 100.0f;
    public List<Hex> exits = new List<Hex>();
    public List<Hex> entrances = new List<Hex>();
    public Biome biome = new Biome();

    private List<GameObject> tempWalls = new List<GameObject>();
    private List<int> wallCounter = new List<int>();

    public void Start()
    {
        //Fixes the collider bug
        foreach (var child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Collider>())
            {
                child.GetComponent<Collider>().enabled = false;
                child.GetComponent<Collider>().enabled = true;
            }
        }
    }

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

    public void DeactivateDuplicateWalls(List<int> edgeIndices)
    {
        tempWalls.Clear();

        foreach (var wall in GetComponentsInChildren<Transform>())
        {
            if (wall.gameObject.tag == "HexWall")
            {
                tempWalls.Add(wall.gameObject);
            }
        }

        for (int i = 0; i < tempWalls.Count; i++)
        {
            if (edgeIndices.Contains(i))
            {
                tempWalls[i].SetActive(false);
            }
        }
    }

    public void RandomiseWalls()
    {
        tempWalls.Clear();
        wallCounter.Clear();
       
        foreach (var wall in GetComponentsInChildren<Transform>())
        {
            if (wall.gameObject.tag == "HexWall")
            {
                tempWalls.Add(wall.gameObject);
            }
        }

        for (int i = 0; i < tempWalls.Count; i++) // will be <6 if some children are deactivated
        {
            wallCounter.Add(i);
        }

        int wallLowerNumber = Random.Range(1, tempWalls.Count); //how many walls to lower
                                                    
        for (int i = 0; i < wallLowerNumber; i++)
        {
            int n = Random.Range(0, wallCounter.Count);

            Vector3 newPos = new Vector3(tempWalls[wallCounter[n]].transform.localPosition.x, -2, tempWalls[wallCounter[n]].transform.localPosition.z);
            tempWalls[wallCounter[n]].transform.localPosition = newPos;
            wallCounter.RemoveAt(n);
        }
    }
}
