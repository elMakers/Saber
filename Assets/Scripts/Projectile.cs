using MagicKit;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Projectile : MonoBehaviour 
{
	// Properties
	public GameObject Explosion;
	public float Lifespan = 10.0f;
	public AudioSource ReflectSound;
	public float ReflectMinPitch = 0.8f;
	public float ReflectMaxPitch = 1.2f;
	
	// Private data that needs to be initialized
	private ControllerInput _controller;
	
	void Awake()
	{
		Destroy(gameObject, Lifespan);
	}

	void OnCollisionEnter(Collision col)
	{
		// Check for blade reflect
		if (col.collider.gameObject.layer == 10)
		{
			// Play reflect sound
			var sound = Instantiate(ReflectSound, transform.position, Quaternion.identity);
			sound.pitch = Random.Range(ReflectMinPitch, ReflectMaxPitch);
			sound.PlayOneShot(sound.clip);
			
			// Add some haptics
			MLInputControllerFeedbackIntensity intensity = MLInputControllerFeedbackIntensity.Medium;
			_controller.Controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, intensity);
			
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

	public void SetController(ControllerInput controller)
	{
		_controller = controller;
	}
}
