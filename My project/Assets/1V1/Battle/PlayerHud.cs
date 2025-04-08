using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ManaBar manaBar;

    Player _player;

    public void SetData(Player player)
    {
        _player = player;

        nameText.text = player.Base.Name;
        levelText.text = "Lv " + player.Level;
        hpBar.SetHP((float) player.HP / player.MaxHp);
        manaBar.SetMana((float) player.MANA / player.MaxMana);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float) _player.HP / _player.MaxHp);
    }

    public IEnumerator UpdateMana()
    {
        yield return manaBar.SetManaSmooth((float) _player.MANA / _player.MaxMana);
    }
}
