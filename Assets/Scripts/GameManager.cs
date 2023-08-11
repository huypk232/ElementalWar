using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private static GameObject _firstChar;
    private static GameObject _secondChar;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void OnFirstPlayerSelect(GameObject gameObject) {
        _firstChar = gameObject;
    }

    public void OnSecondPlayerSelect(GameObject gameObject) {
        _secondChar = gameObject;
    }
}
