using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public GameObject Explosion;
	public float Lifespan = 10.0f;

	void Awake()
	{
		Destroy(gameObject, Lifespan);
	}

	void OnCollisionEnter(Collision col)
	{
		// Debug.Log("Collided with " + col.collider.name);
		
		// Spawn an explosion
		if (Explosion)
		{
			Instantiate(Explosion, transform.position, Quaternion.identity);
		}
		
		Destroy(gameObject);
	}
}
