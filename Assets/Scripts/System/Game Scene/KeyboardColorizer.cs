using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardColorizer : MonoBehaviour
{
    private Dictionary<char, KeyboardKey> keyboardKeyDict = new Dictionary<char, KeyboardKey>();

    private void Awake()
    {
        KeyboardKey[] keys = FindObjectsOfType<KeyboardKey>(true);

        foreach (var key in keys)
        {
            if (!keyboardKeyDict.ContainsKey(key.letterText.text[0]))
            {
                keyboardKeyDict.Add(key.letterText.text[0], key);
            }
        }
    }

    public void UpdateKeyColor(char letter, LetterState letterState, ColorState colorState)
    {
        if (keyboardKeyDict.TryGetValue(letter, out KeyboardKey key))
        {
            key.SetLetterState(letterState, colorState);
        }
    }
}

