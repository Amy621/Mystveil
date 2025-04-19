using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonoBehaviour
{
    [SerializeField] public EnemyBase Base { get; set; }
    [SerializeField] public int Level { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Base.Base.Name);
        Debug.Log(Level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
