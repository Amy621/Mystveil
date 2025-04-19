using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHud : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] GameObject statusBox;
    [SerializeField] TMP_Text statusText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color badpsnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color swmColor;
    [SerializeField] Color parColor;
    [SerializeField] Color slpColor;

    EnemyBase _enemy;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(EnemyBase enemy)
    {
        _enemy = enemy;

        nameText.text = enemy.Base.Name;
        levelText.text = "Lv " + enemy.Level;
        hpBar.SetHP((float) enemy.HP / enemy.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.badpsn, badpsnColor},
            {ConditionID.burn, brnColor},
            {ConditionID.swm, swmColor},
            {ConditionID.par, parColor},
            {ConditionID.slp, slpColor},
        };

        SetStatusText();
        _enemy.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_enemy.Status == null)
        {
            statusText.text = "";
            statusBox.SetActive(false);
        }
        else 
        {
            statusText.text = _enemy.Status.Id.ToString().ToUpper();
            if (statusText.text == "BADPSN")
                statusText.text = "PSN";
            if (statusText.text == "BURN")
                statusText.text = "BRN";
            statusBox.SetActive(true);
            statusBox.GetComponent<Image>().color = statusColors[_enemy.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if(_enemy.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float) _enemy.HP / _enemy.MaxHp);
            _enemy.HpChanged = false;
        }
    }
}
