using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MiniMap : MonoBehaviour
{
    public GameObject player;
    private List<GameObject> monsters;
    private List<GameObject> collectables;
    private GameObject nextScene;
    private GameObject nextSceneIconInstance; // To hold the instance of the next scene icon

    public GameObject enemyIconPrefab;
    public GameObject collectableIconPrefab;
    public GameObject nextSceneIconPrefab;

    public float mapScale = 1f;

    private RectTransform miniMapRectTransform;

    private bool isLevelRebuilding = false;
    private float cleanupDelayTimer = 0f;
    private float cleanupDelayDuration = 0.2f;

    void Start()
    {
        Debug.Log("MINI -- in start");
        FindTaggedObjects();
        SpawnIcons();
    }

    void FindTaggedObjects()
    {
        Debug.Log("MINI -- finding tagged objects");
        monsters = GameObject.FindGameObjectsWithTag("Monster").ToList();
        collectables = GameObject.FindGameObjectsWithTag("Collectable").ToList();
        nextScene = GameObject.FindGameObjectWithTag("NextScene");
        Debug.Log("MINI -- Next scene object found: " + nextScene);
    }

    void SpawnIcons()
    {
        Debug.Log("MINI -- spawning icons");
        if (miniMapRectTransform == null)
        {
            miniMapRectTransform = GetComponent<RectTransform>();
        }
        SpawnMonsterIcons();
        SpawnCollectableIcons();
        SpawnNextSceneIcon();
        Debug.Log("MINI -- finished spawning the icons!!");
    }

    void SpawnMonsterIcons()
    {
        if (enemyIconPrefab == null)
        {
            Debug.LogError("Enemy Icon Prefab is not assigned in the Inspector!");
            return;
        }

        foreach (GameObject monster in monsters)
        {
            if (monster != null) // Check if the monster still exists
            {
                GameObject iconGO = Instantiate(enemyIconPrefab, transform); // Instantiate as a child of the minimap
                Image iconImage = iconGO.GetComponent<Image>();
                MiniMapIcon iconScript = iconGO.GetComponent<MiniMapIcon>(); // Get the MiniMapIcon script

                if (iconImage != null && iconScript != null)
                {
                    // Store a reference to the monster's transform on the icon script
                    iconScript.SetTarget(monster.transform);
                    UpdateIconPosition(iconImage.rectTransform, monster.transform.position); // Use the generic update function
                }
                else
                {
                    LogErrorForIconSetup(iconGO, iconImage, iconScript, "monster");
                }
            }
        }
    }

    void SpawnCollectableIcons()
    {
        if (collectableIconPrefab == null)
        {
            Debug.LogError("Collectable Icon Prefab is not assigned in the Inspector!");
            return;
        }

        foreach (GameObject collectable in collectables)
        {
            if (collectable != null)
            {
                GameObject iconGO = Instantiate(collectableIconPrefab, transform);
                Image iconImage = iconGO.GetComponent<Image>();
                MiniMapIcon iconScript = iconGO.GetComponent<MiniMapIcon>(); // You can reuse the same script

                if (iconImage != null && iconScript != null)
                {
                    iconScript.SetTarget(collectable.transform);
                    UpdateIconPosition(iconImage.rectTransform, collectable.transform.position); // Reuse the generic update function
                }
                else
                {
                    LogErrorForIconSetup(iconGO, iconImage, iconScript, "collectable");
                }
            }
        }
    }

    void SpawnNextSceneIcon()
    {
        if (nextSceneIconPrefab == null)
        {
            Debug.LogError("Next Scene Icon Prefab is not assigned in the Inspector!");
            return;
        }

        if (nextScene != null)
        {
            Debug.Log("MINI -- creating new next scene icon for this level");
            GameObject iconGO = Instantiate(nextSceneIconPrefab, transform);
            Debug.Log(iconGO);
            Image iconImage = iconGO.GetComponent<Image>();
            Debug.Log(iconImage);
            MiniMapIcon iconScript = iconGO.GetComponent<MiniMapIcon>();
            Debug.Log(iconScript);

            if (iconImage != null && iconScript != null)
            {
                Debug.Log("MINI -- Setting the next scene icon instance");
                iconScript.SetTarget(nextScene.transform);
                UpdateIconPosition(iconImage.rectTransform, nextScene.transform.position);
                nextSceneIconInstance = iconGO; // Store the instance for potential later use
            }
            else
            {
                LogErrorForIconSetup(iconGO, iconImage, iconScript, "next scene");
            }
        }
        else
        {
            Debug.LogWarning("GameObject with 'NextScene' tag not found. Next scene icon will not be spawned.");
        }
    }

    private void LogErrorForIconSetup(GameObject iconGO, Image iconImage, MiniMapIcon iconScript, string objectType)
    {
        if (iconImage == null)
        {
            Debug.LogError($"Instantiated {objectType} icon does not have an Image component!");
        }
        if (iconScript == null)
        {
            Debug.LogError($"Instantiated {objectType} icon does not have a MiniMapIcon component!");
        }
        Destroy(iconGO);
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the position of each icon
        foreach (Transform child in transform)
        {
            MiniMapIcon iconScript = child.GetComponent<MiniMapIcon>();
            if (iconScript != null && iconScript.target != null)
            {
                UpdateIconPosition(child.GetComponent<RectTransform>(), iconScript.target.position);
            }
        }

        if (isLevelRebuilding) {
            cleanupDelayTimer += Time.deltaTime;
            if (cleanupDelayTimer >= cleanupDelayDuration) {
                isLevelRebuilding = false;
                cleanupDelayTimer = 0f;
            }
        } else {
            // Clean up destroyed monsters and their icons
            int monstersRemoved = monsters.RemoveAll(monster => monster == null);
            if (monstersRemoved > 0)
            {
                Debug.Log("MINI -- cleaning up monster icons!");
                CleanUpIcons(monsters);
            }

            // Clean up destroyed collectables and their icons
            int collectablesRemoved = collectables.RemoveAll(collectable => collectable == null);
            if (collectablesRemoved > 0)
            {
                Debug.Log("MINI -- cleaning up collectable icons!");
                CleanUpIcons(collectables);
            }

            // Check and handle the Next Scene icon
            // if (nextScene == null && nextSceneIconInstance != null)
            // {
            //     // The Next Scene object has been destroyed, so destroy its icon
            //     Debug.Log("MINI -- cleaning up next scene icon since its target was destroyed!");
            //     Destroy(nextSceneIconInstance);
            // }
            // else if (nextScene != null && nextSceneIconInstance == null)
            // {
            //     // The Next Scene object exists, but its icon hasn't been created yet, so spawn it
            //     SpawnNextSceneIcon();
            // }
            // If nextScene is not null and nextSceneIconInstance is not null, the icon should already exist and be updated.
        }
    }

    // This function will destroy all monster and collectable tagged items that are children
    // and then call the Start function to reinstate them.
    public void ResetMiniMap()
    {
        Debug.Log("MINI -- resetting the minimap");
        // Destroy all monster icons
        foreach (Transform child in transform)
        {
            MiniMapIcon iconScript = child.GetComponent<MiniMapIcon>();
            if (iconScript != null && iconScript.target != null)
            {
                if (iconScript.target.CompareTag("Monster") || iconScript.target.CompareTag("Collectable") || iconScript.target.CompareTag("NextScene"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
        Debug.Log("MINI -- clearing the lists and then calling start");
        // Clear the lists of tracked objects
        monsters.Clear();
        collectables.Clear();
        nextScene = null;
        nextSceneIconInstance = null;

        // Re-initialize by calling Start
        isLevelRebuilding = true;
        Start();
    }

    public void CleanUpIcons(List<GameObject> trackedObjects)
    {
        Debug.Log("MINI -- inside cleaning up icons...");
        List<Transform> iconsToRemove = new List<Transform>();
        foreach (Transform child in transform)
        {
            MiniMapIcon iconScript = child.GetComponent<MiniMapIcon>();
            if (iconScript != null && iconScript.target == null)
            {
                iconsToRemove.Add(child);
            }
        }

        foreach (Transform icon in iconsToRemove)
        {
            Debug.Log(icon);
            Destroy(icon.gameObject);
        }
    }

    void UpdateIconPosition(RectTransform iconRectTransform, Vector3 targetPosition)
    {
        // Calculate the position relative to the player
        Vector3 playerPosition = player.transform.position;
        Vector3 relativePosition = targetPosition - playerPosition;

        // Scale the relative position
        float scaledX = relativePosition.x / mapScale;
        float scaledZ = relativePosition.z / mapScale;

        // Set the local position of the icon
        iconRectTransform.localPosition = new Vector3(scaledX, scaledZ, 0);

        // Ensure the icon stays within the bounds of the minimap
        if (miniMapRectTransform != null)
        {
            Vector3 clampedPosition = iconRectTransform.localPosition;
            float halfWidth = miniMapRectTransform.rect.width / 2f;
            float halfHeight = miniMapRectTransform.rect.height / 2f;

            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -halfWidth, halfWidth);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -halfHeight, halfHeight);

            iconRectTransform.localPosition = clampedPosition;
        }
    }
}