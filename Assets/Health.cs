using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
	[SyncVar(hook = "OnChangeHealth")]
	public int health = maxHealth;
	public static int maxHealth = 5;
	public RectTransform healthBar;

	public void TakeDamage(int damage)
	{
		if(!isServer)
		{ return; }
		health -= damage;
		if(health <= 0)
		{
			Destroy(gameObject);
		}
		print(health);

	}

	void OnChangeHealth(int health)
	{
		healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * 100f, healthBar.sizeDelta.y);
	}

}
