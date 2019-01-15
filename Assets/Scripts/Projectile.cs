using UnityEngine;

public class Projectile : MonoBehaviour 
{
	// Properties
	public GameObject Explosion;
	public float Lifespan = 10.0f;
	public AudioSource ReflectSound;
	public float ReflectMinPitch = 0.8f;
	public float ReflectMaxPitch = 1.2f;
	
	void Awake()
	{
		Destroy(gameObject, Lifespan);
	}

	void OnCollisionEnter(Collision col)
	{
		// Check for blade reflect
		if (col.collider.gameObject.layer == 10)
		{
			var sound = Instantiate(ReflectSound, transform.position, Quaternion.identity);
			sound.pitch = Random.Range(ReflectMinPitch, ReflectMaxPitch);
			sound.PlayOneShot(sound.clip);
			
			// TODO: Reflect instead of destroy
		}
		
		// Spawn an explosion
		if (Explosion)
		{
			var explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
			Destroy(explosion, Lifespan);
		}
		
		Destroy(gameObject);
	}
}
