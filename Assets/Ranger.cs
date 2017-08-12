using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ranger : NetworkBehaviour
{
    public Sprite idleImage;
    public Sprite attackImage;
    public Sprite ultimateImage;
    public GameObject basicAttackArrow;
	public GameObject bomb;

    private static float speed = 6f;
    private static float attackSpeed = 0.2f;
    private static float chargeTime = 0.75f;
    private static float ultCooldown = 8f;
    private static Color damagedColor = new Color(255f / 255f, 132f / 255f, 128f / 255f);
    private Color defaultColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    private static Color chargingColor = new Color(220f / 255f, 232f / 255f, 160f / 255f);
    private static Color chargedColor = new Color(169f / 255f, 192f / 255f, 106f / 255f);

    private float lastAttackTime = 0;
    private float timeLastDamaged = 0;
    private float timeStartedCharging = -1;
    private float lastUltTime = 0;
    private GameObject arrow;

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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool attackChargeInput = Input.GetMouseButton(0);
        bool attackReleaseInput = Input.GetMouseButtonUp(0);
        bool ultInput = Input.GetMouseButtonDown(1);

        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.velocity = new Vector2(horizontalInput * speed * 100 * Time.deltaTime, verticalInput * speed * 100 * Time.deltaTime);

        if (attackChargeInput && !IsCharging() && TimeSinceLastAttack() > attackSpeed + 0.05f)
        {
            StartCharging();
        }

        if (attackReleaseInput && TimeSinceStartedCharging() != -1)
        {
            if (TimeSinceStartedCharging() > chargeTime)
            {
                ChargeAttack();
            }
            else
            {
                Attack();
            }
        }

        if (IsCharging() && TimeSinceStartedCharging() > chargeTime / 2)
        {
            ChangeColor(defaultColor, chargingColor);
        }

        if (IsCharging() && TimeSinceStartedCharging() > chargeTime)
        {
            ChangeColor(chargingColor, chargedColor);
        }

        if (TimeSinceStartedCharging() == -1 && TimeSinceLastAttack() > attackSpeed)
        {
            FinishAttackAnimation();
        }

        if (Time.time > timeLastDamaged + 0.4)
        {
            ChangeColor(damagedColor, defaultColor);
        }

        if (ultInput && Time.time > lastUltTime + ultCooldown)
        {
            UltimateAttack();
        }

        if (GetComponent<SpriteRenderer>().sprite == ultimateImage && Time.time > lastUltTime + 1f)
        {
            GetComponent<SpriteRenderer>().sprite = idleImage;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Slime"))
        {
            if (!other.gameObject.GetComponent<Slime>().isStunned())
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
		CmdCreateArrow(Camera.main.ScreenToWorldPoint(Input.mousePosition), false);
        timeStartedCharging = -1;
        lastAttackTime = Time.time;
    }

    private void ChargeAttack()
    {
        GetComponent<SpriteRenderer>().sprite = attackImage;
		CmdCreateArrow(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
        timeStartedCharging = -1;
        lastAttackTime = Time.time;
    }

    private void UltimateAttack()
    {
        GetComponent<SpriteRenderer>().sprite = ultimateImage;
		CmdCreateBombs();
        lastUltTime = Time.time;
    }

    private void FinishAttackAnimation()
    {
        if (GetComponent<SpriteRenderer>().sprite == attackImage)
        {
            GetComponent<SpriteRenderer>().sprite = idleImage;
        }
        if (GetComponent<SpriteRenderer>().color == chargedColor || GetComponent<SpriteRenderer>().color == chargingColor)
        {
            GetComponent<SpriteRenderer>().color = defaultColor;
        }
  
    }

    private Direction GetDirection(float horizontalInput, float verticalInput)
    {
        if (Mathf.Abs(horizontalInput) >= Mathf.Abs(verticalInput))
        {
            if (horizontalInput < 0)
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
	private void CmdCreateArrow(Vector3 mousePosition, bool piercing)
    {
		mousePosition.z = 0;
        arrow = GameObject.Instantiate(basicAttackArrow);
		arrow.GetComponent<RangerBasicArrow>().piercing = piercing;

		Vector3 playerDifference = mousePosition - transform.position;
		float arrowAngle = Mathf.Atan2(playerDifference.y, playerDifference.x) * Mathf.Rad2Deg + 180;

		arrow.transform.position = transform.position + new Vector3(playerDifference.x, playerDifference.y, -1).normalized;
        arrow.transform.rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
        arrow.SendMessage("SetLaunchVector", playerDifference);
		NetworkServer.Spawn(arrow);
    }

	[Command]
	private void CmdCreateBombs()
	{
		GameObject bomb1 = GameObject.Instantiate(bomb);
		GameObject bomb2 = GameObject.Instantiate(bomb);
		GameObject bomb3 = GameObject.Instantiate(bomb);

		bomb1.transform.position = transform.position + new Vector3(0, 2, 0);
		bomb2.transform.position = transform.position + new Vector3(Mathf.Sqrt(2), -Mathf.Sqrt(2), 0);
		bomb3.transform.position = transform.position + new Vector3(- Mathf.Sqrt(2), -Mathf.Sqrt(2), 0);

		NetworkServer.Spawn(bomb1);
		NetworkServer.Spawn(bomb2);
		NetworkServer.Spawn(bomb3);
	}

    private static Vector3 GetRotationForDirection(Direction direction)
    {
        switch (direction)
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
        if (fromColor == GetComponent<SpriteRenderer>().color)
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
        if (timeStartedCharging == -1)
        {
            return -1;
        }
        return Time.time - timeStartedCharging;
    }

};