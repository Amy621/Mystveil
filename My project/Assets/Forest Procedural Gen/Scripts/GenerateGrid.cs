using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;

public class GenerateGrid : MonoBehaviour
{
    [Header("Player Settings")]
    private Vector3 playerSpawn;
    public GameObject player;
    public PlayerStats playerObject;
    public PlayerDB playerDB;

    [Header("Floor/Ground Settings")]
    public GameObject grassFloorObject;
    public GameObject spawnFloorObject;
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

    [Header("All 1v1 Enemy Prefabs")]
    public GameObject GuardianOfTheWell;
    public GameObject LunarFenrir;
    public GameObject RootboundTyrant;
    public GameObject ShadowMirror;
    private List<GameObject> bossEnemyList = new List<GameObject>();
    private Vector3 bossPosition;
    private int bossLevelSizeX = 25;
    private int bossLevelSizeZ = 25;

    public string bossMonsterTag = "BossMonster";

    [Header("All Overworld Enemy Prefabs")]
    public GameObject CottonTail;
    public GameObject Webweaver;
    public GameObject AsterEye;
    public GameObject Dionaeant;
    public GameObject Stranterry;
    public GameObject Bantboo;
    public GameObject Pollant;
    public GameObject Briarheart;
    public GameObject Odosapling;
    public GameObject Vilebloom;
    private List<GameObject> monsterList = new List<GameObject>();
    public GameObject waypointPrefab;
    public string monsterTag = "Monster";

    private List<GameObject> allMonstersList = new List<GameObject>();

    [Header("All Monster Scriptable Objects")]
    public EnemyStats GuardianOfTheWellObject;
    public EnemyStats LunarFenrirObject;
    public EnemyStats RootboundTyrantObject;
    public EnemyStats CottonTailObject;
    public EnemyStats WebweaverObject;
    public EnemyStats AsterEyeObject;
    public EnemyStats DionaeantObject;
    public EnemyStats StranterryObject;
    public EnemyStats BantbooObject;
    public EnemyStats PollantObject;
    public EnemyStats BriarheartObject;
    public EnemyStats OdosaplingObject;
    public EnemyStats VilebloomObject;

    private List<EnemyStats> monsterScriptableObjects = new List<EnemyStats>();

    [Header("All Collectable Prefabs")]
    public GameObject DragonsBreath;
    public GameObject DreamLeaf;
    public GameObject EmberLeaf;
    public GameObject FairyFlax;
    public GameObject GrimalkinsClaw;
    public GameObject HolySanctum;
    public GameObject MoonBloom;
    public GameObject MoonpetalBlossom;
    public GameObject NightWhisper;
    public GameObject NightshadeBloom;
    public GameObject RainbowRoot;
    public GameObject RavenClawRoot;
    public GameObject Shadowberry;
    public GameObject SilverSeed;
    public GameObject SparkleSprig;
    public GameObject StarlightBerry;
    public GameObject SunstoneSeed;
    public GameObject Veilwort;
    public GameObject WhimsyWillow;
    public GameObject Whisperwood;

    private List<GameObject> commonCollectables = new List<GameObject>();
    private List<GameObject> rareCollectables = new List<GameObject>();
    private List<GameObject> ultraRareCollectables = new List<GameObject>();
    
    private List<GameObject> collectableList = new List<GameObject>();
    public string collectableTag = "Collectable";

    private int numMonsterAreas;
    private int numCollectableAreas;

    [Header("All Item Scriptable Objects")]
    public Item DragonsBreathObject;
    public Item DreamLeafObject;
    public Item EmberLeafObject;
    public Item FairyFlaxObject;
    public Item GrimalkinsClawObject;
    public Item HolySanctumObject;
    public Item MoonBloomObject;
    public Item MoonpetalBlossomObject;
    public Item NightWhisperObject;
    public Item NightshadeBloomObject;
    public Item RainbowRootObject;
    public Item RavenClawRootObject;
    public Item ShadowberryObject;
    public Item SilverSeedObject;
    public Item SparkleSprigObject;
    public Item StarlightBerryObject;
    public Item SunstoneSeedObject;
    public Item VeilwortObject;
    public Item WhimsyWillowObject;
    public Item WhisperwoodObject;

    [Header("All Non-collectable Item Prefabs")]
    public GameObject grassSpawn;
    public GameObject rock1Spawn;
    public GameObject rock2Spawn;
    public GameObject tree1Spawn;
    public GameObject tree2Spawn;
    public GameObject treeBorderSpawn;
    public GameObject logSpawn;
    public GameObject toNextArea;
    private List<GameObject> allItems = new List<GameObject>();

    private Dictionary<GameObject, EnemyBase> monsterDictionary = new Dictionary<GameObject, EnemyBase>();
    private Dictionary<GameObject, Item> collectableDictionary = new Dictionary<GameObject, Item>();

    public List<GameObject> getAllObjects() {
        return allItems;
    }

    public Vector3 getPlayerSpawnCoords() {
        return playerSpawn;
    }

    void Update()
    {
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float zMin = float.MaxValue;
        float zMax = float.MinValue;

        foreach (Vector3 border in borderPositions)
        {
            xMin = Mathf.Min(xMin, border.x);
            xMax = Mathf.Max(xMax, border.x);
            zMin = Mathf.Min(zMin, border.z);
            zMax = Mathf.Max(zMax, border.z);
        }

        if (player.transform.position.x > xMax)
        {
            player.transform.position = new Vector3(xMax, player.transform.position.y, player.transform.position.z);
            Debug.Log("out of range in xMax based on borderPositions");
        }
        if (player.transform.position.x < xMin)
        {
            player.transform.position = new Vector3(xMin, player.transform.position.y, player.transform.position.z);
            Debug.Log("out of range in xMin based on borderPositions");
        }
        if (player.transform.position.z > zMax)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zMax);
            Debug.Log("out of range in zMax based on borderPositions");
        }
        if (player.transform.position.z < zMin)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zMin);
            Debug.Log("out of range in zMin based on borderPositions");
        }
    }

    public void Destroy() {
        for(int i = 0; i < allItems.Count; i++) {
            Destroy(allItems[i]);
        }
        Debug.Log("now in clearing functions!");
        allItems.Clear();

        floorPositions.Clear();
        borderPositions.Clear();
        grassPositions.Clear();
    }

    public void GenerateLevel(bool isFirstLevel) {
        Debug.Log("in generate level!");

        if(!isFirstLevel) {
            Destroy();
        } else {
            // Find PlayerDB
            playerDB = FindObjectOfType<PlayerDB>();

            // Assign the dictionaries
            monsterList.Add(CottonTail);
            monsterList.Add(Webweaver);
            monsterList.Add(AsterEye);
            monsterList.Add(Dionaeant);
            monsterList.Add(Stranterry);
            monsterList.Add(Bantboo);
            monsterList.Add(Pollant);
            monsterList.Add(Briarheart);
            monsterList.Add(Odosapling);
            monsterList.Add(Vilebloom);

            bossEnemyList.Add(GuardianOfTheWell);
            bossEnemyList.Add(RootboundTyrant);
            bossEnemyList.Add(LunarFenrir);

            //bossEnemyList.Add(ShadowMirror);
            // make a different script for the shadow mirror

            // must be the same order!!
            allMonstersList.Add(GuardianOfTheWell);
            allMonstersList.Add(LunarFenrir);
            allMonstersList.Add(RootboundTyrant);
            allMonstersList.Add(CottonTail);
            allMonstersList.Add(Webweaver);
            allMonstersList.Add(AsterEye);
            allMonstersList.Add(Dionaeant);
            allMonstersList.Add(Stranterry);
            allMonstersList.Add(Bantboo);
            allMonstersList.Add(Pollant);
            allMonstersList.Add(Briarheart);
            allMonstersList.Add(Odosapling);
            allMonstersList.Add(Vilebloom);

            monsterScriptableObjects.Add(GuardianOfTheWellObject);
            monsterScriptableObjects.Add(LunarFenrirObject);
            monsterScriptableObjects.Add(RootboundTyrantObject);
            monsterScriptableObjects.Add(CottonTailObject);
            monsterScriptableObjects.Add(WebweaverObject);
            monsterScriptableObjects.Add(AsterEyeObject);
            monsterScriptableObjects.Add(DionaeantObject);
            monsterScriptableObjects.Add(StranterryObject);
            monsterScriptableObjects.Add(BantbooObject);
            monsterScriptableObjects.Add(PollantObject);
            monsterScriptableObjects.Add(BriarheartObject);
            monsterScriptableObjects.Add(OdosaplingObject);
            monsterScriptableObjects.Add(VilebloomObject);

            int index = 0;
            foreach(EnemyStats monster in monsterScriptableObjects)
            {
                int randomLevel = Random.Range(1, 11);
                randomLevel = playerDB.Player.Level - randomLevel;

                if (randomLevel <= 0)
                    randomLevel = 1;

                EnemyBase monsterToAdd = new EnemyBase(monster, randomLevel);
                monsterDictionary.Add(allMonstersList[index], monsterToAdd);

                index++;
            }

            collectableDictionary = new Dictionary<GameObject, Item>()
            {
                {DragonsBreath, DragonsBreathObject},
                {DreamLeaf, DreamLeafObject},
                {EmberLeaf, EmberLeafObject},
                {FairyFlax, FairyFlaxObject},
                {GrimalkinsClaw, GrimalkinsClawObject},
                {HolySanctum, HolySanctumObject},
                {MoonBloom, MoonBloomObject},
                {MoonpetalBlossom, MoonpetalBlossomObject},
                {NightWhisper, NightWhisperObject},
                {NightshadeBloom, NightshadeBloomObject},
                {RainbowRoot, RainbowRootObject},
                {RavenClawRoot, RavenClawRootObject},
                {Shadowberry, ShadowberryObject},
                {SilverSeed, SilverSeedObject},
                {SparkleSprig, SparkleSprigObject},
                {StarlightBerry, StarlightBerryObject},
                {SunstoneSeed, SunstoneSeedObject},
                {Veilwort, VeilwortObject},
                {WhimsyWillow, WhimsyWillowObject},
                {Whisperwood, WhisperwoodObject},
            };

             // adding in the collectable objects to the list
            commonCollectables.Add(Whisperwood);
            commonCollectables.Add(SunstoneSeed);
            commonCollectables.Add(StarlightBerry);
            commonCollectables.Add(Shadowberry);
            commonCollectables.Add(MoonpetalBlossom);
            commonCollectables.Add(EmberLeaf);

            rareCollectables.Add(WhimsyWillow);
            rareCollectables.Add(Veilwort);
            rareCollectables.Add(SparkleSprig);
            rareCollectables.Add(RavenClawRoot);
            rareCollectables.Add(RainbowRoot);
            rareCollectables.Add(NightshadeBloom);
            rareCollectables.Add(GrimalkinsClaw);
            rareCollectables.Add(DreamLeaf);

            ultraRareCollectables.Add(SilverSeed);
            ultraRareCollectables.Add(NightWhisper);
            ultraRareCollectables.Add(MoonBloom);
            ultraRareCollectables.Add(HolySanctum);
            ultraRareCollectables.Add(FairyFlax);
            ultraRareCollectables.Add(DragonsBreath);
        }

        /**    MAKING THE WORLD     **/

        for(int x = 0; x < worldSizeX; x++) 
        {
            // make the border
            if (x <= 2) {
                for (int z = 0; z < worldSizeZ; z++) {
                    Vector3 pos = new Vector3(x * gridOffset,
                    0,
                    z * gridOffset);

                    GameObject nonWalkFloor = Instantiate(nonWalkFloorObject,
                    pos,
                    Quaternion.identity) as GameObject;

                    allItems.Add(nonWalkFloor);
                    borderPositions.Add(nonWalkFloor.transform.position);
                    nonWalkFloor.transform.SetParent(this.transform);
                }
            }
            // make the player spawn area
            else if (x <= 5) {
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
                        nonWalkFloor.transform.SetParent(this.transform);
                    } else {
                        // player spawn
                        if (x == 5 && z == 18) {
                            GameObject spawnFloor = Instantiate(spawnFloorObject,
                            pos,
                            Quaternion.identity) as GameObject;
                            allItems.Add(spawnFloor);
                            
                            playerSpawn = pos;

                            spawnFloor.transform.SetParent(this.transform);

                        } else {
                            GameObject floor = Instantiate(grassFloorObject,
                            pos,
                            Quaternion.identity) as GameObject;
                            allItems.Add(floor);

                            grassPositions.Add(floor.transform.position);
                            floor.transform.SetParent(this.transform);
                        }
                    }
                } 
            }
            // make the level area
            else {
                for(int z = 0; z < worldSizeZ; z++) {
                    Vector3 pos = new Vector3(x * gridOffset,
                    0,
                    z * gridOffset);

                    bool isNextToNextSceneRock = false;

                    // making the borders less straight line and more random
                    int offsetRandom = Random.Range(0, 1);

                    // adding in the next scene rock
                    if (x == 25 && z == 15) {
                        nextSceneRockPosition = pos;
                        isNextToNextSceneRock = true;
                    }

                    if ((x == 25 || x == 24) && z < 17 && z > 13) {
                        isNextToNextSceneRock = true;
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

                        // check if is front of the next scene rock, can't spawn stuff in front or on it
                        if (isNextToNextSceneRock) {
                            grassPositions.Add(floor.transform.position);
                            floor.transform.SetParent(this.transform);
                            isNextToNextSceneRock = false;
                        // ensure monsters don't spawn right next to you
                        } else if (x > 5 && x < 8) {
                            grassPositions.Add(floor.transform.position);
                            floor.transform.SetParent(this.transform);
                        } else {
                            floorPositions.Add(floor.transform.position);
                            grassPositions.Add(floor.transform.position);

                            floor.transform.SetParent(this.transform);
                        }
                    }
                }

            }
        }

        GameObject player = GameObject.Find("Player");

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = playerSpawn;
        player.GetComponent<CharacterController>().enabled = true;

        // generate navmesh
        GenerateNavMesh();

        // spawn monsters
        Debug.Log("spawning monsters...");
        SpawnMonsters();

        GenerateWaypoints();

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
        Destroy();

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

                if(z < borderLength || z > bossLevelSizeZ - borderLength || x < borderLength || x > bossLevelSizeX - borderLength) {
                    GameObject nonWalkFloor = Instantiate(nonWalkFloorObject,
                    pos,
                    Quaternion.identity) as GameObject;
                    allItems.Add(nonWalkFloor);

                    borderPositions.Add(nonWalkFloor.transform.position);

                    nonWalkFloor.transform.SetParent(this.transform);
                } else {
                    // adding in the player spawn
                    if (x == 7 && z == 10) {
                        playerSpawn = pos;
                        GameObject spawnFloor = Instantiate(spawnFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;

                        allItems.Add(spawnFloor);
                        spawnFloor.transform.SetParent(this.transform);
                    // getting boss spawn location
                    } else if (x == 13 && z == 13) {
                        GameObject floor = Instantiate(grassFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;

                        allItems.Add(floor);
                        floor.transform.SetParent(this.transform);

                        bossPosition = pos;
                    } else {
                        GameObject floor = Instantiate(grassFloorObject,
                        pos,
                        Quaternion.identity) as GameObject;

                        allItems.Add(floor);
                        floor.transform.SetParent(this.transform);
                    }
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
        GameObject player = GameObject.Find("Player");

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = playerSpawn;
        player.GetComponent<CharacterController>().enabled = true;

        SpawnBossMonster();

        SpawnNextSceneRock();
        
    }

    public void GenerateNavMesh() {
        if (navMeshSurface == null)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            Debug.Log(navMeshSurface);
            navMeshSurface.BuildNavMesh();
        }

        if (navMeshSurface != null)
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found on this GameObject!");
        }
    }

    

    private void SpawnCollectables() {

        numCollectableAreas = Random.Range(3, 7);
        
        // how many collectable item areas total of this level
        for (int i = 0; i < numCollectableAreas; i++) {

            int whichCollectable = Random.Range(1, 11);

            if (whichCollectable <= 6)
            {
                collectableList = commonCollectables;
            } else if (whichCollectable > 6 && whichCollectable < 10) {
                collectableList = rareCollectables;
            } else {
                collectableList = ultraRareCollectables;
            }

            int randCollectableIndex = Random.Range(0, collectableList.Count);
            int randNumOfCollectable = Random.Range(2, 6);
            int index = Random.Range(0, floorPositions.Count);

            // store all the coords possible
            List<string> possibleCoords = new List<string>();

            for (int m = 0; m <= 3; m++) {
                for (int n = 0; n <= 3; n++) {
                    possibleCoords.Add(m + "," + n);
                }
            }

            // how many collectables per area
            for(int j = 0; j < randNumOfCollectable; j++) {
                int randIndex = Random.Range(0, possibleCoords.Count);
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

                collectable.tag = collectableTag;

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

                SphereCollider sphereCollider = waypoint.GetComponent<SphereCollider>();
                if (sphereCollider != null) {
                    sphereCollider.enabled = false;
                    sphereCollider.isTrigger = true;
                }

                Renderer renderer = waypoint.GetComponent<Renderer>();
                if (renderer != null) {
                    renderer.enabled = false;
                    MeshFilter meshfilter = waypoint.GetComponent<MeshFilter>();
                    Destroy(meshfilter);
                } 
            } else {
                Debug.LogError("Waypoint prefab is not assigned!");
            }
        }
    }

    private void SpawnMonsters() {
        numMonsterAreas = Random.Range(2, 8);

        // how many collectable item areas total of this level
        for (int i = 0; i < numMonsterAreas; i++) {
            // int randCollectableIndex = Random.Range(0, collectableList.Count);
            int index = Random.Range(0, floorPositions.Count);

            Vector3 newPos = new Vector3 (
                floorPositions[index].x - 1.5f,
                floorPositions[index].y,
                floorPositions[index].z - 1.5f
            );

            // Random.Range(0, monsterList.Count)

            GameObject monsterPrefab = monsterList[Random.Range(0, monsterList.Count)];
            GameObject monster = Instantiate(monsterPrefab,
            newPos,
            Quaternion.identity);
            allItems.Add(monster);

            monster.tag = monsterTag;

            int layerIndex = LayerMask.NameToLayer("Enemy");

            // Check if the layer name is valid
            if (layerIndex != -1)
            {
                // Assign the layer to this GameObject
                monster.layer = layerIndex;
            }
            else
            {
                Debug.LogError($"Layer not found. Please check your Tag and Layers settings.");
            }

            Debug.Log("Passing dictionary value to Enemy cs script...");
            Debug.Log("monster stat value = " + monsterDictionary[monsterPrefab].Base);
            Debug.Log("monster level value = " + monsterDictionary[monsterPrefab].Level);
            Debug.Log("=============================================");

            Enemy enemyComponent = monster.GetComponent<Enemy>();

            enemyComponent.monster = monsterDictionary[monsterPrefab];

            monster.transform.SetParent(this.transform);

            floorPositions.RemoveAt(index);
        }
    }

    private void SpawnBossMonster() {

        // select the boss for the boss level
        int index = Random.Range(0, bossEnemyList.Count);

        Vector3 newPos = new Vector3 (
            bossPosition.x,
            bossPosition.y,
            bossPosition.z
        );

        GameObject monsterPrefab = bossEnemyList[index];
        GameObject monster = Instantiate(bossEnemyList[index],
        newPos,
        Quaternion.identity);
        allItems.Add(monster);

        monster.tag = bossMonsterTag;
        monster.AddComponent<LookAtPlayer>();
        BossMonster bossMonsterComponent = monster.AddComponent<BossMonster>();
        bossMonsterComponent.Base = monsterDictionary[monsterPrefab];

        int randLevel = Random.Range(0, 9);
        randLevel = playerDB.Player.Level - randLevel;
        if (randLevel < 1)
            randLevel = 1;
        bossMonsterComponent.Level = randLevel;


        monster.transform.SetParent(this.transform);
    }

    private void SpawnNextSceneRock() {
        nextSceneRockPosition = new Vector3 (
            nextSceneRockPosition.x,
            nextSceneRockPosition.y + 1,
            nextSceneRockPosition.z + 1
        );

        GameObject nextAreaRock = Instantiate(toNextArea,
        nextSceneRockPosition,
        Quaternion.identity);

        nextAreaRock.transform.SetParent(this.transform);

        SphereCollider collider = nextAreaRock.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.isTrigger = true;
        nextAreaRock.AddComponent<GoingToNextForestLv>();

        allItems.Add(nextAreaRock);
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
