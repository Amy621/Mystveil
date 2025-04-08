using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField] EnemyStats _base;
    [SerializeField] int level;

    public EnemyBase Enemy { get; set;}

    public void Setup()
    {
        Enemy = new EnemyBase(_base, level);
        GetComponent<Image>().sprite = Enemy.Base.BossImage;
    }
}
