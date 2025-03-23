using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
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

    // Creating 1v1 boss battle enemy
    // TO-DO: make a list and randomly choose one boss monster
    public GameObject bossMonster;
    private int bossLevelSizeX = 25;
    private int bossLevelSizeZ = 25;

    // Creating pockets of collectables and monsters to spawn
    // TO-DO: make a list of monsters and collectables to randomize
    public GameObject monsterSpider;
    public GameObject collectableHerb;
    private int numMonsterAreas;
    private int numCollectableAreas;

    // Creating objects to spawn (non-collectable)
    public GameObject grassSpawn;
    public GameObject flower1Spawn;
    public GameObject flower2Spawn;
    public GameObject flower3Spawn;
    public GameObject flower4Spawn;
    public GameObject flower5Spawn;
    public GameObject rock1Spawn;
    public GameObject rock2Spawn;
    public GameObject tree1Spawn;
    public GameObject tree2Spawn;
    public GameObject treeBorderSpawn;
    public GameObject mushroomSpawn;
    public GameObject logSpawn;
    public GameObject toNextArea;
    private List<GameObject> allItems = new List<GameObject>();

    public List<GameObject> getAllObjects() {
        return allItems;
    }

    public void GenerateLevel() {
        Debug.Log("Inside generate level");
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
            // make the level area
            else {
                for(int z = 0; z < worldSizeZ; z++) {
                    Vector3 pos = new Vector3(x * gridOffset,
                    0,
                    z * gridOffset);

                    // making the borders less straight line and more random
                    int offsetRandom = Random.Range(0, 2);

                    // adding in the next scene rock
                    // if (x == 25 && z == 15) {
                    //     nextSceneRockPosition = pos;
                    // }

                    // temp position to make sure it works
                    if (x == 6 && z == 17) {
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

        // spawn the obj that will be used to go to the next level / scene
        SpawnNextSceneRock();

        // spawning the objects (non-interactable)
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
                // if (x == 25 && z == 15) {
                //     nextSceneRockPosition = pos;
                // }

                // temp position to make sure it works
                if (x == 6 && z == 9) {
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
        }

        SpawnNextSceneRock();
        
    }

    // private void SpawnCollectables() {

    // }

    // private void SpawnMonsters() {

    // }

    private void SpawnNextSceneRock() {
        GameObject nextAreaRock = Instantiate(toNextArea,
        nextSceneRockPosition,
        Quaternion.identity);
        allItems.Add(nextAreaRock);

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
        }

        for(int c = 0; c < 10; c++) {
            GameObject toPlaceFlower1 = Instantiate(flower1Spawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceFlower1);

            GameObject toPlaceFlower2 = Instantiate(flower2Spawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceFlower2);

            GameObject toPlaceFlower3 = Instantiate(flower3Spawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceFlower3);

            GameObject toPlaceFlower4 = Instantiate(flower4Spawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceFlower4);

            GameObject toPlaceFlower5 = Instantiate(flower5Spawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceFlower5);

            GameObject toPlaceTree1 = Instantiate(tree1Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceTree1);

            GameObject toPlaceTree2 = Instantiate(tree2Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceTree2);
        }

        for(int c = 0; c < 1; c++) {
            GameObject toPlaceRock1 = Instantiate(rock1Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceRock1);

            GameObject toPlaceRock2 = Instantiate(rock2Spawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceRock2);

            GameObject toPlaceMushroom = Instantiate(mushroomSpawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            allItems.Add(toPlaceMushroom);

            GameObject toPlaceLog = Instantiate(logSpawn,
            ObjectSpawnLocation(),
            Quaternion.identity);
            toPlaceLog.transform.Rotate(0, Random.Range(0, 359), 0);
            allItems.Add(toPlaceLog);
        }

        for(int c = 0; c < 200; c++) {
            GameObject toPlaceGrass = Instantiate(grassSpawn,
            SpawnGrass(),
            Quaternion.identity);
            allItems.Add(toPlaceGrass);
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
