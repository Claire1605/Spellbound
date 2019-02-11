using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetectAndInteract : MonoBehaviour {

    public float detectionDistance;
    public float nameDisplayDistance;
    public float detectionAngle;
    public Transform CameraTransform;
   // public PauseMenu pauseMenu;
    public PlayerMovement fpc;
    public Text displayName;
    public Image cursorImage;
    public Sprite cursorDefault;
    public Sprite cursorHover;
    public Sprite cursorClick;
    public Witch witch;

    private Collider[] Hits;
    private float objDistance;
    private InteractableObject intObj;
    private InteractableObject actualObject;
    private InteractableObject tempObject;
    private RaycastHit rayHit;

    void Update() {    
        displayName.text = "";
        cursorImage.sprite = cursorDefault;
        actualObject = null;

        if (tempObject != actualObject && tempObject != null)
            TurnOffEmission();

        if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out rayHit, detectionDistance))
        {
           Debug.Log(rayHit.collider.gameObject.name.ToString());
            intObj = rayHit.transform.GetComponent<InteractableObject>();
            if (intObj != null)
                actualObject = intObj;
        }
        if (actualObject == null)
        {
            Hits = Physics.OverlapSphere(CameraTransform.position, detectionDistance); //detectionDistance
            objDistance = float.MaxValue;

            foreach (Collider hit in Hits)
            {
                intObj = hit.transform.GetComponent<InteractableObject>();
                if (intObj != null)
                {
                    if (Vector3.Dot(CameraTransform.TransformDirection(Vector3.Normalize(Vector3.forward)), Vector3.Normalize(intObj.transform.position - CameraTransform.position)) > detectionAngle)
                    {
                        if (Physics.Linecast(CameraTransform.position, intObj.transform.position, 2))
                        {
                            if ((intObj.transform.position - CameraTransform.position).magnitude < objDistance)
                            {
                                objDistance = (intObj.transform.position - CameraTransform.position).magnitude;
                                actualObject = intObj;
                            }
                        }
                    }
                }
            }
        }

        if (!fpc.paused)
        {
            if (actualObject != null && actualObject.canInteractWith)
            {
                tempObject = actualObject;
                float baseEmit = tempObject.baseEmitStrength;
                foreach (var item in tempObject.GetComponentsInChildren<Renderer>())
                {
                    if (item.material.HasProperty("_EmitStrength"))
                    {
                        if (item.material.GetFloat("_EmitStrength") == baseEmit)
                            item.material.SetFloat("_EmitStrength", 0.25f);
                    }
                }
                cursorImage.sprite = cursorHover;
                if (Vector3.Distance(fpc.transform.position, tempObject.transform.position) < actualObject.individualDetectionDistance)
                {
                    if (PlayerSpellbook.namesFound.Contains(actualObject.trueName))
                    {
                        displayName.text = actualObject.trueName;
                    }
                    else
                    {
                        displayName.text = "?";
                    }
                    cursorImage.sprite = cursorClick;
                    if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        witch.Identify(actualObject.trueName, actualObject.gameObject);
                        actualObject.Interact();
                    }
                } 
            }
        }
    }

    void TurnOffEmission()
    {
        float baseEmit = tempObject.baseEmitStrength;
        foreach (var item in tempObject.GetComponentsInChildren<Renderer>())
        {
            if (item.material.HasProperty("_EmitStrength"))
            {
                if (item.material.GetFloat("_EmitStrength") == 0.25f)
                    item.material.SetFloat("_EmitStrength", baseEmit);
            }
        }
        tempObject = null;
    }
}
