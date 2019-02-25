using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour {

    [Header("Hex")]
    public GameObject hexParent;
    public GameObject hexPrefab;
    public GameObject hexWallPrefab;
    public GameObject hexColumnPrefab;
    public int wallHeightMin = 0;
    public int wallHeightMax = 6;
    public Hex[,] hexArray;
    public int hexArraySize = 10;
    public int hexScale = 1;

    private float hexVertRadius;
    private float hexEdgeRadius;
    private int limit = 0;

    [Header("Biome")]
    public int biomeNumber = 6;
    public List<Biome> biomeList = new List<Biome>();
    public Shader biomeShader;
    public Texture2D biomeShaderTex;
    [Range(0.0f,1.0f)]
    public float hueMin = 0.0f;
    [Range(0.0f, 1.0f)]
    public float hueMax = 1.0f;
    [Range(0.0f, 1.0f)]
    public float satMin = 0.0f;
    [Range(0.0f, 1.0f)]
    public float satMax = 1.0f;
    [Range(0.0f, 1.0f)]
    public float valMin = 0.0f;
    [Range(0.0f, 1.0f)]
    public float valMax = 1.0f;

    private Witch witch;
    private Vector3 origin = new Vector3(0, 0, 0);
    private bool pathfindingDone = false;

    private void Awake()
    {
        //IF ANY HEX's FROM EDIT MODE
        foreach (var item in hexParent.GetComponentsInChildren<Hex>())
        {
            DestroyImmediate(item.gameObject);
        }
    }
    void Start () {
        witch = GameObject.FindGameObjectWithTag("Player").GetComponent<Witch>();
        GenerateLabyrinth(false);
        witch.setPositionToOrigin(origin);
    }

    private void Update()
    {
        if (!pathfindingDone)
        {
            pathfindingDone = true;
        }
    }

    public void GenerateLabyrinth(bool editMode)
    {
        //IF ANY HEX's FROM EDIT MODE
        foreach (var item in hexParent.GetComponentsInChildren<Hex>())
        {
            Destroy(item.gameObject);
        }

        //INSTANTIATE BIOMES
        biomeList.Clear();
        for (int i = 0; i < biomeNumber; i++)
        {
            biomeList.Add(new Biome());
            biomeList[i].Initialise(hueMin, hueMax, satMin, satMax, valMin, valMax);
            biomeList[i].mat = new Material(biomeShader);
            biomeList[i].mat.SetColor("_Colour1", biomeList[i].colour1);
            biomeList[i].mat.SetColor("_Colour2", biomeList[i].colour2);
            biomeList[i].mat.SetColor("_Colour3", biomeList[i].colour3);
            biomeList[i].mat.SetTexture("_MainTex", biomeShaderTex);
        }

        //HEX
        if (hexArraySize % 2 != 0) //Make an even number
        {
            hexArraySize += 1;
        }

        hexArray = new Hex[hexArraySize, hexArraySize];
        hexVertRadius = 1.0f * hexScale;
        hexEdgeRadius = (Mathf.Cos(30 * Mathf.PI / 180)) * hexVertRadius;
        limit = (hexArraySize / 2) - 1;

        for (int x = 0; x < hexArraySize - 1; x++)
        {
            if (x < hexArraySize / 2)
            {
                limit += 1;
            }
            else if (x > hexArraySize / 2)
            {
                limit -= 1;
            }

            for (int z = 0; z < hexArraySize - 1; z++)
            {
                hexArray[x, z] = new Hex();
                hexArray[x, z].x = (hexEdgeRadius * 2 * z) - (hexEdgeRadius * x);
                hexArray[x, z].z = -((hexVertRadius + (hexVertRadius / 2)) * x);

                //INSTANTIATE GAME HEX-TILES
                if (x < hexArraySize / 2 && z < limit || x >= hexArraySize / 2 && z >= hexArraySize - limit)
                {
                    GameObject h = Instantiate(hexPrefab);
                    h.GetComponent<Hex>().Initialise(editMode, hexParent, hexArray[x, z].x, hexArray[x, z].z, x, z, hexScale, biomeList[Random.Range(0, biomeList.Count)]);
                    GameObject w = Instantiate(hexWallPrefab);
                    GameObject c = Instantiate(hexColumnPrefab);
                    h.GetComponent<Hex>().wallHeight = Random.Range(wallHeightMin, wallHeightMax);
                    Vector3 scale = new Vector3(w.transform.localScale.x * hexScale, h.GetComponent<Hex>().wallHeight, w.transform.localScale.z * hexScale);
                    w.transform.localScale = scale;
                    w.transform.parent = h.transform;
                    w.transform.localPosition = Vector3.zero;
                    foreach (var column in c.GetComponentsInChildren<Transform>())
                    {
                        if (column.gameObject.tag == "HexColumn")
                        {
                            column.transform.localScale = scale;
                        }
                    }
                    c.transform.parent = h.transform;
                    c.transform.localPosition = Vector3.zero;

                    //DEACTIVATE DUPLICATE WALLS
                    List<int> edgesToDeactivate = new List<int>();
                    List<int> columnsToDeactivate = new List<int>();
                    List<int> edgesToKeepRaised = new List<int>();

                    edgesToDeactivate.Clear();
                    columnsToDeactivate.Clear();
                    edgesToKeepRaised.Clear();

                    if (x == 0 && z == 0) // 0-0, West corner
                    {
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(2);
                        edgesToKeepRaised.Add(3);
                        edgesToKeepRaised.Add(4);
                        edgesToKeepRaised.Add(5);

                    }
                    else if (x == 0 && z == limit - 1) // e.g 0-2, NNW corner
                    {
                        edgesToDeactivate.Add(3);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(4);
                        edgesToKeepRaised.Add(0);
                        edgesToKeepRaised.Add(4);
                        edgesToKeepRaised.Add(5);
                    }
                    else if (x == (hexArraySize / 2) - 1 && z == limit - 1) // e.g 2-4, NNE corner
                    {
                        edgesToDeactivate.Add(3);
                        edgesToDeactivate.Add(4);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(4);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(0);
                        edgesToKeepRaised.Add(1);
                        edgesToKeepRaised.Add(5);
                    }
                    else if (x == hexArraySize - 2 && z == x) // e.g 4-4, E corner
                    {
                        edgesToDeactivate.Add(3);
                        edgesToDeactivate.Add(4);
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(4);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(0);
                        edgesToKeepRaised.Add(1);
                        edgesToKeepRaised.Add(2);
                    }
                    else if (x == hexArraySize - 2 && z == hexArraySize - limit) // e.g 4-2, SSE corner
                    {
                        edgesToDeactivate.Add(0);
                        edgesToDeactivate.Add(4);
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(1);
                        edgesToKeepRaised.Add(2);
                        edgesToKeepRaised.Add(3);
                    }
                    else if (x == (hexArraySize / 2) - 1 && z == 0) // e.g 2-0, SSW corner
                    {
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(2);
                        edgesToKeepRaised.Add(3);
                        edgesToKeepRaised.Add(4);
                    }
                    else if (z == 0) // SSW edge
                    {
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(3);
                        edgesToKeepRaised.Add(4);
                    }
                    else if (x == 0) // W edge
                    {
                        edgesToDeactivate.Add(3);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(4);
                        edgesToKeepRaised.Add(4);
                        edgesToKeepRaised.Add(5);
                    }
                    else if (x < hexArraySize / 2 && z == limit - 1) // NNW edge
                    {
                        edgesToDeactivate.Add(3);
                        edgesToDeactivate.Add(4);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(4);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(0);
                        edgesToKeepRaised.Add(5);
                    }
                    else if (x >= (hexArraySize / 2) - 1 && z == hexArraySize - 2) // NNE edge
                    {
                        edgesToDeactivate.Add(3);
                        edgesToDeactivate.Add(4);
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(4);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(0);
                        edgesToKeepRaised.Add(1);
                    }
                    else if (x >= hexArraySize - 2 && z >= hexArraySize / 2 - 1) // E edge
                    {
                        edgesToDeactivate.Add(4);
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(4);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(1);
                        edgesToKeepRaised.Add(2);
                    }
                    else if (x >= hexArraySize/2 && z == hexArraySize - limit) // SSE edge
                    {
                        edgesToDeactivate.Add(4);
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(5);
                        edgesToKeepRaised.Add(2);
                        edgesToKeepRaised.Add(3);
                    }
                    else // all others
                    {
                        edgesToDeactivate.Add(3);
                        edgesToDeactivate.Add(4);
                        edgesToDeactivate.Add(5);
                        columnsToDeactivate.Add(1);
                        columnsToDeactivate.Add(2);
                        columnsToDeactivate.Add(4);
                        columnsToDeactivate.Add(5);
                    }

                    //RANDOMISE WALL LOWERING AND SET OUTER WALLS TO RAISED
                    h.GetComponent<Hex>().RandomiseWalls(edgesToKeepRaised);
                    h.GetComponent<Hex>().DeactivateDuplicateWalls(edgesToDeactivate, columnsToDeactivate);
                    //h.GetComponent<Hex>().SetColumnHeights(x, z, hexScale);
                }
            }
        }

        foreach (var hex in GameObject.Find("HexParent").gameObject.GetComponentsInChildren<Hex>())
        {
            hex.SetColumnHeights(hex.hex_X, hex.hex_Z, hexScale);
        }
        origin = new Vector3(hexArray[hexArraySize / 2 - 1, hexArraySize / 2 - 1].x, 0, hexArray[hexArraySize / 2 - 1, hexArraySize / 2 - 1].z);
    }

    public void GenerateLabyrinthButton()
    {
        //IF ANY HEX's FROM EDIT MODE
        foreach (var item in hexParent.GetComponentsInChildren<Hex>())
        {
            DestroyImmediate(item.gameObject);
        }
        GenerateLabyrinth(true);
    }
}
