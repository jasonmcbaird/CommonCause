using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerBasicArrow : MonoBehaviour
{
    private float arrowSpeed = 5.5f;
    public float arrowDirectionX;
    public float arrowDirectionY;
    Rigidbody2D rigidBody;

    void Awake ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    void SetLaunchVector(Vector3 launchVector)
    {
        arrowDirectionX = launchVector.x;
        arrowDirectionY = launchVector.y;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(new Vector2(arrowDirectionX, arrowDirectionY) * arrowSpeed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
        else
        {
            DestroyObject(gameObject);
        }
    }

}
