using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RangerBasicArrow : NetworkBehaviour
{
    private float arrowSpeed = 66f;
    public float arrowDirectionX;
    public float arrowDirectionY;
    Rigidbody2D rigidBody;

    void Awake ()
    {
        
    }
	
	void Update ()
    {
        
    }

    void SetLaunchVector(Vector3 launchVector)
    {
        arrowDirectionX = launchVector.x;
        arrowDirectionY = launchVector.y;
        rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.velocity = new Vector2(arrowDirectionX, arrowDirectionY).normalized * arrowSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
		{
        	DestroyObject(gameObject);
		}
    }

}
