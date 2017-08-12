using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
	[SyncVar(hook = "OnChangeHealth")]
	int health;
	public int maxHealth;
	public RectTransform healthBar;

	void Start ()
	{
		health = maxHealth;
	}

	public void TakeDamage(int damage)
	{
		if(!isServer)
		{ return; }
		health -= damage;
		if(health <= 0)
		{
			Destroy(gameObject);
		}
	}

	void OnChangeHealth(int health)
	{
		if(healthBar == null)
		{ return; }
		healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * 100f, healthBar.sizeDelta.y);
	}

}
