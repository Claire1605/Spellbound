using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameSetup : MonoBehaviour {

    public GameObject introPanel;
    public FirstPersonController fpc;

    void Start()
    {
        introPanel.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fpc.paused = true;
    }

    public void BeginButton()
    {
        foreach (var rend in introPanel.gameObject.GetComponentsInChildren<Image>())
        {
            StartCoroutine(fadeColour(rend, rend.color));
        }
    }

    IEnumerator fadeColour(Image img, Color colour)
    {
        float i = 0;
        Color c = colour;
        while (i <1)
        {
            i += Time.deltaTime * 0.25f;
            c.a = Mathf.Lerp(1, 0, i);
            img.color = c;
            yield return new WaitForEndOfFrame();
        }
        introPanel.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fpc.paused = false;
    }
}
