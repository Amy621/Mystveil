using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBase : MonoBehaviour
{
    // level parameters
    public static int numLevels;
    public static int curLevel = 1;
    public GenerateGrid gridGenerator;
    public static Vector3 playerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        // getting number of levels
        numLevels = Random.Range(3, 6);
        gridGenerator = FindObjectOfType(typeof(GenerateGrid)) as GenerateGrid;
        gridGenerator.GenerateLevel();

        playerSpawn = GameObject.Find("Player").transform.position;
    }
}
