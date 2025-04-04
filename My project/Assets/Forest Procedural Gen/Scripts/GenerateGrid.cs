using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GenerateGrid : MonoBehaviour
{
    // generating player spawn
    private Vector3 playerSpawn;

    // floor/ground parameters
    public GameObject grassFloorObject;
    public GameObject nonWalkFloorObject;
    private int worldSizeX = 30;
    private int worldSizeZ = 35;
    private int borderLength = 5;
    private int gridOffset = 3;
    private List<Vector3> grassPositions = new List<Vector3>();
    private List<Vector3> floorPositions = new List<Vector3>();
    private List<Vector3> borderPositions = new List<Vector3>();
    private Vector3 nextSceneRockPosition;

    //navmesh
    private NavMeshSurface navMeshSurface;

    // Creating 1v1 boss battle enemy
    public GameObject bossEnemy;
    private List<GameObject> bossEnemyList = new List<GameObject>();
    private int bossLevelSizeX = 25;
    private int bossLevelSizeZ = 25;

    // Creating pockets of collectables and monsters to spawn
    public GameObject Spider;
    private List<GameObject> monsterList = new List<GameObject>();
    public GameObject waypointPrefab;
    public string monsterTag = "Monster";

    public GameObject Moonpetal_Blossom;
    public GameObject Shadowberry;
    public GameObject Whisperwood;
    public GameObject Sunstone_Seed;
    public GameObject Emberleaf;
    public GameObject Starlight_Berry;
    
    private List<GameObject> collectableList = new List<GameObject>();

    private int numMonsterAreas;
    private int numCollectableAreas;

    // Creating objects to spawn (non-collectable)
    public GameObject grassSpawn;
    public GameObject rock1Spawn;
    public GameObject rock2Spawn;
    public GameObject tree1Spawn;
    public GameObject tree2Spawn;
    public GameObject treeBorderSpawn;
    public GameObject logSpawn;
    public GameObject toNextArea;
    private List<GameObject> allItems = new List<GameObject>();

    public List<GameObject> getAllObjects() {
        return allItems;
    }

    public Vector3 getPlayerSpawnCoords() {
        return playerSpawn;
    }

    public void GenerateLevel() {
        Debug.Log("Inside generate level");

        for(int x = 0; x < worldSizeX; x++) 
        {
            // make the border
            if (x <= 2) {
                Debug.Log("making the border before player spawn...");
                for (int z = 0; z < worldSizeZ; z++) {
                    Vector3 pos = new Vector3(x * gridOffset,
                    0,
                    z * gridOffset);

                    GameObject nonWalkFloor = Instantiate(nonWalkFloorObject,
                    pos,
                    Quaternion.identity) as GameObject;

                    allItems.Add(nonWalkFloor);
                    borderPositions.Add(nonWalkFloor.transform.position);
                }
            }
            // make the player spawn area
            else if (x <= 5) {
                Debug.Log("making the player spawn area...");
                for (int z = 0; z < worldSizeZ; z++) {
                    Vector3 pos = new Vector3(x * gridOffset,
                    0,
                    z * gridOffset);

                    if (z < 15 || z > 20) {
                        GameObject nonWalkFloor = Instantiate(nonWalkFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;
                        allItems.Add(nonWalkFloor);

                        borderPositions.Add(nonWalkFloor.transform.position);
                    } else {
                        GameObject floor = Instantiate(grassFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;
                        allItems.Add(floor);

                        if (x == 4 && z == 17) {
                            playerSpawn = pos;
                        }

                        grassPositions.Add(floor.transform.position);

                        floor.transform.SetParent(this.transform);
                    }
                } 
            }
            // make the level area
            else {
                Debug.Log("making the rest of the level...");
                for(int z = 0; z < worldSizeZ; z++) {
                    Vector3 pos = new Vector3(x * gridOffset,
                    0,
                    z * gridOffset);

                    // making the borders less straight line and more random
                    int offsetRandom = Random.Range(0, 2);

                    // adding in the next scene rock
                    if (x == 25 && z == 15) {
                        nextSceneRockPosition = pos;
                    }

                    if(z < borderLength + offsetRandom || z > worldSizeZ - borderLength - offsetRandom || x < borderLength + offsetRandom || x > worldSizeX - borderLength - offsetRandom) {
                        GameObject nonWalkFloor = Instantiate(nonWalkFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;
                        allItems.Add(nonWalkFloor);

                        borderPositions.Add(nonWalkFloor.transform.position);

                        nonWalkFloor.transform.SetParent(this.transform);
                    } else {
                        GameObject floor = Instantiate(grassFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;
                        allItems.Add(floor);

                        floorPositions.Add(floor.transform.position);
                        grassPositions.Add(floor.transform.position);

                        floor.transform.SetParent(this.transform);
                    }
                }

            }
        }

        // spawn in the player
        Debug.Log("spawning the player...");
        GameObject.Find("Player").transform.position = playerSpawn;

        // generate navmesh
        // GenerateNavMesh();

        // spawn monsters
        Debug.Log("spawning monsters...");
        SpawnMonsters();

        // GenerateWaypoints();

        // spawn collectable items
        Debug.Log("spawning collectables...");
        SpawnCollectables();

        // spawn the obj that will be used to go to the next level / scene
        Debug.Log("spawning the rock used for going to next scene...");
        SpawnNextSceneRock();

        // spawning the objects (non-interactable)
        Debug.Log("spawning all other objects...");
        SpawnObjects();
    }

    public void GenerateBossLevel() {
        for(int x = 0; x < bossLevelSizeX; x++) 
        {
            for(int z = 0; z < bossLevelSizeZ; z++) {
                Vector3 pos = new Vector3(x * gridOffset,
                0,
                z * gridOffset);

                // adding in the next scene rock
                if (x == 20 && z == 10) {
                    nextSceneRockPosition = pos;
                }

                // adding in the player spawn
                if (x == 7 && z == 10) {
                    playerSpawn = pos;
                }

                if(z < borderLength || z > bossLevelSizeZ - borderLength || x < borderLength || x > bossLevelSizeX - borderLength) {
                    GameObject nonWalkFloor = Instantiate(nonWalkFloorObject,
                    pos,
                    Quaternion.identity) as GameObject;
                    allItems.Add(nonWalkFloor);

                    borderPositions.Add(nonWalkFloor.transform.position);

                    nonWalkFloor.transform.SetParent(this.transform);
                } else {
                    GameObject floor = Instantiate(grassFloorObject,
                    pos,
                    Quaternion.identity) as GameObject;
                    allItems.Add(floor);

                    floor.transform.SetParent(this.transform);
                }
            }

        }

        // spawning border trees
        for (int c = 0; c < borderPositions.Count; c++) {
            GameObject borderTree = Instantiate(treeBorderSpawn,
            SpawnBorderTrees(c),
            Quaternion.identity);
            allItems.Add(borderTree);

            borderTree.transform.SetParent(this.transform);
        }

        // spawn in the player
        GameObject.Find("Player").transform.position = playerSpawn;

        SpawnNextSceneRock();
        
    }

    public void GenerateNavMesh() {
        if (navMeshSurface == null)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            Debug.Log(navMeshSurface);
            navMeshSurface.BuildNavMesh();
            Debug.Log("building nav mesh!");
        }

        if (navMeshSurface != null)
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            Debug.Log("updating nav mesh!");
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found on this GameObject!");
        }
    }

    

    private void SpawnCollectables() {
        // adding in the collectable objects to the list
        collectableList.Add(Moonpetal_Blossom);
        collectableList.Add(Shadowberry);
        collectableList.Add(Whisperwood);
        collectableList.Add(Sunstone_Seed);
        collectableList.Add(Emberleaf);
        collectableList.Add(Starlight_Berry);


        numCollectableAreas = Random.Range(3, 7);
        
        // how many collectable item areas total of this level
        for (int i = 0; i < numCollectableAreas; i++) {
            int randCollectableIndex = Random.Range(0, collectableList.Count - 1);
            int randNumOfCollectable = Random.Range(2, 6);
            int index = Random.Range(0, floorPositions.Count - 1);

            // store all the coords possible
            List<string> possibleCoords = new List<string>();

            for (int m = 0; m <= 3; m++) {
                for (int n = 0; n <= 3; n++) {
                    possibleCoords.Add(m + "," + n);
                }
            }

            // how many collectables per area
            for(int j = 0; j < randNumOfCollectable; j++) {
                int randIndex = Random.Range(0, possibleCoords.Count - 1);
                string rawString = possibleCoords[randIndex];

                Vector3 newPos = new Vector3 (
                    floorPositions[index].x - int.Parse(rawString.Split(',')[0]),
                    floorPositions[index].y,
                    floorPositions[index].z - int.Parse(rawString.Split(',')[1])
                );

                possibleCoords.RemoveAt(randIndex);

                GameObject collectable = Instantiate(collectableList[randCollectableIndex],
                newPos,
                Quaternion.identity);
                allItems.Add(collectable);

                collectable.transform.SetParent(this.transform);
            }

            floorPositions.RemoveAt(index);
        }
        
    }

    private void GenerateWaypoints() {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag(monsterTag);

        GameObject waypointsParent = new GameObject("WayPoints");
        allItems.Add(waypointsParent);
        waypointsParent.tag = "WayPoints";

        foreach (GameObject monster in monsters) {
            if (waypointPrefab != null) {
                GameObject waypoint = Instantiate(waypointPrefab, 
                monster.transform.position,
                Quaternion.identity);

                waypoint.transform.SetParent(waypointsParent.transform);

                allItems.Add(waypoint);

                Renderer renderer = waypoint.GetComponent<Renderer>();
                if (renderer != null) {
                    renderer.enabled = false;
                } else {
                    MeshFilter meshfilter = waypoint.GetComponent<MeshFilter>();
                    if (meshfilter != null) {
                        Destroy(meshfilter);
                    }
                }
            } else {
                Debug.LogError("Waypoint prefab is not assigned!");
            }
        }
    }

    private void SpawnMonsters() {
        monsterList.Add(Spider);

        numMonsterAreas = Random.Range(2, 8);

        // how many collectable item areas total of this level
        for (int i = 0; i < numMonsterAreas; i++) {
            // int randCollectableIndex = Random.Range(0, collectableList.Count - 1);
            int index = Random.Range(0, floorPositions.Count - 1);

            Vector3 newPos = new Vector3 (
                floorPositions[index].x - 1.5f,
                floorPositions[index].y,
                floorPositions[index].z - 1.5f
            );

            GameObject monster = Instantiate(monsterList[0],
            newPos,
            Quaternion.identity);
            allItems.Add(monster);

            monster.transform.SetParent(this.transform);

            floorPositions.RemoveAt(index);
        }
    }

    // private void SpawnBossMonster() {
    //}

    private void SpawnNextSceneRock() {
        nextSceneRockPosition = new Vector3 (
            nextSceneRockPosition.x,
            nextSceneRockPosition.y + 1,
            nextSceneRockPosition.z + 1
        );

        GameObject nextAreaRock = Instantiate(toNextArea,
        nextSceneRockPosition,
        Quaternion.identity);
        allItems.Add(nextAreaRock);

        nextAreaRock.transform.SetParent(this.transform);

        SphereCollider collider = nextAreaRock.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.isTrigger = true;
        nextAreaRock.AddComponent<GoingToNextForestLv>();
    }

    private void SpawnObjects() {
        for (int c = 0; c < borderPositions.Count; c++) {
            GameObject borderTree = Instantiate(treeBorderSpawn,
            SpawnBorderTrees(c),
            Quaternion.identity);
            allItems.Add(borderTree);
            borderTree.transform.SetParent(this.transform);
        }

        for(int c = 0; c < 10; c++) {
            GameObject toPlaceTree1 = Instantiate(tree1Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceTree1);
            toPlaceTree1.transform.SetParent(this.transform);

            GameObject toPlaceTree2 = Instantiate(tree2Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceTree2);
            toPlaceTree2.transform.SetParent(this.transform);
        }

        for(int c = 0; c < 1; c++) {
            GameObject toPlaceRock1 = Instantiate(rock1Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceRock1);
            toPlaceRock1.transform.SetParent(this.transform);

            GameObject toPlaceRock2 = Instantiate(rock2Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceRock2);
            toPlaceRock2.transform.SetParent(this.transform);

            GameObject toPlaceLog = Instantiate(logSpawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            toPlaceLog.transform.Rotate(0, Random.Range(0, 359), 0);
            allItems.Add(toPlaceLog);
            toPlaceLog.transform.SetParent(this.transform);
        }

        for(int c = 0; c < 200; c++) {
            GameObject toPlaceGrass = Instantiate(grassSpawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceGrass);
            toPlaceGrass.transform.SetParent(this.transform);
        }

    }

    private Vector3 SpawnBorderTrees(int i) {
        Vector3 newPos = new Vector3 (
            borderPositions[i].x - 1.5f,
            borderPositions[i].y,
            borderPositions[i].z - 1.5f
        );

        return newPos;
    }

    private Vector3 SpawnGrass() {
        int rndIndex = Random.Range(0, grassPositions.Count);
        int rndSquareX = Random.Range(0, 3);
        int rndSquareZ = Random.Range(0, 3);

        Vector3 newPos = new Vector3 (
            grassPositions[rndIndex].x - rndSquareX,
            grassPositions[rndIndex].y,
            grassPositions[rndIndex].z - rndSquareZ
        );

        grassPositions.RemoveAt(rndIndex);
        return newPos;
    }

    // randomize the object spawn within the 3x3 grid tile
    private Vector3 ObjectSpawnLocation() {
        int rndIndex = Random.Range(0, floorPositions.Count);
        int rndSquareX = Random.Range(0, 3);
        int rndSquareZ = Random.Range(0, 3);

        Vector3 newPos = new Vector3 (
            floorPositions[rndIndex].x - rndSquareX,
            floorPositions[rndIndex].y,
            floorPositions[rndIndex].z - rndSquareZ
        );

        floorPositions.RemoveAt(rndIndex);
        return newPos;
    }

}
