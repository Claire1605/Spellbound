using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Witch : MonoBehaviour {

    public int IDNumber = 0;
    public string IDName = "";
    public PlayerMovement movement;

    [Header("Spellbook")]
    public GameObject spellbook;
    public Text spellList;
    public List<GameObject> identifySymbols = new List<GameObject>();

    void Start () {
        movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!spellbook.activeSelf)
            {
                OpenSpellbook();
            }
            else if (spellbook.activeSelf)
            {
                CloseSpellbook();
            }
        }
    }

    public void Identify(string name)
    {
        if (IDName == "" || name == IDName) //if null or the same, add number
        {
            IDNumber += 1;
            foreach (var symbol in identifySymbols)
            {
                identifySymbols[IDNumber - 1].SetActive(true);
            }
            IDName = name;
        }
        else //if different, reset number
        {
            IDNumber = 0;
            IDName = "";
            foreach (var symbol in identifySymbols)
            {
                symbol.SetActive(false);
            }
        }

        if (IDNumber == 3)
        {
            PlayerSpellbook.AddName(name);
            IDNumber = 0;
            //foreach (var symbol in identifySymbols)
            //{
            //    symbol.SetActive(false);
            //}
        }
    }

    public void OpenSpellbook()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        movement.paused = true;
        spellbook.SetActive(true);
        spellList.text = "";
        foreach (var name in PlayerSpellbook.namesFound)
        {
            spellList.text += name + "\r\n";
        }

    }

    public void CloseSpellbook()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        movement.paused = false;
        spellbook.SetActive(false);
    }
}
