using UnityEngine;
using Random = UnityEngine.Random;

public class FireBlaster : MonoBehaviour
{
	// Properties
	public GameObject Projectile;
	public float MinFrequency = 0.3f;
	public float MaxFrequency = 1.2f;
	public float Speed = 2.0f;
	public float SpawnOffset = 2.0f;

	// State
	private float _nextFire;
	private Rigidbody _body;
	
	void Start ()
	{
		ScheduleFire();
		_body = GetComponent<Rigidbody>();
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
		Vector3 direction = Camera.main.transform.position - _body.position;
		direction = direction.normalized;
		var spawnLocation = _body.position + direction * SpawnOffset;
		GameObject spawned = Instantiate(Projectile, spawnLocation, Quaternion.LookRotation(direction));
		spawned.GetComponent<Rigidbody>().AddForce(direction * Speed);
	}
}
