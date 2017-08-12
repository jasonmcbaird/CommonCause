using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb: NetworkBehaviour
{
	public float fuseTime = 3;
	public Sprite explodeSprite;
	private float startTime;
	private bool exploded = false;

	void Start()
	{
		startTime = Time.time;
	}
	
	void Update()
	{
		if(Time.time > startTime + fuseTime && !exploded)
		{
			Explode();
		}
	}

	private void Explode()
	{
		BoxCollider2D pushCollider = gameObject.AddComponent<BoxCollider2D>();
		pushCollider.transform.localScale = new Vector3(2, 2, 2);
		Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(transform.position, 5);
		foreach(Collider2D collider in overlappingColliders)
		{
			GameObject gameObject = collider.gameObject;
			if(gameObject.CompareTag("Slime"))
			{
				gameObject.GetComponent<Health>().TakeDamage(1);
			}
		}
		GetComponent<SpriteRenderer>().sprite = explodeSprite;
		exploded = true;
		GameObject.Destroy(gameObject, 1);
	}
}
