using UnityEngine;
using System.Collections.Generic;

public class Picker : MonoBehaviour {
    public Transform pickPrefab;

    static List<Transform> pickedObjects = new List<Transform>();

    public Vector3 index;
    public int colorType;
    public float fadeOutDuration = 0.5f;

    public float fadeOutCurrent = 0;

    public AudioClip explodeSound;
    public AudioClip clearSound;
    public AudioClip powerUpSpawnSound;

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        if (fadeOutCurrent > 0)
        {
            if (fadeOutCurrent == fadeOutDuration)
                transform.parent.GetComponentInChildren<Score>().frameScore++;

            fadeOutCurrent = Mathf.Max(0, fadeOutCurrent - Time.deltaTime);

            var color = renderer.material.color;
            color.a = Mathf.Lerp(1, 0, (fadeOutDuration - fadeOutCurrent) / fadeOutDuration);
            renderer.material.color = color;

            foreach (Transform child in transform)
            {
                child.renderer.material.color = color;
            }

            transform.position -= Vector3.up * 1.0f * Time.deltaTime;

            if (fadeOutCurrent == 0)
            {
                renderer.enabled = false;
                foreach (Transform child in transform)
                {
                    child.renderer.enabled = false;
                    transform.parent.GetComponent<GridInstance>().dynGrid[(int)index.y][(int)index.x, (int)index.z] = null;
                    //Destroy(gameObject);
                }
            }
        }
    }

    bool IsValidPick()
    {
        // If the picked objects list is empty, then it is valid
        if (pickedObjects.Count == 0)
            return true;

        // If the current object has a matching color, and is within radius of the previous picked object, then it is also valid.
        var previousPick = pickedObjects[pickedObjects.Count-1];
        float distance = Vector3.Distance(previousPick.position, transform.position);
        if (distance <= 2.0f && previousPick.renderer.material.color == renderer.material.color && GetComponentInChildren<SelectionCube>() == null)
        {
            // Debug.Log(GetComponent<Picker>().index + " " + previousPick.GetComponent<Picker>().index);
            // Should also exist in the current layer
            if (GetComponent<Picker>().index.y == previousPick.GetComponent<Picker>().index.y)
                return true;
            //if (index.y == previousPick.GetComponent<Picker>().index.y)
            //    return true;
        }

        return false;
    }

    void PickThis()
    {
        pickedObjects.Add(transform);
        transform.collider.enabled = false;
        audio.Play();

        // Instance a pick highlight
        var highlight = Instantiate(pickPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as Transform;
        highlight.parent = transform;
    }

    void OnMouseDown()
    {
        if (IsValidPick())
        {
            PickThis();
        }
    }

    void OnMouseEnter()
    {
        if (pickedObjects.Count == 0)
            return;

        if (IsValidPick())
        {
            PickThis();
        }
    }

    public bool HasAliveObjectsAbove(GridInstance gridInstance)
    {
        var pickComponent = GetComponent<Picker>();
        var objectAbove = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Max(0, pickComponent.index.y - 1), (int)(pickComponent.index.z));

        if (objectAbove && objectAbove != transform && objectAbove.renderer.enabled)
            return true;
        else
            return false;
    }

    public void GetAliveObjectsAbove(GridInstance gridInstance, Transform pick, List<Transform> aliveObjects)
    {
        var pickComponent = pick.GetComponent<Picker>();
        //var objectAbove = gridInstance.grid[(int)(pickComponent.index.x), (int)Mathf.Min(gridInstance.ySize - 1, pickComponent.index.y + 1), (int)(pickComponent.index.z)];
        //var objectAbove = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Min(gridInstance.ySize - 1, pickComponent.index.y + 1), (int)(pickComponent.index.z));
        //Vector3 debug = new Vector3((int)(pickComponent.index.x), (int)Mathf.Max(0, pickComponent.index.y - 1), (int)(pickComponent.index.z));
        //Debug.Log("Above: " + pick.GetInstanceID() + " " + debug);
        var objectAbove = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Max(0, pickComponent.index.y - 1), (int)(pickComponent.index.z));

        if (objectAbove && objectAbove != pick && objectAbove.renderer.enabled)
        {
            aliveObjects.Add(objectAbove);
            GetAliveObjectsAbove(gridInstance, objectAbove, aliveObjects);
        }
    }

    void OnMouseUp()
    {
        if (pickedObjects.Count < 2)
        {
            if (pickedObjects[0].renderer.enabled)
                pickedObjects[0].collider.enabled = true;
            Destroy(pickedObjects[0].GetComponentInChildren<SelectionCube>().gameObject);
            pickedObjects.Clear();
        }


        var gridInstance = transform.parent.GetComponent<GridInstance>();

        bool explosion = false;
        foreach (var pick in pickedObjects)
        {
            
            pick.collider.enabled = false;


            var pickComponent = pick.GetComponent<Picker>();

            //var objectBeneath = gridInstance.grid[(int)(pickComponent.index.x), (int)Mathf.Max(0, pickComponent.index.y - 1), (int)(pickComponent.index.z)];
            //var objectBeneath = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Max(0, pickComponent.index.y - 1), (int)(pickComponent.index.z));
            var objectBeneath = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Min(gridInstance.ySize - 1, pickComponent.index.y + 1), (int)(pickComponent.index.z));
       //     Debug.Log("A");

            if (objectBeneath && objectBeneath != pick)
                objectBeneath.collider.enabled = true;

            //var objectAbove = gridInstance.grid[(int)(pickComponent.index.x), (int)Mathf.Min(gridInstance.ySize - 1, pickComponent.index.y + 1), (int)(pickComponent.index.z)];
            //var objectAbove = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Min(gridInstance.ySize - 1, pickComponent.index.y + 1), (int)(pickComponent.index.z));
     //       Debug.Log((int)Mathf.Max(0, pickComponent.index.y - 1));
            var objectAbove = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Max(0, pickComponent.index.y - 1), (int)(pickComponent.index.z));
        //    Debug.Log("B");

            if (objectAbove && objectAbove != pick && objectAbove.collider.enabled)
            {
                objectAbove.GetComponent<Picker>().fadeOutCurrent = fadeOutDuration;
                objectAbove.collider.enabled = false;
            }

            pickComponent.fadeOutCurrent = fadeOutDuration;
            if (pick.transform.childCount > 0)
                pick.GetComponentInChildren<SelectionCube>().renderer.enabled = false;

            bool b = pick.GetComponent<Speciality>().Trigger();
            if (b && !explosion)
                explosion = true;
        }

        int i = 0;

        // TODO: Instead of exploding blocks if the player gets more than 6 blocks cleared, spawn a powerup
        // on the layer below on the last tile that was pressed.
        int spawnChance = Random.Range(2, 12);
        if (pickedObjects.Count >= spawnChance)
        {
            // Make a special tile if there is one below!
            var pick = pickedObjects[pickedObjects.Count-1];
            var pickComponent = pick.GetComponent<Picker>();
            var objectBeneath = gridInstance.GetGridObject((int)(pickComponent.index.x), (int)Mathf.Min(gridInstance.ySize - 1, pickComponent.index.y + 1), (int)(pickComponent.index.z));

            if (objectBeneath && objectBeneath != pick)
            {
                objectBeneath.GetComponent<Speciality>().isAvailable = true;
                var powerupOverlay = objectBeneath.GetComponentInChildren<PowerupOverlay>();
                powerupOverlay.renderer.material.mainTexture = objectBeneath.GetComponent<Speciality>().texture;
                powerupOverlay.renderer.material.color = Color.white;

            }
            transform.parent.audio.clip = powerUpSpawnSound;
        }
        else
            transform.parent.audio.clip = clearSound;

        if (explosion)
            transform.parent.audio.clip = explodeSound;
         /*
        if (pickedObjects.Count >= 6)
        {
           // Debug.Log("Explode");
            var explodeObj = pickedObjects[pickedObjects.Count-1];
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

                    if (neighbour != explodeObj)
                    {
                        neighbour.GetComponent<Picker>().fadeOutCurrent = fadeOutDuration;
                        neighbour.collider.enabled = false;

                        var neighbourPick = neighbour.GetComponent<Picker>();
                        //var objectBeneath = gridInstance.grid[(int)(neighbourPick.index.x), (int)Mathf.Max(0, neighbourPick.index.y - 1), (int)(neighbourPick.index.z)];
                        //var objectBeneath = gridInstance.GetGridObject((int)(neighbourPick.index.x), (int)Mathf.Max(0, neighbourPick.index.y - 1), (int)(neighbourPick.index.z));
                        //Debug.Log((int)Mathf.Max(gridInstance.ySize - 1, neighbourPick.index.y + 1));
                        var objectBeneath = gridInstance.GetGridObject((int)(neighbourPick.index.x), (int)Mathf.Min(gridInstance.ySize - 1, neighbourPick.index.y + 1), (int)(neighbourPick.index.z));

                        if (objectBeneath != neighbour)
                            objectBeneath.collider.enabled = true;

                        List<Transform> aboveObjects = new List<Transform>();
                        GetAliveObjectsAbove(gridInstance, neighbour, aboveObjects);

                        //Debug.Log("Objects to break: " + aboveObjects.Count);
                        foreach (Transform t in aboveObjects)
                        {
                            t.GetComponent<Picker>().fadeOutCurrent = fadeOutDuration;
                            t.collider.enabled = false;
                        }
                    }
                }
            }
            transform.parent.audio.clip = explodeSound;
        }
        else
            transform.parent.audio.clip = clearSound;
        */
        if (pickedObjects.Count > 0)
            transform.parent.audio.Play();

        pickedObjects.Clear();
    }
}
