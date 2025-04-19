using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : MonoBehaviour
{
    public EnemyBase Enemy { get; set;}

    public void Setup()
    {
        BossMonster boss = FindObjectOfType<BossMonster>();
        Enemy = boss.Base;
        GetComponent<Image>().sprite = boss.Base.Base.BossImage;
    }
}
