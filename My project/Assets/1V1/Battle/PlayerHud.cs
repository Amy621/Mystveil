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
    [SerializeField] GameObject expBar;

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
        SetLevel();
        hpBar.SetHP((float) player.HP / player.MaxHp);
        manaBar.SetMana((float) player.MANA / player.MaxMana);
        SetExp();

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

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        float currentScaleX = expBar.transform.localScale.x;

        float timeElapsed = 0f;
        float velocity = 0.0f;

        while (timeElapsed < 1.5f)
        {
            timeElapsed += Time.deltaTime;
            float newScaleX = Mathf.SmoothDamp(currentScaleX, normalizedExp, ref velocity, 1.5f);
            expBar.transform.localScale = new Vector3(newScaleX, expBar.transform.localScale.y, expBar.transform.localScale.z);
            yield return null;
        }

        expBar.transform.localScale = new Vector3(normalizedExp, expBar.transform.localScale.y, expBar.transform.localScale.z);
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _player.Base.GetExpForLevel(_player.Level);
        int nextLevelExp = _player.Base.GetExpForLevel(_player.Level + 1);
        Debug.Log("current level exp: " + currLevelExp);
        Debug.Log("Next level exp: : " + nextLevelExp);
        Debug.Log("current player exp: " + _player.Exp);

        float normalizedExp = (float)(_player.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _player.Level;
    }
}
