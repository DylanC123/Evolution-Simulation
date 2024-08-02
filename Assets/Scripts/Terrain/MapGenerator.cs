using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public void CreateMap(int width, int height, float waterLevel, float[,] noiseMap)
    {
        //creating a noiseMap
        //displaying map onto the scene
        MapDisplay mapdisplay = FindObjectOfType<MapDisplay>();
        mapdisplay.drawMap(width, height, waterLevel, noiseMap);
    }

}

