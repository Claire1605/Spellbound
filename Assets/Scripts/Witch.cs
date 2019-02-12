using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Witch : MonoBehaviour {

    public PlayerMovement movement;

    [Header("Identify")]
    public bool identifyEnabled = true;
    public int IDNumber = 0;
    public string IDName = "";
    public List<GameObject> IDObjects = new List<GameObject>();
    public Animator IDAnim;
    private bool lerpSizeInProgress = false;

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

    public void setPositionToOrigin(Vector3 origin)
    {
        transform.position = origin;
    }

    public void Identify(string name, GameObject obj)
    {
        IDAnim.SetBool("success", false);
        IDAnim.SetBool("failure", false);

        //OBJECT ALREADY IN LIST
        if (IDObjects.Contains(obj) && !lerpSizeInProgress)
        {
            StartCoroutine(lerpSize(identifySymbols[IDObjects.IndexOf(obj)].transform));
        }
        else if (!IDObjects.Contains(obj) && identifyEnabled && !PlayerSpellbook.namesFound.Contains(name) && !lerpSizeInProgress)
        {
            IDObjects.Add(obj);

            //CORRECT IDENTIFICATION, BEGIN/CONTINUE SPELL
            if (IDName == "" || name == IDName) 
            {
                IDNumber += 1;
                foreach (var symbol in identifySymbols)
                {
                    identifySymbols[IDNumber - 1].SetActive(true);
                }
                IDName = name;
            }
            //INCORRECT IDENTIFICATION, END SPELL
            else 
            {
                IDAnim.SetBool("failure", true);
                identifyEnabled = false;
                StartCoroutine(identifyAnim("identifyFailure"));
                IDNumber = 0;
                IDName = "";
                //foreach (var symbol in identifySymbols)
                //{
                //    symbol.SetActive(false);
                //}
                IDObjects.Clear();
            }

            //ADD TO SPELLBOOK WHEN FOUND 3
            if (IDNumber == 3) 
            {
                IDAnim.SetBool("success", true);
                identifyEnabled = false;
                StartCoroutine(identifyAnim("identifySuccess"));
                IDNumber = 0;
                IDObjects.Clear();
                PlayerSpellbook.AddName(name);
            }
        }
        //INCORRECT IDENTIFICATION, END SPELL
        else if (!lerpSizeInProgress)
        {
            IDAnim.SetBool("failure", true);
            identifyEnabled = false;
            StartCoroutine(identifyAnim("identifyFailure"));
            IDNumber = 0;
            IDName = "";
            //foreach (var symbol in identifySymbols)
            //{
            //    symbol.SetActive(false);
            //}
            IDObjects.Clear();
        }
    }

    private IEnumerator identifyAnim(string animName)
    {
        //WAIT FOR SUCCESS ANIM TO BEGIN
        while (!IDAnim.GetCurrentAnimatorStateInfo(0).IsName(animName)) 
        {
            yield return new WaitForEndOfFrame();
        }
        IDAnim.SetBool("success", false);
        IDAnim.SetBool("failure", false);
        //WAIT FOR IT TO END
        while (IDAnim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            yield return new WaitForEndOfFrame();
        }
        
        foreach (var symbol in identifySymbols)
        {
            symbol.SetActive(false);
        }

        //COOLDOWN
        while (IDAnim.GetCurrentAnimatorStateInfo(0).IsName("identifyCooldown"))
        {
            yield return new WaitForEndOfFrame();
        }
        identifyEnabled = true;
    }

    private IEnumerator lerpSize(Transform t)
    {
        lerpSizeInProgress = true;
       // identifyEnabled = false;
        float i = 0;
        Vector3 initialScale = t.localScale;
        while (i < 1)
        {
            i += Time.deltaTime;
            t.localScale = initialScale * (1 + Mathf.Sin(i * Mathf.PI));
            yield return new WaitForEndOfFrame();
        }
        t.localScale = initialScale;
        lerpSizeInProgress = false;
       // identifyEnabled = true;
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
