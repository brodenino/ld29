using UnityEngine;
using System.Collections.Generic;

public class GridInstance : MonoBehaviour {
    public int xSize = 8;
    public int zSize = 8;

    public int yBeginHeight = 3;
    public float ySpeed = 0.1f;

    public float marigin = 1.2f;

    //public Transform[,,]    grid;
    public Transform        cubePrefab;

    public List<Transform[,]> dynGrid = new List<Transform[,]>();

    public Transform[,] GetGrid2D(int height)
    {
        return dynGrid[height];
    }

    public Transform GetGridObject(int x, int y, int z)
    {
        return dynGrid[y][x, z];
    }

    public int ySize { get { return dynGrid.Count; } }

    // Use this for initialization
    void Start () {
        //grid = new Transform[xSize, ySize, zSize];
        
        var palettes = GetComponent<LayerColors>().palettes;

        for (int y = 0; y < yBeginHeight; y++)
        {
            Transform[,] grid2D = new Transform[xSize, zSize];
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    var obj = Instantiate(cubePrefab, new Vector3(x, y, z) * marigin, Quaternion.identity) as Transform;

                    obj.parent = transform;
                    obj.GetComponent<Picker>().index = new Vector3(x, y, z);

                    obj.renderer.material.color = palettes[y].colors[Random.Range(0, palettes[y].colors.Length)];
                    if (y < ySize - 1)
                        obj.collider.enabled = false;

                    //grid[x, y, z] = obj;
                    grid2D[x, z] = obj;
                }
            }
            dynGrid.Add(grid2D);
        }
        // Check for cubes that can't be taken
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    //var obj = grid[x, y, z];
                    var obj = dynGrid[y][x, z];

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
                                //otherObj = grid[nx, y, nz];
                                var d = dynGrid[y];
                                otherObj = d[nx, nz];

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
                    //var obj = grid[x, y, z];
                    var obj = dynGrid[y][x, z];
                    obj.position += Vector3.up * ySpeed * Time.deltaTime;
                }
            }
        }
    }
}
