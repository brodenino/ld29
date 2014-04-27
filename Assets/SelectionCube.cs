using UnityEngine;
using System.Collections;

public class SelectionCube : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        if (!transform.parent.renderer.enabled && renderer.enabled)
        {
            renderer.enabled = false;
        }
    }
}
