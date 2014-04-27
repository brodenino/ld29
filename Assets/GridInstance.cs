using UnityEngine;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

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

    LayerColors.ColorPalette[] palettes;
    Texture2D[]                overlays;   

    void AddGridLayer(float overridePosY = Int16.MaxValue)
    {
        Transform[,] grid2D = new Transform[xSize, zSize];

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                var pos = new Vector3(x, -dynGrid.Count, z) * marigin;

                if (overridePosY != Int16.MaxValue)
                {
                    pos.y = overridePosY;
                }
                var obj = Instantiate(cubePrefab, pos, Quaternion.identity) as Transform;
                

                obj.parent = transform;
                obj.GetComponent<Picker>().index = new Vector3(x, dynGrid.Count, z);

                int y = ySize % palettes.Length;
                int type = Random.Range(0, palettes[y].colors.Length);

                obj.GetComponent<Picker>().colorType = type;

                var child = obj.GetComponentInChildren<ShapeOverlay>();
                child.renderer.material.mainTexture = overlays[type];
                
                obj.renderer.material.color = palettes[y].colors[type];

                /*if (ySize > 0)
                {
                
                    obj.collider.enabled = false;
                }*/
                //grid[x, y, z] = obj;
                grid2D[x, z] = obj;
            }
        }
        dynGrid.Add(grid2D);

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                //var obj = grid[x, y, z];
                var obj = dynGrid[ySize - 1][x, z];

                // Check if the object is the topmost one. If not, disable collider.
                var pick = obj.GetComponent<Picker>();
                var list = new List<Transform>();
                pick.GetAliveObjectsAbove(this, pick.transform, list);
                if (pick.HasAliveObjectsAbove(this))
                    pick.collider.enabled = false;
                else
                    pick.collider.enabled = true;

                // Check for cubes that can't be taken
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
                            otherObj = dynGrid[ySize - 1][nx, nz];

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
                    var colorType = otherObj.GetComponent<Picker>().colorType;
                    obj.GetComponent<Picker>().colorType = colorType;
                    var child = obj.GetComponentInChildren<ShapeOverlay>();
                    child.renderer.material.mainTexture = overlays[colorType];
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        //grid = new Transform[xSize, ySize, zSize];
        
        palettes = GetComponent<LayerColors>().palettes;
        overlays = GetComponent<LayerColors>().overlays;

        for (int y = 0; y < yBeginHeight; y++)
        {
            AddGridLayer();
        }

    }
    
    int     heightIndex = 0;
    float   heightOffset = 0;

    // Update is called once per frame
    void Update () {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                var grid2D = dynGrid[y];
                for (int z = 0; z < zSize; z++)
                {
                    //var obj = grid[x, y, z];
                    var obj = grid2D[x, z];
                    if (obj)
                        obj.position += Vector3.up * ySpeed * Time.deltaTime;
                }
            }
        }

        // Just check one object position, it doesn't matter. Check height, and see when a new grid should be added.
        heightOffset = dynGrid[heightIndex][0, 0].position.y;
        if (heightOffset >= marigin)
        {
            heightIndex++;
            AddGridLayer( dynGrid[ySize - 1][0, 0].position.y - marigin );
        }
    }
}
