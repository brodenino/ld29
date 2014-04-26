using UnityEngine;
using System.Collections;

public class ColorSetter : MonoBehaviour {
    public Color[] colors;

    // Use this for initialization
    void Start () {
        if (colors.Length > 0)
            renderer.material.color = colors[Random.Range(0, colors.Length)];
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
