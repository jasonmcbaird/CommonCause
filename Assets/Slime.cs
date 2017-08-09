﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime: MonoBehaviour
{

	public GameObject target;
	public int health = 3;
	public static int speed = 3;

	private static Color damagedColor = new Color();
	private static Color defaultColor = new Color();
	private static Color stunnedColor = new Color();

	private float timeLastDamaged = 0;
	private float timeLastStunned = -1;
	
	void Start()
	{
		damagedColor.r = 132f/255f;
		damagedColor.g = 255f/255f;
		damagedColor.b = 128f/255f;
		damagedColor.a = 255f/255f;
		defaultColor.r = 255f/255f;
		defaultColor.g = 255f/255f;
		defaultColor.b = 255f/255f;
		defaultColor.a = 255f/255f;
		stunnedColor.r = 220f/255f;
		stunnedColor.g = 232f/255f;
		stunnedColor.b = 160f/255f;
		stunnedColor.a = 255f/255f;
		tag = "Slime";
	}

	void Update()
	{
		if(Time.time > timeLastStunned + 2f)
		{
			timeLastStunned = -1;
		}

		if(!isStunned())
		{
			MoveTowardPlayer();
		}

		if(GetComponent<SpriteRenderer>().color == defaultColor && isStunned())
		{
			GetComponent<SpriteRenderer>().color = stunnedColor;
		}

		if(GetComponent<SpriteRenderer>().color == stunnedColor && !isStunned())
		{
			GetComponent<SpriteRenderer>().color = defaultColor;
		}

		if(GetComponent<SpriteRenderer>().color == damagedColor && Time.time > timeLastDamaged + 0.4f)
		{
			GetComponent<SpriteRenderer>().color = defaultColor;
		}
	}

	public void stun()
	{
		timeLastStunned = Time.time;
	}

	public bool isStunned()
	{
		return timeLastStunned != -1;
	}

	private void MoveTowardPlayer()
	{
		Vector2 pathToTarget = target.transform.position - transform.position;
		GetComponent<Rigidbody2D>().velocity = pathToTarget.normalized * 100 * speed * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("PlayerAttack") || other.gameObject.CompareTag("PlayerAttackDouble"))
		{
			GetComponent<SpriteRenderer>().color = damagedColor;
			if(other.gameObject.CompareTag("PlayerAttack"))
			{
				health -= 1;
			}
			else if(other.gameObject.CompareTag("PlayerAttackDouble"))
			{
				health -= 2;
			}
			timeLastDamaged = Time.time;
			if(health <= 0)
			{
				DestroyObject(gameObject);
			}
		}
	}
}