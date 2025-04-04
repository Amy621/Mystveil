using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBase : MonoBehaviour
{
    // level parameters
    public static int numLevels;
    public static int curLevel = 1;
    public GenerateGrid gridGenerator;
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
    }

    public static void onDestroy() 
    {
        for(int i = 0; i < currentObjs.Count; i++) {
            Destroy(currentObjs[i]);
        }
        currentObjs.Clear();
    }

    public static void onCreateLevel()
    {
        ForestBase forestBase = FindObjectOfType<ForestBase>();
        if (forestBase != null)
        {
            forestBase.gridGenerator.GenerateNavMesh();
            forestBase.gridGenerator.GenerateLevel();
        }
        else
        {
            Debug.LogError("ForestBase not found in the scene.");
        }
    }

    public static void onCreateBossLevel()
    {
        ForestBase forestBase = FindObjectOfType<ForestBase>();
        if (forestBase != null)
        {
            forestBase.gridGenerator.GenerateNavMesh();
            forestBase.gridGenerator.GenerateBossLevel();
        }
        else
        {
            Debug.LogError("ForestBase not found in the scene.");
        }
    }
}
