
using UnityEngine;

public class Food : MonoBehaviour
{

    public int timeToGrow;
    public int energy = 100;
    public int xCoord;
    public int zCoord;

    // Start is called before the first frame update
    public virtual void Init(int x, int z)
    {
        xCoord = x;
        zCoord = z;
    }

}
