using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] GameObject statusBox;
    [SerializeField] TMP_Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ManaBar manaBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color badpsnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color swmColor;
    [SerializeField] Color parColor;
    [SerializeField] Color slpColor;

    Player _player;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Player player)
    {
        _player = player;

        nameText.text = player.Base.Name;
        levelText.text = "Lv " + player.Level;
        hpBar.SetHP((float) player.HP / player.MaxHp);
        manaBar.SetMana((float) player.MANA / player.MaxMana);

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
        _player.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_player.Status == null)
        {
            statusText.text = "";
            statusBox.SetActive(false);
        }
        else
        {
            statusText.text = _player.Status.Id.ToString().ToUpper();
            Debug.Log(statusText.text);
            if (statusText.text == "BADPSN")
                statusText.text = "PSN";
            if (statusText.text == "BURN")
                statusText.text = "BRN";
            statusBox.SetActive(true);
            statusBox.GetComponent<Image>().color = statusColors[_player.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if(_player.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float) _player.HP / _player.MaxHp);
            _player.HpChanged = false;
        }
    }

    public IEnumerator UpdateMana()
    {
        yield return manaBar.SetManaSmooth((float) _player.MANA / _player.MaxMana);
    }
}
