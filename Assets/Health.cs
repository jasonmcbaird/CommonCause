using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

	public static int maxHealth = 5;
	public int health = maxHealth;
	public RectTransform healthBar;

	public void TakeDamage(int damage)
	{
		health -= damage;
		if(health <= 0)
		{
			Destroy(gameObject);
		}
		print(health);
		healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * 100f, healthBar.sizeDelta.y);
	}

}
