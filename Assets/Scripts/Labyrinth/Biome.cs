using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome {

    public Color colour1;
    public Color colour2;
    public Color colour3;
    public float deformHeight;
    public float deformZoom;
    public List<string> life = new List<string>();
    public Material mat;

	public void Initialise(float hMin, float hMax, float sMin, float sMax, float vMin, float vMax, float minDeformHeight, float maxDeformHeight, float minDeformZoom, float maxDeformZoom)
    {
        colour1 = Random.ColorHSV(hMin, hMax, sMin, sMax, vMin, vMax);
        colour2 = Random.ColorHSV(hMin, hMax, sMin, sMax, vMin, vMax);
        colour3 = Random.ColorHSV(hMin, hMax, sMin, sMax, vMin, vMax);
        deformHeight = Random.Range(minDeformHeight, maxDeformHeight);
        deformZoom = Random.Range(minDeformZoom, maxDeformZoom);
    }
}
