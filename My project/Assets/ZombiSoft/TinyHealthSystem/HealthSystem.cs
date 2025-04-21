//==============================================================
// HealthSystem
// HealthSystem.Instance.TakeDamage (float Damage);
// HealthSystem.Instance.HealDamage (float Heal);
// HealthSystem.Instance.UseMana (float Mana);
// HealthSystem.Instance.RestoreMana (float Mana);
// Attach to the Hero.
//==============================================================

using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SearchService;
using Unity.VisualScripting;
using System;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem Instance;

	public Image currentHealthBar;
	public Image currentHealthGlobe;
	public Text healthText;

	// keeping track of how much HP the player has
	public float hitPoint;
	// max HP
	private float maxHitPoint;

	public Image currentManaBar;
	public Image currentManaGlobe;
	public Text manaText;

	// keeping track of how much MP the player has
	public float manaPoint;
	// max MP
	private float maxManaPoint;

	public Player player { get; private set; }

	//==============================================================
	// Regenerate Health & Mana
	//==============================================================
	public bool Regenerate = true;
	public float regen = 0.1f;
	private float timeleft = 0.0f;	// Left time for current interval
	public float regenUpdateInterval = 1f;

	public bool GodMode;

	//==============================================================
	// Awake
	//==============================================================
	void Awake()
	{
		Instance = this;
	}
	
	//==============================================================
	// Awake
	//==============================================================
	void Start()
	{
		PlayerDB playerDB = FindObjectOfType<PlayerDB>();

		if (playerDB != null)
		{
			player = playerDB.Player;

			hitPoint = player.HP;
			maxHitPoint = player.MaxHp;

			manaPoint = player.MANA;
			maxManaPoint = player.MaxMana;
		}

		UpdateGraphics();
		timeleft = regenUpdateInterval; 
	}

	public void UpdateHealthOutsideOneVOne()
	{
		PlayerDB playerDB = FindObjectOfType<PlayerDB>();

		if (playerDB != null)
		{
			player = playerDB.Player;

			hitPoint = player.HP;
			maxHitPoint = player.MaxHp;

			manaPoint = player.MANA;
			maxManaPoint = player.MaxMana;
		}

		UpdateGraphics();
		timeleft = regenUpdateInterval; 
	}

	//==============================================================
	// Update
	//==============================================================
	void Update ()
	{
		if (Regenerate)
			Regen();
	}

	//==============================================================
	// Regenerate Health & Mana
	//==============================================================
	private void Regen()
	{
		timeleft -= Time.deltaTime;

		if (timeleft <= 0.0) // Interval ended - update health & mana and start new interval
		{
			// Debug mode
			if (GodMode)
			{
				HealDamage(maxHitPoint);
				RestoreMana(maxManaPoint);
			}
			else
			{
				HealDamage(regen);
				RestoreMana(regen);				
			}

			UpdateGraphics();

			timeleft = regenUpdateInterval;
		}
	}

	//==============================================================
	// Health Logic
	//==============================================================
	private void UpdateHealthBar()
	{
		float ratio = hitPoint / maxHitPoint;
		currentHealthBar.rectTransform.localPosition = new Vector3(currentHealthBar.rectTransform.rect.width * ratio - currentHealthBar.rectTransform.rect.width, 0, 0);
		healthText.text = hitPoint.ToString ("0") + "/" + maxHitPoint.ToString ("0");
	}

	private void UpdateHealthGlobe()
	{
		float ratio = hitPoint / maxHitPoint;
		currentHealthGlobe.rectTransform.localPosition = new Vector3(0, currentHealthGlobe.rectTransform.rect.height * ratio - currentHealthGlobe.rectTransform.rect.height, 0);
		healthText.text = hitPoint.ToString("0") + "/" + maxHitPoint.ToString("0");
	}

	public void TakeDamage(EnemyBase monster, MonsterMove move)
	{
		Debug.Log("Player taking damage!");

		// adding in formula for damage calculation of monster
		float attack = (move.Base.Category == MoveCategory.Special)? player.SpAttack : player.Attack;
        float defense = (move.Base.Category == MoveCategory.Special)? monster.SpDefense : monster.Defense;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
        float a = (2 * player.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        Debug.Log("Player took: " + damage + " damage");

		hitPoint -= damage;
		if (hitPoint < 1)
			hitPoint = 0;

		UpdateGraphics();

		// change it in the DB too for 1v1 to reflect
		player.HP -= Mathf.FloorToInt(damage);
		if (player.HP < 1)
			player.HP = 0;

		StartCoroutine(PlayerHurts());
	}

	public void HealDamage(float Heal)
	{
		hitPoint += Heal;
		if (hitPoint > maxHitPoint) 
			hitPoint = maxHitPoint;

		player.HP += Mathf.FloorToInt(Heal);
		if (player.HP > player.MaxHp)
			player.HP = player.MaxHp;

		UpdateGraphics();
	}
	public void SetMaxHealth(float max)
	{
		maxHitPoint += (int)(maxHitPoint * max / 100);

		UpdateGraphics();
	}

	//==============================================================
	// Mana Logic
	//==============================================================
	private void UpdateManaBar()
	{
		float ratio = manaPoint / maxManaPoint;
		currentManaBar.rectTransform.localPosition = new Vector3(currentManaBar.rectTransform.rect.width * ratio - currentManaBar.rectTransform.rect.width, 0, 0);
		manaText.text = manaPoint.ToString ("0") + "/" + maxManaPoint.ToString ("0");
	}

	private void UpdateManaGlobe()
	{
		float ratio = manaPoint / maxManaPoint;
		currentManaGlobe.rectTransform.localPosition = new Vector3(0, currentManaGlobe.rectTransform.rect.height * ratio - currentManaGlobe.rectTransform.rect.height, 0);
		manaText.text = manaPoint.ToString("0") + "/" + maxManaPoint.ToString("0");
	}

	public void UseMana(float Mana)
	{
		manaPoint -= Mana;
		if (manaPoint < 1) // Mana is Zero!!
			manaPoint = 0;
		
		player.MANA -= Mathf.FloorToInt(Mana);
		if (player.MANA < 1)
			player.MANA = 0;

		UpdateGraphics();
	}

	public void RestoreMana(float Mana)
	{
		manaPoint += Mana;
		if (manaPoint > maxManaPoint) 
			manaPoint = maxManaPoint;
		
		player.MANA += Mathf.FloorToInt(Mana);
		if (player.MANA > player.MaxMana)
			player.MANA = player.MaxMana;

		UpdateGraphics();
	}
	public void SetMaxMana(float max)
	{
		maxManaPoint += (int)(maxManaPoint * max / 100);
		
		UpdateGraphics();
	}

	//==============================================================
	// Update all Bars & Globes UI graphics
	//==============================================================
	private void UpdateGraphics()
	{
		UpdateHealthBar();
		UpdateHealthGlobe();
		UpdateManaBar();
		UpdateManaGlobe();
	}

	//==============================================================
	// Coroutine Player Hurts
	//==============================================================
	IEnumerator PlayerHurts()
	{
		// Player gets hurt. Do stuff.. play anim, sound..

		if (PopupText.Instance != null)
		{
			PopupText.Instance.Popup("Ouch!", 1f, 1f); // Demo stuff!
		} // Demo stuff!

		if (hitPoint < 1) // Health is Zero!!
		{
			StartCoroutine(PlayerDied()); // Hero is Dead
		}

		else
			yield return null;
	}

	//==============================================================
	// Hero is dead
	//==============================================================
	IEnumerator PlayerDied()
	{
		{
			Debug.Log("Player died. Fading out...");
			int numItems = UnityEngine.Random.Range(1,4); //items to take on death
			Inventory.Singleton.removeItems(null, numItems);
			PlayerDeath fadeScript = FindObjectOfType<PlayerDeath>();
			if (fadeScript != null)
			{
				StartCoroutine(fadeScript.FadeOutThenLoad());
			}
			else
			{
				Debug.LogError("No PlayerDeath script found in scene. Add fade prefab.");
			}
			yield return null;
		}
	}
}
