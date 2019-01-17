using MagicKit;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireBlaster : MonoBehaviour
{
	// Properties
	public GameObject Projectile;
	public ControllerInput Controller;
	public float MinFrequency = 0.3f;
	public float MaxFrequency = 1.2f;
	public float Speed = 2.0f;
	public float SpawnOffset = 2.0f;
	public float MinPitch = 0.8f;
	public float MaxPitch = 1.2f;
	public AudioSource Sound;
	public float MaxHeight = 5.0f;
	public float MinHeight = 0.1f;
	public float MaxSide = 1.0f;

	// State
	private float _nextFire;
	private Rigidbody _body;
	private RaycastHit[] _hits;
	
	void Start ()
	{
		ScheduleFire();
		_body = GetComponent<Rigidbody>();
		_hits = new RaycastHit[1];
	}

	private void ScheduleFire()
	{
		_nextFire = Time.time + Random.Range(MinFrequency, MaxFrequency);
	}
	
	void Update () 
	{
		if (Time.time > _nextFire)
		{
			Fire();
			ScheduleFire();
		}
	}

	private void Fire()
	{
		// Look for the ground
		Vector3 targetLocation = Camera.main.transform.position;
		var hitCount = Physics.RaycastNonAlloc(targetLocation, Vector3.down, _hits, MaxHeight, 9);
		if (hitCount > 0)
		{
			var distanceToFloor = targetLocation.y - _hits[0].point.y;
			if (distanceToFloor > MinHeight)
			{
				distanceToFloor -= MinHeight;
				targetLocation.y -= Random.Range(0, distanceToFloor);
			}
		}
		
		// Randomly target right or left
		targetLocation.x += Random.Range(-MaxSide, MaxSide);
		targetLocation.z += Random.Range(-MaxSide, MaxSide);
		
		Vector3 direction = targetLocation - _body.position;
		direction = direction.normalized;
		var spawnLocation = _body.position + direction * SpawnOffset;
		GameObject spawned = Instantiate(Projectile, spawnLocation, Quaternion.LookRotation(direction));
		spawned.GetComponent<Projectile>().SetController(Controller);
		spawned.GetComponent<Rigidbody>().AddForce(direction * Speed);
		
		Sound.pitch = Random.Range(MinPitch, MaxPitch);
		//Sound.transform.position = _body.position;
		Sound.PlayOneShot(Sound.clip);
	}
}
