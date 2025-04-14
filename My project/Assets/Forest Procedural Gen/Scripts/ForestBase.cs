using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBase : MonoBehaviour
{
    // level parameters
    public static int numLevels;
    public static int curLevel = 1;
    [SerializeField] private GenerateGrid gridGenerator;
    [SerializeField] private MiniMap miniMapScript;

    // Start is called before the first frame update
    void Start()
    {
        // getting number of levels
        numLevels = Random.Range(3, 6);
        Debug.Log("Number of levels: " + numLevels);
        //gridGenerator.GenerateLevel(true);
        gridGenerator.GenerateBossLevel();
     }

    public static void onCreateLevel()
    {
        Debug.Log("FORBASE - In create level...");
        ForestBase forestBase = FindObjectOfType<ForestBase>();
        Debug.Log(forestBase);
        if (forestBase != null)
        {
            forestBase.gridGenerator.GenerateNavMesh();
            forestBase.gridGenerator.GenerateLevel(false);
            forestBase.miniMapScript.ResetMiniMap();
        }
        else
        {
            Debug.LogError("ForestBase not found in the scene.");
        }
    }

    public static void onCreateBossLevel()
    {
        Debug.Log("FORBASE - In create boss level...");
        ForestBase forestBase = FindObjectOfType<ForestBase>();
        if (forestBase != null)
        {
            forestBase.gridGenerator.GenerateNavMesh();
            forestBase.gridGenerator.GenerateBossLevel();
            forestBase.miniMapScript.ResetMiniMap();
        }
        else
        {
            Debug.LogError("ForestBase not found in the scene.");
        }
    }
}
