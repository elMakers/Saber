using UnityEngine;

public class Saber : MonoBehaviour
{
	public AudioClip[] SwingSounds;
	public AudioSource SwingSource;
	public GameObject TipLocation;
	public float MinimumSwingSpeed = 0.2f;
	public float SwingSoundTimeout = 1.0f;
	
	private Rigidbody _body;
	
	// State
	private Vector3 _lastLocation;
	private float _lastSwingSound;

	// Use this for initialization
	void Start () {
		_body = TipLocation.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (_lastLocation == Vector3.zero)
		{
			_lastLocation = _body.position;
			return;
		}

		Vector3 velocity = _body.position - _lastLocation;
		_lastLocation = _body.position;
		var currentTime = Time.time;
		var speed = velocity.magnitude;
		// currentTime > _lastSwingSound + SwingSoundTimeout
		if (speed >= MinimumSwingSpeed)
		{
			// Debug.Log("SWING? " + currentTime + " : " + (_lastSwingSound + SwingSoundTimeout) + ", speed: " + speed);
			if (currentTime < _lastSwingSound + SwingSoundTimeout) return;
			_lastSwingSound = currentTime;
			var clipIndex = Random.Range(0, SwingSounds.Length);
			SwingSource.PlayOneShot(SwingSounds[clipIndex]);
		}
	}
}
