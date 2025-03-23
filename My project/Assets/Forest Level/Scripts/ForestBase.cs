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
    public static List<GameObject> currentObjs;

    // Start is called before the first frame update
    void Start()
    {
        // getting number of levels
        numLevels = Random.Range(3, 6);
        Debug.Log("Number of levels: " + numLevels);

        gridGenerator = FindObjectOfType(typeof(GenerateGrid)) as GenerateGrid;
        gridGenerator.GenerateLevel();

        currentObjs = gridGenerator.getAllObjects();

        playerSpawn = GameObject.Find("Player").transform.position;
    }

    public static void onDestroy() 
    {
        for(int i = 0; i < currentObjs.Count; i++) {
            Destroy(currentObjs[i]);
        }
        currentObjs.Clear();
    }
}
