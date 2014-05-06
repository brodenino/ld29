using UnityEngine;
using System.Collections;
using System;

public class LayerColors : MonoBehaviour {
    public ColorPalette[] palettes;
    public Texture2D[] overlays;

    [Serializable]
    public class ColorPalette
    {
        public Color baseColor;
        public Color[] colors;
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
