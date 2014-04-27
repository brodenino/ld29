using UnityEngine;
using System.Collections.Generic;

public class Speciality : MonoBehaviour {
    public bool isAvailable = false;

    public AudioClip explodeSound;
    public Texture2D texture;

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    public bool Trigger()
    {
        if (!isAvailable)
            return false;

        var explodeObj = this;
        var gridInstance = transform.parent.GetComponent<GridInstance>();
        var randomPickComponent = explodeObj.GetComponent<Picker>();
        // Find all objects nearby, and have them explode.
        for (int x = -1; x <= 1; x += 1)
        {
            for (int z = -1; z <= 1; z += 1)
            {
                // constrain
                int nx = Mathf.Clamp((int)(randomPickComponent.index.x) + x, 0, gridInstance.xSize - 1);
                int nz = Mathf.Clamp((int)(randomPickComponent.index.z) + z, 0, gridInstance.zSize - 1);

                var neighbour = gridInstance.GetGridObject(nx, (int)randomPickComponent.index.y, nz);

                if (neighbour && neighbour != explodeObj)
                {
                    var fadeOutDuration = neighbour.GetComponent<Picker>().fadeOutDuration;

                    neighbour.GetComponent<Picker>().fadeOutCurrent = fadeOutDuration;
                    neighbour.collider.enabled = false;

                    var neighbourPick = neighbour.GetComponent<Picker>();
                    //var objectBeneath = gridInstance.grid[(int)(neighbourPick.index.x), (int)Mathf.Max(0, neighbourPick.index.y - 1), (int)(neighbourPick.index.z)];
                    //var objectBeneath = gridInstance.GetGridObject((int)(neighbourPick.index.x), (int)Mathf.Max(0, neighbourPick.index.y - 1), (int)(neighbourPick.index.z));
                    //Debug.Log((int)Mathf.Max(gridInstance.ySize - 1, neighbourPick.index.y + 1));
                    var objectBeneath = gridInstance.GetGridObject((int)(neighbourPick.index.x), (int)Mathf.Min(gridInstance.ySize - 1, neighbourPick.index.y + 1), (int)(neighbourPick.index.z));

                    if (objectBeneath && objectBeneath != neighbour)
                        objectBeneath.collider.enabled = true;

                    List<Transform> aboveObjects = new List<Transform>();
                    neighbour.GetComponent<Picker>().GetAliveObjectsAbove(gridInstance, neighbour, aboveObjects);

                    //neighbour.GetComponent<Speciality>().Trigger();
                    //Debug.Log("Objects to break: " + aboveObjects.Count);
                    foreach (Transform t in aboveObjects)
                    {
                        t.GetComponent<Picker>().fadeOutCurrent = fadeOutDuration;
                        t.collider.enabled = false;
                        //t.GetComponent<Speciality>().Trigger();
                    }
                }
            }
        }
        transform.parent.audio.clip = explodeSound;
        return true;
    }

}
