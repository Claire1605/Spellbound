using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureGenerator : MonoBehaviour {

    public int layerCount = 3;
    public GameObject[] shapePrefabs = new GameObject[6];

    //transformations
    float[] layerTranslation; //placeholder transformation test
    float[] layerRotation; //later - turn these into vectors
    float[] layerScale;

    //shapes
    GameObject[] shapes;
    float[] shapeMag; //size of the shapes in each layer
    float[] halfShapeMag ; //size/2 of the shapes in each layer
    //float[] shapeRadii; //pivot point radii
    List<Matrix4x4> matrixStack = new List<Matrix4x4>();

    //colours
    Color[] colours;

    int[] shapeCountMag;

    void Start () {
        generate();
    }

    public void generate()
    {
        layerTranslation = new float[layerCount];
        layerRotation = new float[layerCount];
        layerScale = new float[layerCount];
        shapes = new GameObject[layerCount];
        shapeMag = new float[layerCount];
        halfShapeMag = new float[layerCount];
        //shapeRadii = new float[layerCount];
        colours = new Color[layerCount];
        shapeCountMag = new int[layerCount];
        matrixStack = new List<Matrix4x4>();
        matrixStack.Add(Matrix4x4.identity);

        setupShapeVariables();

        generateLayers(0);
    }

    public void setupShapeVariables()
    {
        float maxScale = 200; //later - make this a variable

        //SET COLOURS
        colours[0] = Random.ColorHSV(); //later - change these to have a range
        colours[1] = Random.ColorHSV();
        colours[2] = Random.ColorHSV();

        for (int i = 0; i < layerCount; i++)
        {
            //DEFINE TRANSFORMATIONS
            layerTranslation[i] = Random.Range(0, 6) * 0.2f; //later - make these variables
            layerRotation[i] = Random.Range(0, 360);
            layerScale[i] = Random.Range(0.75f, 1.0f); //later - make these variables

            //DEFINE SHAPE SIZE MAGNTIUDES
            shapeMag[i] = Random.Range(maxScale / 4, maxScale);
            halfShapeMag[i] = shapeMag[i] / 2;
            maxScale = shapeMag[i];

            shapes[i] = shapePrefabs[Random.Range(0, shapePrefabs.Length)];

            //SET SHAPE COUNT MAGNITUDES
            shapeCountMag[i] = Random.Range(1, 7); //later - make these variables
        }
    }

    public void generateLayers(int layerNumber)
    {
        for (int shape = 0; shape < shapeCountMag[layerNumber]; shape++)
        {
            GameObject newShape = Instantiate(shapes[layerNumber]);
            Matrix4x4 currentMatrix = matrixStack[matrixStack.Count - 1];
            Matrix4x4 matrixRotation = Matrix4x4.Rotate(Quaternion.Euler(0,layerRotation[layerNumber],0));
            currentMatrix = matrixRotation * currentMatrix;
            matrixStack.Add(currentMatrix);

            Matrix4x4 matrixScale = Matrix4x4.Scale(new Vector3(layerScale[layerNumber], layerScale[layerNumber], layerScale[layerNumber]));
            Matrix4x4 matrixTranslation = Matrix4x4.Translate(new Vector3(layerTranslation[layerNumber], 0,0));

            currentMatrix = matrixScale * matrixTranslation * currentMatrix;
            matrixStack.Add(currentMatrix);

            newShape.transform.position = currentMatrix.MultiplyPoint(Vector3.zero);
            newShape.transform.rotation = currentMatrix.rotation;
            newShape.transform.localScale = currentMatrix.lossyScale;

            if (layerNumber + 1 < layerCount)
            {
                generateLayers(layerNumber + 1);
            }

            //pop matrix
            matrixStack.RemoveAt(matrixStack.Count - 1);
            //later - parent shapes properly
        }
    }
}
