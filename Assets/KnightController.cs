﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum Direction
{
	up,
	right,
	down,
	left
};

public class KnightController: NetworkBehaviour
{
	public Sprite idleImage;
	public Sprite attackImage;
	public Sprite ultimateImage;
	public GameObject basicAttackPrefab;
	public GameObject chargeAttackPrefab;

	private static float reach = 1.5f;
	private static float speed = 6f;
	private static float attackSpeed = 0.2f;
	private static float chargeTime = 0.75f;
	private static float ultCooldown = 8f;
	private static Color damagedColor = new Color(255f/255f, 132f/255f, 128f/255f);
	private Color defaultColor = new Color(255f/255f, 255f/255f, 255f/255f);
	private static Color chargingColor = new Color(220f/255f, 232f/255f, 160f/255f);
	private static Color chargedColor = new Color(169f/255f, 192f/255f, 106f/255f);

	private float lastAttackTime = 0;
	private float timeLastDamaged = 0;
	private float timeStartedCharging = -1;
	private float lastUltTime = 0;
	private GameObject attackObject;
	private BoxCollider2D ultCollider;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		defaultColor = new Color(220f/255f, 220f/255f, 255f/255f);
		ChangeColor(defaultColor);
	}

	void Update()
	{
		if(!isLocalPlayer) {
			return;
		}
		float horizontalInput = Input.GetAxis ("Horizontal");
		float verticalInput = Input.GetAxis ("Vertical");
		bool attackChargeInput = Input.GetMouseButton(0);
		bool attackReleaseInput = Input.GetMouseButtonUp(0);
		bool ultInput = Input.GetMouseButtonDown(1);

		Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();

		rigidBody.velocity = new Vector2 (horizontalInput * speed * 100 * Time.deltaTime, verticalInput * speed * 100 * Time.deltaTime);

		if(attackChargeInput && !IsCharging() && TimeSinceLastAttack() > attackSpeed + 0.05f)
		{
			StartCharging();
		}

		if(attackReleaseInput && TimeSinceStartedCharging() != -1)
		{
			if(TimeSinceStartedCharging() > chargeTime)
			{
				SpinAttack();
			}
			else
			{
				Attack();
			}
		}

		if(IsCharging() && TimeSinceStartedCharging() > chargeTime / 2)
		{
			ChangeColor(defaultColor, chargingColor);
		}

		if(IsCharging() && TimeSinceStartedCharging() > chargeTime)
		{
			ChangeColor(chargingColor, chargedColor);
		}

		if(TimeSinceStartedCharging() == -1 && TimeSinceLastAttack() > attackSpeed)
		{
			FinishAttackAnimation();
		}

		if(Time.time > timeLastDamaged + 0.4)
		{
			ChangeColor(damagedColor, defaultColor);
		}

		if(ultInput && Time.time > lastUltTime + ultCooldown)
		{
			UltimateAttack();
		}

		if(GetComponent<SpriteRenderer>().sprite == ultimateImage && Time.time > lastUltTime + 1f)
		{
			GetComponent<SpriteRenderer>().sprite = idleImage;
		}

		if(Time.time > lastUltTime + 2f)
		{
			Destroy(ultCollider);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Slime"))
		{
			if(!other.gameObject.GetComponent<Slime>().isStunned())
			{
				GetComponent<SpriteRenderer>().color = damagedColor;
				GetComponent<Health>().TakeDamage(1);
				timeLastDamaged = Time.time;
			}
		}
	}

	private void Attack()
	{
		GetComponent<SpriteRenderer>().sprite = attackImage;
		CmdCreateAttackObject(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		timeStartedCharging = -1;
		lastAttackTime = Time.time;
	}

	private void SpinAttack()
	{
		GetComponent<SpriteRenderer>().sprite = attackImage;
		CmdCreateSpinAttackObject();
		timeStartedCharging = -1;
		lastAttackTime = Time.time;
	}


	private void UltimateAttack()
	{
		GetComponent<SpriteRenderer>().sprite = ultimateImage;
		lastUltTime = Time.time;
		CmdPullEnemies();
	}

	[Command]
	private void CmdPullEnemies()
	{
		ultCollider = gameObject.AddComponent<BoxCollider2D>();
		GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
		foreach(GameObject slime in slimes)
		{
			Rigidbody2D rigidbody = slime.GetComponent<Rigidbody2D>();
			Vector2 offsetFromSlimeToKnight = transform.position - slime.transform.position;
			rigidbody.velocity = offsetFromSlimeToKnight * 2;
			slime.GetComponent<Slime>().stun();
		}
	}

	private void FinishAttackAnimation()
	{
		if(GetComponent<SpriteRenderer>().sprite == attackImage)
		{
			GetComponent<SpriteRenderer>().sprite = idleImage;
		}
		if(GetComponent<SpriteRenderer>().color == chargedColor || GetComponent<SpriteRenderer>().color == chargingColor)
		{
			GetComponent<SpriteRenderer>().color = defaultColor;
		}
	}

	private Direction GetDirection(float horizontalInput, float verticalInput)
	{
		if(Mathf.Abs(horizontalInput) >= Mathf.Abs(verticalInput))
		{
			if(horizontalInput < 0)
			{
				return Direction.left;
			}
			else
			{
				return Direction.right;
			}
		}
		else
		{
			if (verticalInput < 0)
			{
				return Direction.down;
			}
			else
			{
				return Direction.up;
			}
		}
	}

	[Command]
	private void CmdCreateAttackObject(Vector3 mousePosition)
	{
		mousePosition.z = 0;
		Direction facing = GetDirection(mousePosition.x - gameObject.transform.position.x, mousePosition.y - gameObject.transform.position.y);

		attackObject = GameObject.Instantiate(basicAttackPrefab);
		Vector2 offsetFromKnight = new Vector2(0, 0);
		switch(facing)
		{
		case Direction.up:
			{
				offsetFromKnight = new Vector2(0, reach);
				break;
			}
		case Direction.right:
			{
				offsetFromKnight = new Vector2(reach, 0);
				break;
			}
		case Direction.down:
			{
				offsetFromKnight = new Vector2(0, -reach);
				break;
			}
		case Direction.left:
			{
				offsetFromKnight = new Vector2(-reach, 0);
				break;
			}
		}
		attackObject.transform.position = new Vector3(transform.position.x + offsetFromKnight.x, transform.position.y + offsetFromKnight.y, -1);
		attackObject.transform.Rotate(GetRotationForDirection(facing));
		NetworkServer.Spawn(attackObject);
		Destroy(attackObject, attackSpeed);
	}

	[Command]
	private void CmdCreateSpinAttackObject()
	{
		attackObject = GameObject.Instantiate(chargeAttackPrefab);
		attackObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
		NetworkServer.Spawn(attackObject);
		Destroy(attackObject, attackSpeed);
	}

	private static Vector3 GetRotationForDirection(Direction direction)
	{
		switch(direction) 
		{
		case Direction.up:
			{
				return new Vector3(0, 0, -90);
			}
		case Direction.right:
			{
				return new Vector3(0, 0, 180);
			}
		case Direction.down:
			{
				return new Vector3(0, 0, 90);
			}
		case Direction.left:
			{
				return new Vector3(0, 0, 0);
			}
		}
		return new Vector3(0, 0, 0);
	}

	private void ChangeColor(Color fromColor, Color toColor)
	{
		if(fromColor == GetComponent<SpriteRenderer>().color)
		{
			GetComponent<SpriteRenderer>().color = toColor;
			return;
		}
	}

	private void ChangeColor(Color toColor)
	{
		GetComponent<SpriteRenderer>().color = toColor;
	}

	private float TimeSinceLastAttack()
	{
		return Time.time - lastAttackTime;
	}

	private void StartCharging()
	{
		timeStartedCharging = Time.time;
	}

	private bool IsCharging()
	{
		return timeStartedCharging != -1;
	}

	private float TimeSinceStartedCharging()
	{
		if(timeStartedCharging == -1)
		{
			return -1;
		}
		return Time.time - timeStartedCharging;
	}
	
};
