using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingToNextForestLv : MonoBehaviour
{
    public ChangeScene sceneChanger;
    public GenerateGrid gridGenerator;
    private Collider objectCollider;

    void Start() {
        objectCollider = GetComponent<Collider>();
        Debug.Log(objectCollider);
    }

    public void onTriggerEnter(Collider other) {
        if (gridGenerator.curLevel  == gridGenerator.numLevels) {
            Debug.Log("Back to town");
            sceneChanger.MoveToScene(2);
        } else if (gridGenerator.curLevel + 1 == gridGenerator.numLevels) {
            Debug.Log("Boss battle");
            gridGenerator.curLevel++;
            gridGenerator.GenerateLevel();
        } else {
            Debug.Log("Next level");
            gridGenerator.curLevel++;
            gridGenerator.GenerateLevel();
        }
    }
}
