using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RangerBasicArrow : NetworkBehaviour
{
    private float arrowSpeed = 66f;
	public bool piercing = false;
    Rigidbody2D rigidBody;

    public void SetLaunchVector(Vector3 launchVector)
    {
		GetComponent<Rigidbody2D>().velocity = new Vector2(launchVector.x, launchVector.y).normalized * arrowSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player") && !piercing)
		{
        	DestroyObject(gameObject);
		}
    }

}
