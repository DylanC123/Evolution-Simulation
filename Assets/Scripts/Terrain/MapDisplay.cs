
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    //Environment Data
    public bool isLand;
    public int numTiles;
    public bool[,] walkable;
    public bool[,] waterArray;
    public bool[,] foodArray;
    
    public terrains[] terrains;

    public void drawMap(int width, int height, float waterLevel, float[,] noiseMap)
    {
        //initialising lists
        Texture2D texture = new Texture2D(width, height);

        Color[] colours = new Color[width * height];

        walkable = new bool[width, height];
        waterArray = new bool[width, height];
        foodArray = new bool[width, height];

        //looping through all coordinates
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int i = 0; i < terrains.Length; i++) {
                    //setting first terrain greater than noiseMap[x, y]
                    if (terrains[i].height >= noiseMap[x, y])
                    {
                        colours[y * width + x] = terrains[i].colour;
                        //if noiseMap value less than waterLevel then set isLand to false
                        if (colours[y * width + x] == terrains[0].colour || colours[y * width + x] == terrains[1].colour)
                        {
                            isLand = false;
                        }
                        else
                        {
                            isLand = true;
                        }
                        //add isLand bool to walkable array
                        walkable[x, y] = isLand;
                        //opposite of walkable array
                        waterArray[x, y] = !isLand;
                        //add 1 to numTiles
                        numTiles++;
                        //setting all coordinates of foodArray to false
                        foodArray[x, y] = false;
                        break;
                    }
                }
            }
        }

        //applying texture settings
        //filter point makes the lines between colours clean
        texture.filterMode = FilterMode.Point;
        //prevents colours going to the other side of the map
        texture.wrapMode = TextureWrapMode.Clamp;
        //setting colours to positions in the texture
        texture.SetPixels(colours);
        //applying texture
        texture.Apply();

        DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), texture);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}

//creating a public struct to add terrain types in map
[System.Serializable]
public struct terrains
{
    public Color colour;
    public float height;
}

