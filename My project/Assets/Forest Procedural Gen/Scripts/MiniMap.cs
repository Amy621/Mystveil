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

    void Start()
    {
        monsters = GameObject.FindGameObjectsWithTag("Monster").ToList();
        collectables = GameObject.FindGameObjectsWithTag("Collectable").ToList();
        nextScene = GameObject.FindGameObjectWithTag("NextScene");
        miniMapRectTransform = GetComponent<RectTransform>();
        SpawnMonsterIcons();
        SpawnCollectableIcons();
        SpawnNextSceneIcon(); // Call the new spawn function
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
            GameObject iconGO = Instantiate(nextSceneIconPrefab, transform);
            Image iconImage = iconGO.GetComponent<Image>();
            MiniMapIcon iconScript = iconGO.GetComponent<MiniMapIcon>();

            if (iconImage != null && iconScript != null)
            {
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

        // Clean up destroyed monsters and their icons
        int monstersRemoved = monsters.RemoveAll(monster => monster == null);
        if (monstersRemoved > 0)
        {
            CleanUpIcons(monsters);
        }

        // Clean up destroyed collectables and their icons
        int collectablesRemoved = collectables.RemoveAll(collectable => collectable == null);
        if (collectablesRemoved > 0)
        {
            CleanUpIcons(collectables);
        }

        // Check if the next scene object was destroyed
        if (nextScene != null && nextSceneIconInstance != null && nextScene == null)
        {
            Destroy(nextSceneIconInstance);
            nextSceneIconInstance = null;
        }
        else if (nextScene != null && nextSceneIconInstance == null)
        {
            SpawnNextSceneIcon(); // Re-spawn if it was destroyed and the target exists
        }
    }

    public void CleanUpIcons(List<GameObject> trackedObjects)
    {
        List<Transform> iconsToRemove = new List<Transform>();
        foreach (Transform child in transform)
        {
            MiniMapIcon iconScript = child.GetComponent<MiniMapIcon>();
            if (iconScript != null && (iconScript.target == null || !trackedObjects.Contains(iconScript.target.gameObject)))
            {
                iconsToRemove.Add(child);
            }
        }

        foreach (Transform icon in iconsToRemove)
        {
            Destroy(icon.gameObject);
        }

        // Optionally, clear the lists here if the scene is changing completely
        if (trackedObjects == monsters)
        {
            monsters.Clear();
        }
        else if (trackedObjects == collectables)
        {
            collectables.Clear();
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