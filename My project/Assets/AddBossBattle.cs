using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBossBattle : MonoBehaviour
{

    [SerializeField] public EnemyStats enemy;
    public int level = 70;
    void Awake()
    {
        BossMonster boss = FindObjectOfType<BossMonster>();
        EnemyBase setEnemy = new EnemyBase(enemy, level);
        boss.Base = setEnemy;
        boss.Level = level;
    }

}
