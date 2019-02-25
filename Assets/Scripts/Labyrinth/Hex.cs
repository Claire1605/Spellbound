using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

    public float x = 100.0f;
    public float z = 100.0f;
    public int hex_X = 0;
    public int hex_Z = 0;
    public int wallHeight = 1;
    public List<Hex> exits = new List<Hex>();
    public List<Hex> entrances = new List<Hex>();
    public Biome biome = new Biome();

    private List<GameObject> tempWalls = new List<GameObject>();
    private List<GameObject> tempColumns = new List<GameObject>();
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
        hex_X = xIndex;
        hex_Z = zIndex;
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

    public void DeactivateDuplicateWalls(List<int> edgeIndices, List<int> columnIndices)
    {
        tempWalls.Clear();
        tempColumns.Clear();

        foreach (var wall in GetComponentsInChildren<Transform>())
        {
            if (wall.gameObject.tag == "HexWall")
            {
                tempWalls.Add(wall.gameObject);
            }
            if (wall.gameObject.tag == "HexColumn")
            {
                tempColumns.Add(wall.gameObject);
            }
        }

        for (int i = 0; i < tempWalls.Count; i++)
        {
            if (edgeIndices.Contains(i))
            {
                tempWalls[i].SetActive(false);
            }
        }
        for (int i = 0; i < tempColumns.Count; i++)
        {
            if (columnIndices.Contains(i))
            {
                tempColumns[i].SetActive(false);
            }
        }
    }

    public void RandomiseWalls(List<int> edgeIndicesToIgnore)
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
            if (!edgeIndicesToIgnore.Contains(i))
            {
                wallCounter.Add(i);
            }   
        }

        int wallLowerNumber = Random.Range(1, wallCounter.Count); //how many walls to lower
                                                    
        for (int i = 0; i < wallLowerNumber; i++)
        {
            int n = Random.Range(0, wallCounter.Count);

            Vector3 newPos = new Vector3(tempWalls[wallCounter[n]].transform.localPosition.x, -2, tempWalls[wallCounter[n]].transform.localPosition.z);
            tempWalls[wallCounter[n]].transform.localPosition = newPos;
            wallCounter.RemoveAt(n);
        }
    }

    public void SetColumnHeights(int hexX, int hexZ, int hexScale)
    {
        List<int> wallHeights = new List<int>();

        tempColumns.Clear();

        foreach (var child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.tag == "HexColumn")
            {
                tempColumns.Add(child.gameObject);
            }
        }

        foreach (var column in tempColumns)
        {
            wallHeights.Clear();
            int colHeight = 0;

            if (column.name == "ColumnNNW")
            {
                if (transform.Find("HexWall(Clone)/WallNorth").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallNorth").transform.localPosition.y != -2) //x.z
                {
                    wallHeights.Add(wallHeight);
                }
                if (transform.Find("HexWall(Clone)/WallNorthWest").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallNorthWest").transform.localPosition.y != -2)
                {
                    wallHeights.Add(wallHeight);
                }
                if (GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString())) //x-1.z
                {
                    if (GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString()).transform.Find("HexWall(Clone)/WallSouthEast").gameObject.activeSelf == true && GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString()).transform.Find("HexWall(Clone)/WallSouthEast").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString()).GetComponent<Hex>().wallHeight);
                    }
                    if (GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString()).transform.Find("HexWall(Clone)/WallNorthEast").gameObject.activeSelf == true && GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString()).transform.Find("HexWall(Clone)/WallNorthEast").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + hexZ.ToString()).GetComponent<Hex>().wallHeight);
                    }
                }
                if (GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString())) //x.z+1
                {
                    if (GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString()).transform.Find("HexWall(Clone)/WallSouthWest").gameObject.activeSelf == true && GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString()).transform.Find("HexWall(Clone)/WallSouthWest").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                    if (GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString()).transform.Find("HexWall(Clone)/WallSouth").gameObject.activeSelf == true && GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString()).transform.Find("HexWall(Clone)/WallSouth").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ + 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                }
            }
            else if (column.name == "ColumnNNE")
            {
                if (transform.Find("HexWall(Clone)/WallNorth").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallNorth").transform.localPosition.y != -2) //x.z
                {
                    wallHeights.Add(wallHeight);
                }
                if (transform.Find("HexWall(Clone)/WallNorthEast").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallNorthEast").transform.localPosition.y != -2)
                {
                    wallHeights.Add(wallHeight);
                }
                if (GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + (hexZ + 1).ToString()))
                {
                    if (GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + (hexZ + 1).ToString()).transform.Find("HexWall(Clone)/WallNorthWest").gameObject.activeSelf == true && GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + (hexZ + 1).ToString()).transform.Find("HexWall(Clone)/WallNorthWest").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + (hexZ + 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                }
            }
            else if (column.name == "ColumnE")
            {
                if (transform.Find("HexWall(Clone)/WallNorthEast").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallNorthEast").transform.localPosition.y != -2) //x.z
                {
                    wallHeights.Add(wallHeight);
                }
                if (transform.Find("HexWall(Clone)/WallSouthEast").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallSouthEast").transform.localPosition.y != -2)
                {
                    wallHeights.Add(wallHeight);
                }
            }
            else if (column.name == "ColumnSSE")
            {
                if (transform.Find("HexWall(Clone)/WallSouth").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallSouth").transform.localPosition.y != -2) //x.z
                {
                    wallHeights.Add(wallHeight);
                    Debug.Log(column.transform.parent.transform.parent.name + gameObject.name + " colSSE-wallS " + wallHeight);
                }
                if (transform.Find("HexWall(Clone)/WallSouthEast").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallSouthEast").transform.localPosition.y != -2)
                {
                    wallHeights.Add(wallHeight);
                    Debug.Log(column.transform.parent.transform.parent.name + gameObject.name + " colSSE-wallSE " + wallHeight);
                }
                if (GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString())) //x.z-1
                {
                    if (GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallNorthEast").gameObject.activeSelf == true && GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallNorthEast").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).GetComponent<Hex>().wallHeight);
                        Debug.Log(column.transform.parent.transform.parent.name + " Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString() + " colSSE-wallNE " + GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                    if (GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallNorth").gameObject.activeSelf == true && GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallNorth").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).GetComponent<Hex>().wallHeight);
                        Debug.Log(column.transform.parent.transform.parent.name + " Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString() + " colSSE-wallN " + GameObject.Find("Hex-" + hexX.ToString() + "-" + (hexZ - 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                }
                if (GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + hexZ.ToString())) //x+1.z
                {
                    if (GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + hexZ.ToString()).transform.Find("HexWall(Clone)/WallSouthWest").gameObject.activeSelf == true && GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + hexZ.ToString()).transform.Find("HexWall(Clone)/WallSouthWest").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + hexZ.ToString()).GetComponent<Hex>().wallHeight);
                        Debug.Log(column.transform.parent.transform.parent.name + " Hex-" + (hexX + 1).ToString() + "-" + hexZ.ToString() + " colSSE-wallSW " + GameObject.Find("Hex-" + (hexX + 1).ToString() + "-" + hexZ.ToString()).GetComponent<Hex>().wallHeight);
                    }
                }
            }
            else if (column.name == "ColumnSSW")
            {
                if (transform.Find("HexWall(Clone)/WallSouth").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallSouth").transform.localPosition.y != -2) //x.z
                {
                    wallHeights.Add(wallHeight);
                }
                if (transform.Find("HexWall(Clone)/WallSouthWest").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallSouthWest").transform.localPosition.y != -2)
                {
                    wallHeights.Add(wallHeight);
                }
                if (GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString())) //x-1.z-1
                {
                    if (GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallSouthEast").gameObject.activeSelf == true && GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallSouthEast").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                    if (GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallNorthEast").gameObject.activeSelf == true && GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString()).transform.Find("HexWall(Clone)/WallNorthEast").transform.localPosition.y != -2)
                    {
                        wallHeights.Add(GameObject.Find("Hex-" + (hexX - 1).ToString() + "-" + (hexZ - 1).ToString()).GetComponent<Hex>().wallHeight);
                    }
                }
            }
            else if (column.name == "ColumnW")
            {
                if (transform.Find("HexWall(Clone)/WallNorthWest").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallNorthWest").transform.localPosition.y != -2) //x.z
                {
                    wallHeights.Add(wallHeight);
                }
                if (transform.Find("HexWall(Clone)/WallSouthWest").gameObject.activeSelf == true && transform.Find("HexWall(Clone)/WallSouthWest").transform.localPosition.y != -2)
                {
                    wallHeights.Add(wallHeight);
                }
            }
            
            if (wallHeights == null)
            {
                wallHeights.Add(wallHeight);
            }

            for (int i = 0; i < wallHeights.Count; i++)
            {       
                if (wallHeights[i] > colHeight)
                {
                    colHeight = wallHeights[i];
                }

                if (column.name == "ColumnSSE")
                {
                    Debug.Log(column.transform.parent.transform.parent.name + "wallHeight: " + wallHeights[i]);
                }
            }

            if (column.name == "ColumnSSE")
            {
                Debug.Log(column.transform.parent.transform.parent.name + "colHeight: " + colHeight);
            }

            column.GetComponent<Transform>().localScale = new Vector3(column.GetComponent<Transform>().localScale.x, colHeight + (0.005f * hexScale), column.GetComponent<Transform>().localScale.z);
        }
    }
}
