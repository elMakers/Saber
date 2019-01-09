using UnityEngine;

public class Drone : MonoBehaviour
{
	// Properties
	public float SpawnDistance = 2.0f;
	
	// Components
	private Rigidbody _body;
	
	// State
	private bool _paused;
	
	void Start ()
	{
		
	}

	public void Pause()
	{
		_paused = true;
	}

	public void Resume()
	{
		_paused = false;
	}

	public void Reset()
	{
		if (_body == null)
		{
			_body = GetComponent<Rigidbody>();
		}
		Vector3 position = Camera.main.transform.position;
		_body.transform.position = position + Camera.main.transform.forward * SpawnDistance;
		var targetRotation = Camera.main.transform.rotation.eulerAngles;
		_body.transform.rotation = Quaternion.Euler(targetRotation);
	}
	
	void Update ()
	{
		if (_paused) return;
		
	}
}
