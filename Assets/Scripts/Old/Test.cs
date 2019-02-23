using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public List<GameObject> testList = new List<GameObject>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        testList.Clear();
        foreach (var wall in GetComponentsInChildren<Transform>())
        {
            if (wall.gameObject.tag == "TEST")
            {
                testList.Add(wall.gameObject);
            }
        }

        Debug.Log("children:" + testList.Count);
        Debug.Log("child 1:" + testList[0].gameObject.name);
    }
}
