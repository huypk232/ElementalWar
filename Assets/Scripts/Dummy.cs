using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public SpriteRenderer Renderer;

    void Update()
    {
        
    }

    public void TakeDamamge(int damage)
    {
        Renderer.color = new Color(255, 255, 255);
        StartCoroutine(NormalizeColor());
    }

    private IEnumerator NormalizeColor()
    {
        yield return new WaitForSeconds(0.5f);
        Renderer.color = new Color(0, 0, 0);
    }
}
