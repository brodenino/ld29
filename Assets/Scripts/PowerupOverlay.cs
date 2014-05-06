using UnityEngine;
using System.Collections;

public class PowerupOverlay : MonoBehaviour {
    public float floatSpeed = 90;
    public float floatAmplitude = 0.1f;
    // Use this for initialization
    float angle = 0;

    bool activated = false;

    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
        if (renderer.material.mainTexture && !activated)
        {
            activated = true;
            transform.parent.localScale = Vector3.one * 0.8f;
            transform.parent.GetComponentInChildren<ShapeOverlay>().transform.localScale = Vector3.one * 0.9f;
        }
        if (activated)
        {
            //angle += floatSpeed * Time.deltaTime * Mathf.Deg2Rad;
            transform.parent.Rotate(Vector3.up, floatSpeed * Time.deltaTime);
            //transform.parent.localPosition += Vector3.up * Mathf.Sin(angle) * floatAmplitude;
        }   
    }
}
