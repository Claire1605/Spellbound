using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWallChecker : MonoBehaviour {

    public bool wallCollision = false;
    private GameObject otherWall;

    //private void Start()
    //{
    //    GetComponent<BoxCollider>().isTrigger = true;
    //}

    public void removeDuplicateWall()
    {
        //RaycastHit hit;
        //Debug.Log(origin.x + "," + origin.y + "," + origin.z);
        //if (Physics.SphereCast(origin, 0.1f, transform.forward, out hit, 1))
        //{
        //   if (hit.collider.gameObject.tag== "HexWallChecker")
        //    {
        //        hit.collider.gameObject.SetActive(false);
        //    }
        //}

        if (otherWall && wallCollision)
        {
            otherWall.SetActive(false);
            otherWall = null;
            wallCollision = false;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        //Debug.Log("this: " + gameObject.transform.parent.transform.parent.transform.parent.gameObject.name + "." + gameObject.name + " & collider: " + col.gameObject.transform.parent.transform.parent.transform.parent.gameObject.name + "." + col.gameObject.name);
        //if (col.gameObject.tag == "HexWallChecker" && gameObject.tag == "HexWallChecker")
        //{
        //    Debug.Log("this: " + gameObject.transform.parent.transform.parent.transform.parent.gameObject.name + "." + gameObject.name + " & collider: " + col.gameObject.transform.parent.transform.parent.transform.parent.gameObject.name + "." + col.gameObject.name);
        //    wallCollision = true;
        //    otherWall = col.transform.parent.gameObject;
        //}
    }

    //private void OnTriggerExit(Collider col)
    //{
    //    if (col.gameObject.tag == "HexWallChecker")
    //    {
    //        wallCollision = false;
    //    }
    //}
}
