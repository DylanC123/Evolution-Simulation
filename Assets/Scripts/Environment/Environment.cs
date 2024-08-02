using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Environment : MonoBehaviour
{
    //------Mesh Data------
    static int width = 100;
    static int height = 100;
    public float scale = 30;
    static float waterLevel = 0.6f;
    MapGenerator map;

    //------spawn management------
    //------Populations------
    public GameObject foodPrefab;
    public GameObject obstaclePrefab;
    public GameObject rabbitPrefab;

    MapDisplay mapData;

    //Probability of spawning, the higher the less likely to be spawned
    public int foodProb = 150;
    public int obstacleProb = 400;
    public int rabbitProb = 150;
    public float foodSpawnSpeed = 10.0f;
    //walkable array
    public static bool[,] walkable;
    //walkable positions only
    public static List<Vector3> validPositions;
    //food array
    public static bool[,] foodArray;
    //water array
    public static bool[,] waterArray;
    //rabbit array
    public static List<GameObject> rabbits;

    //Called at start
    public void Start()
    {
        //create a noiseMap
        float[,] noiseMap = Noise.noiseGen(width, height, scale);

        //get MapGenerator script and create the map
        map = FindObjectOfType<MapGenerator>();
        map.CreateMap(width, height, waterLevel, noiseMap);

        //get mapDisplay script then get walkable array and food array
        MapDisplay mapData = FindObjectOfType<MapDisplay>();
        walkable = mapData.walkable;
        //setting waterArray (opposite of intial walkable array)
        waterArray = mapData.waterArray;
        //setting foodArray
        foodArray = mapData.foodArray;
        //init rabbitArray
        rabbits = new List<GameObject>();

        validPositions = new List<Vector3>();
        validPositions = GetValidPositions();
        //spawning initial populations at start
        InitialPop();
        //spawning food throughout the simulation
        InvokeRepeating("SpawnFood", 0, foodSpawnSpeed);
        GetRabbitPositions(rabbits);
        InvokeRepeating("PopulationSize", 0, 3.0f);
    }

    void PopulationSize()
    {
        Debug.Log("Population: " + rabbits.Count);
    }


    //generate random spawn position within the map boundaries and spawn object
    public void SpawnObjects(int objType, int probability, GameObject prefab)
    {
        Vector3 randomPos;
        //loop through all coords
        for (int y = 0; y < walkable.GetLength(1) - 1; y++)
        {
            for (int x = 0; x < walkable.GetLength(0) - 1; x++)
            {
                //set a random number
                int randomNum = Random.Range(0, probability);
                //check is random number is 1 and if that position is walkable
                if (randomNum == 1 && (walkable[x, y] == true))
                {
                    //create the random position and offset it so it fits the map
                    randomPos = new Vector3(x, 0.5f, y);
                    GameObject obj = Instantiate(prefab, randomPos, transform.rotation);
                    //if obstacle is being spawned then set it's spawn position to false
                    if (objType == 0)
                    {
                        walkable[x, y] = false;
                    }
                    else if (objType == 1)
                    {
                        //if food spawned then update food Array
                        foodArray[x, y] = true;
                    } else if (objType == 2)
                    {
                        Debug.Log("rabbit added to list");
                        rabbits.Add(obj);
                    }
                }
            }
        }
    }

    public void GetRabbitPositions(List<GameObject> list)
    {
        Debug.Log("Outputting all rabbit positions...");
        for (int i = 0; i < list.Count - 1; i++)
        {
            Debug.Log("rabbit " + i + ": " + list[i].transform.position);
        }
    }

    //Spawn Initial Populations
    public void InitialPop()
    {
        //Spawning objects
        //Obstacle
        SpawnObjects(0, obstacleProb, obstaclePrefab);
        //Food
        SpawnObjects(1, foodProb, foodPrefab);
        //Rabbit
        SpawnObjects(2, rabbitProb, rabbitPrefab);
    }


    public void SpawnFood()
    {
        SpawnObjects(1, foodProb, foodPrefab);
    }

    //All valid positions within the map
    public static List<Vector3> GetValidPositions()
    {
        //create a valid positions list
        List<Vector3> validPositions = new List<Vector3>();
        //loop through all coordinates in the map
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                //if tile is walkable then add to valid list
                if (walkable[x, y] == true)
                {
                    Vector3 pos = new Vector3(x, 0.5f, y);
                    validPositions.Add(pos);
                }
            }
        }
        return validPositions;
    }

    //Getting a random position within vision
    public static Vector3 GetRandomPos(Vector3 position, int vision)
    {
        List<Vector3> validPositions = new List<Vector3>();
        for (int y = (int)position.z + (vision / 2); y > (int)position.z - (vision / 2); y--)
        {
            for (int x = (int)position.x - (vision / 2); x < (int)position.x + (vision / 2); x++)
            {
                Vector3 pos = new Vector3(x, 0.5f, y);
                if (IsPosValid(pos))
                {
                    validPositions.Add(pos);
                }
            }
        }
        int randomNum = Random.Range(0, validPositions.Count);
        return validPositions[randomNum];
    }

    //Check if position is valid, within map
    public static bool IsPosValid(Vector3 position)
    {
        //check height and height
        if (position.z < height - 1 && position.x < width - 1)
        {
            if (position.z >= 0 && position.x >= 0)
            {
                return true;
            }
        }
        return false;
    }
    //check if walkable
    public static bool IsPosWalkable(Vector3 position)
    {
        //check walkable
        if (walkable[(int)position.x, (int)position.z])
        {
            return true;
        }
        return false;
    }

    //return list of all neighbours of current Node
    public static List<Vector3> GetNeighbours(Vector3 currentPos)
    {
        //creating node list
        List<Vector3> neighbours = new List<Vector3>();

        //check above
        Vector3 pos = new Vector3(currentPos.x, 0.5f, currentPos.z + 1);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }

        //check below
        pos = new Vector3(currentPos.x, 0.5f, currentPos.z - 1);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }
        //check right
        pos = new Vector3(currentPos.x + 1, 0.5f, currentPos.z);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }
        //check left
        pos = new Vector3(currentPos.x - 1, 0.5f, currentPos.z);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }

        //check top right
        pos = new Vector3(currentPos.x + 1, 0.5f, currentPos.z + 1);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }
        //check bottom right
        pos = new Vector3(currentPos.x + 1, 0.5f, currentPos.z - 1);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }
        //check top Left
        pos = new Vector3(currentPos.x - 1, 0.5f, currentPos.z + 1);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }
        //check bottom Left
        pos = new Vector3(currentPos.x - 1, 0.5f, currentPos.z - 1);
        if (IsPosValid(pos) && IsPosWalkable(pos))
        {
            neighbours.Add(pos);
        }

        return neighbours;
    }

    //Return the closest nieghbour to target to move to 
    public static Vector3 GetClosestNeighbour(List<Vector3> neighbours, Vector3 target)
    {
        //create distances list
        List<float> distances = new List<float>();
        //add distances
        for (int i = 0; i < neighbours.Count; i++)
        {
            distances.Add(Vector3.Distance(neighbours[i], target));
        }

        //get index of smallest distance
        float minDist = distances.Min();
        int index = distances.IndexOf(minDist);
        //return closet neighbour
        return neighbours[index];
    }


    //Return the closest position from list
    public static Vector3 GetClosest(List<Vector3> list, Vector3 currentPos)
    {
        //create distances list
        List<float> distances = new List<float>();
        //add distances
        for (int i = 0; i < list.Count; i++)
        {
            distances.Add(Vector3.Distance(list[i], currentPos));
        }

        //get index of smallest distance
        float minDist = distances.Min();
        int index = distances.IndexOf(minDist);
        //return closest food
        return list[index];
    }

    //Return closest food
    public static Vector3 FindFood(int vision, Vector3 position)
    {
        //creating a food list
        List<Vector3> food = new List<Vector3>();
        //looping through coordinates within vision
        for (int y = (int)position.z + (vision / 2); y > (int)position.z - (vision / 2); y--)
        {
            for (int x = (int)position.x - (vision / 2); x < (int)position.x + (vision / 2); x++)
            {
                Vector3 pos = new Vector3(x, 0.5f, y);
                if (IsPosValid(pos) && IsPosWalkable(pos))
                {
                    //check if position contains food
                    if (foodArray[x, y] == true)
                    {
                        //if it does then add food position
                        Vector3 foodPos = new Vector3(x, 0.5f, y);
                        food.Add(foodPos);
                    }
                }
            }
        }
        //check if any food in vision
        //if none then return a random position within vision
        //else return closest food
        if (food.Count == 0)
        {
            return GetRandomPos(position, vision);
        }
        else
        {
            return GetClosest(food, position);
        }
    }


    public static GameObject FindMate(Vector3 currentPos)
    {
        GameObject[] mates;
        mates = GameObject.FindGameObjectsWithTag("Rabbit");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = currentPos;
        foreach (GameObject mate in mates)
        {
            if (mate.transform.position != currentPos)
            {
                //get distance
                float tempDis = Vector3.Distance(mate.transform.position, position);
                if (tempDis < distance)
                {
                    closest = mate;
                    distance = tempDis;
                }
            }
        }
        return closest;
    }

    //Return closest Water
    public static Vector3 FindWater(int vision, Vector3 position)
    {
        //creating a water list
        List<Vector3> water = new List<Vector3>();
        //looping through all coordinates within vision
        for (int y = (int)position.z + (vision / 2); y > (int)position.z - (vision / 2); y--)
        {
            for (int x = (int)position.x - (vision / 2); x < (int)position.x + (vision / 2); x++)
            {
                if (IsPosValid(new Vector3(x, 0.5f, y)))
                {
                    if (waterArray[x, y] == true)
                    {
                        //if it does then add water position
                        Vector3 waterPos = new Vector3(x, 0.5f, y);
                        water.Add(waterPos);
                    }
                }
            }
        }

        //check if any water in vision
        //if none then return a random position within vision
        //else return closest water
        if (water.Count == 0)
        {
            return GetRandomPos(position, vision);
        }
        else
        {
            Animal.movingToWater = true;
            return GetClosest(water, position);
        }
    }

    //remove food from array
    public static void RemoveFood(Vector3 position)
    {
        foodArray[(int)position.x, (int)position.z] = false;
    }

    //crossing over to create a new chromosome
    public static List<int> CrossOver(List<int> parent1, List<int> parent2)
    {
        List<int> baby = new List<int>();
        //loop through all attributes in chromosome
        for (int i = 0; i < parent1.Count - 1; i++)
        {
            float random = Random.Range(0, 1);
            //choose p1
            if (random <= 0.5)
            {
                baby.Add(parent1[i]);
            }
            //choose p2
            else {
                baby.Add(parent2[i]);
            }
        }
        //Add mutation 
        return baby;
    }
}


