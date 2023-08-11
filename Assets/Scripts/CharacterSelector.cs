using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    public int character;


    public void OnFirstPlayerSelect() {
        Debug.Log(character);
    }
}
