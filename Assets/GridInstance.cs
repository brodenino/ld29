using UnityEngine;
using System.Collections;

public class GridInstance : MonoBehaviour {
    public int xSize = 8;
    public int ySize = 3;
    public int zSize = 8;

    public int yBeginHeight = 3;
    public float ySpeed = 0;

    public float marigin = 1.2f;

    public Transform[,,]    grid;
    public Transform        cubePrefab;

    // Use this for initialization
    void Start () {
        grid = new Transform[xSize, ySize, zSize];
        
        var palettes = GetComponent<LayerColors>().palettes;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < yBeginHeight; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    var obj = Instantiate(cubePrefab, new Vector3(x, y, z) * marigin, Quaternion.identity) as Transform;

                    obj.parent = transform;
                    obj.GetComponent<Picker>().index = new Vector3(x, y, z);

                    obj.renderer.material.color = palettes[y].colors[Random.Range(0, palettes[y].colors.Length)];
                    if (y < ySize - 1)
                        obj.collider.enabled = false;

                    grid[x, y, z] = obj;
                }
            }
        }

        // Check for cubes that can't be taken
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    var obj = grid[x, y, z];

                    bool canBeTaken = false;
                    Transform otherObj = null;
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
                        {
                            int nx = x + offsetX;
                            int nz = z + offsetZ;

                            if (offsetX != 0 &&
                                offsetZ != 0 &&
                                nx < xSize &&
                                nz < zSize &&
                                nx >= 0 &&
                                nz >= 0)
                            {
                                otherObj = grid[nx, y, nz];

                                if (obj.renderer.material.color == otherObj.renderer.material.color)
                                {
                                    canBeTaken = true;
                                    break;
                                }
                            }
                        }
                        if (canBeTaken)
                            break;
                    }
                    if (!canBeTaken)
                    {
                        obj.renderer.material.color = otherObj.renderer.material.color;
                    }
                }
            }
        }
    
    }
    
    // Update is called once per frame
    void Update () {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    var obj = grid[x, y, z];
                    obj.position += Vector3.up * ySpeed * Time.deltaTime;

                    Vector3 roundAdjust = obj.position + Vector3.one * 0.5f;
                }
            }
        }
    }
}
