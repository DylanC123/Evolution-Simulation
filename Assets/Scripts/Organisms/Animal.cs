using System;
using System.Collections.Generic;
using UnityEngine;

public class Animal : LivingObject
{
    
    //------Traits--------
    public float speed = 5.0f;
    public int metabolism;
    public int vision;
    public int hungerIncrement;
    public int thirstIncrement;
    public int hunger = 0;
    public int thirst = 0;
    public float reproductiveUrge = 0;

    //------List to store traits------
    public static List<int> chromosome;

    //------Movement------
    public bool moving = false;
    public static bool movingToWater = false;
    public int move = 0;
    public int maxMoves;
    Vector3 target;

    public GameObject rabbitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        List<int> chromosome = new List<int>();
        //setting random attributes
        metabolism = UnityEngine.Random.Range(40, 90);
        vision = UnityEngine.Random.Range(15, 40);
        hungerIncrement = UnityEngine.Random.Range(1, 4);
        thirstIncrement = UnityEngine.Random.Range(1, 4);

        //Creating chromosome
        chromosome.Add(metabolism);
        chromosome.Add(vision);
        chromosome.Add(hungerIncrement);
        chromosome.Add(thirstIncrement);

        maxMoves = 15;
        InvokeRepeating("Decision", 0, 0.5f);
    }
    

    void Decision()
    {
        //movement
        //if no target then get new one
        if (!moving)
        {
            newTarget();
            move = 0;
        }
        //if moving
        else
        {
            //increment move and if greater than maxMoves then get new target
            move += 1;
            if (move > maxMoves)
            {
                newTarget();
            }
        }
        //physically moving object

        transform.position = nextPosition(transform.position, target);
        moving = !IsTargetReached(transform.position, target);

        //increase thirst
        thirst += thirstIncrement;
        //increase hunger
        hunger += hungerIncrement;
        //increase reproductive urge by the reverse percentage full of thirst and hunger multiplied by 0.1
        reproductiveUrge += (float) 0.005;
        //Check
        IsDead();
    }

    //get a new target
    public void newTarget()
    {
        if (validToReproduce())
        {
            GameObject mate = Environment.FindMate(transform.position);
            target = mate.transform.position;
        }
        else if (hunger >= thirst)
        {
            target = Environment.FindFood(vision, transform.position);
        }
        else
        {
            target = MoveToWater(transform.position);
        }
    }

    public Vector3 MoveToWater(Vector3 currentPos)
    {
        //Getting all water in vision and return the closest water
        Vector3 closestWater = Environment.FindWater(vision, transform.position);
        //Get valid neighbours of water
        List<Vector3> neighboursList = Environment.GetNeighbours(closestWater);
        //return the target position as the closest valid neighbour to rabbit
        return Environment.GetClosestNeighbour(neighboursList, currentPos);
    }

    //Check if the animal has reached the target
    public bool IsTargetReached(Vector3 currentPos, Vector3 targetPos)
    {
        if (currentPos == targetPos)
        {
            if (movingToWater)
            {
                thirst = 0;
                movingToWater = false;
            }
            return true;
        }
        return false;
    }

    //decides and returns the next position to move to
    public static Vector3 nextPosition(Vector3 currentPos, Vector3 targetPos)
    {
        List<Vector3> neighbours = new List<Vector3>();
        neighbours = Environment.GetNeighbours(currentPos);
        if (neighbours.Count == 0)
        {
            Debug.Log("No neighbours");
            return new Vector3(0, 0, 0);
        }
        return Environment.GetClosestNeighbour(neighbours, targetPos);
    }

    //Detecting Collisions
    void OnTriggerEnter(Collider other)
    {
        //If food then destroy gameObject
        if (other.gameObject.tag == "Food")
        {
            //Remove food from foodArray in the Environment script
            Vector3 pos = other.gameObject.transform.position;
            Environment.RemoveFood(pos);
            //getting energy and removing it from hunger

            hunger -= (int) Math.Round((float)other.gameObject.GetComponent<Food>().energy * ((float)metabolism/100));
            //if less than 0 then set to 0 
            if (hunger < 0)
            {
                hunger = 0;
            }
            //Destroy food
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Rabbit" && validToReproduce())
        {
            //reset reproductive urge
            reproductiveUrge = 0.0f;
            //create new baby and offset it slightly
            GameObject obj = Instantiate(rabbitPrefab, transform.position + new Vector3(1f, 0.5f, 1f), transform.rotation);
            ///update rabbits
            Environment.rabbits.Add(obj);
        }
    }
    //Check if object is dead
    public void IsDead()
    {
        if (hunger >= 150 || thirst >= 150)
        {
            //removing rabbit from rabbit array
            Environment.rabbits.Remove(gameObject);
            //Remove gameObject from world
            Destroy(gameObject);
        }
    }

    public bool validToReproduce()
    {
        if (thirst < 60 && hunger < 60)
        {
            if (reproductiveUrge > 0.2)
            {
                return true;
            }
        }
        return false;
    }
}
