using UnityEngine;
using System.Collections;

public class SelectionCube : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        transform.eulerAngles = Vector3.zero;
        if (!transform.parent.renderer.enabled && renderer.enabled)
        {
            renderer.enabled = false;
        }
    }
}
