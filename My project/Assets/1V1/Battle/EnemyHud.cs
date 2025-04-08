using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHud : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] HPBar hpBar;

    EnemyBase _enemy;

    public void SetData(EnemyBase enemy)
    {
        _enemy = enemy;

        nameText.text = enemy.Base.Name;
        levelText.text = "Lv " + enemy.Level;
        hpBar.SetHP((float) enemy.HP / enemy.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float) _enemy.HP / _enemy.MaxHp);
    }
}
