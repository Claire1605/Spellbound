using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableObject : MonoBehaviour {

    [Header("General: Interactable Object")]
   // public string displayName = "?";
    public string trueName;
    public bool canInteractWith = true;
    public float individualDetectionDistance = 5;
    public float baseEmitStrength = 0f;

    private bool trueNameFound = false;

    public void Awake()
    {
        foreach (var item in GetComponentsInChildren<Renderer>())
        {
            item.material.SetFloat("_EmitStrength", baseEmitStrength);
        }
    }

    public virtual void Interact(){
    }
 
}
