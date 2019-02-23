using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour {

    [Header("Hex")]
    public GameObject hexParent;
    public GameObject hexPrefab;
    public GameObject hexWallPrefab;
    public int wallHeightMin = 0;
    public int wallHeightMax = 6;
    public Hex[,] hexArray;
    public int hexArraySize = 10;
    public float hexScale = 1.0f;

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
            foreach (var item in GameObject.FindGameObjectsWithTag("HexWallChecker"))
            {
                item.GetComponent<HexWallChecker>().removeDuplicateWall();
            }
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
                    Vector3 scale = new Vector3(w.transform.localScale.x * hexScale, Random.Range(wallHeightMin, wallHeightMax), w.transform.localScale.z * hexScale);
                    w.transform.localScale = scale;
                    w.transform.parent = h.transform;
                    w.transform.localPosition = Vector3.zero;

                    //DEACTIVATE DUPLICATE WALLS
                    List<int> edges = new List<int>();

                    if (x == 0 && z == 0) // origin hex
                    {
                        edges.Clear();
                    }
                    else if (x == 0) // W edge
                    {
                        edges.Clear();
                        edges.Add(3);
                    }
                    else if (z == 0) // SSW edge
                    {
                        edges.Clear();
                        edges.Add(5);
                    }
                    else if (x < hexArraySize / 2 && z == limit - 1) // NNW edge
                    {
                        edges.Clear();
                        edges.Add(3);
                        edges.Add(4);
                    }
                    else if (x >= hexArraySize/2 && z == hexArraySize - limit) // SSE edge
                    {
                        edges.Clear();
                        edges.Add(4);
                        edges.Add(5);
                    }
                    else // all others
                    {
                        edges.Clear();
                        edges.Add(3);
                        edges.Add(4);
                        edges.Add(5);
                    }
                    
                    h.GetComponent<Hex>().DeactivateDuplicateWalls(edges);

                    //RANDOMISE WALL LOWERING
                    // to do - keep outer walls raised
                    h.GetComponent<Hex>().RandomiseWalls();
                }
            }
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
