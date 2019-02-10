using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerSpellbook {

    public static List<string> namesFound = new List<string>();

    public static void AddName(string name)
    {
        namesFound.Add(name);
    }
}
