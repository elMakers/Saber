using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Saber : MonoBehaviour
{
	public AudioClip[] SwingSounds;
	public AudioSource SwingSource;
	public GameObject TipLocation;
	public float MinimumSwingSpeed = 0.2f;
	public float SwingSoundTimeout = 1.0f;
	public int SpeedSamples = 30;
	
	private Rigidbody _body;
	
	// State
	private Vector3 _lastLocation;
	private float _lastSwingSound;
	private float[] _speedSamples;
	private int _speedIndex;
	private int _sampleCount;

	// Use this for initialization
	void Start () 
	{
		_body = TipLocation.GetComponent<Rigidbody>();
		_speedSamples = new float[SpeedSamples];
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if (_lastLocation == Vector3.zero)
		{
			_lastLocation = _body.position;
			return;
		}

		Vector3 velocity = _body.position - _lastLocation;
		_lastLocation = _body.position;
		var currentTime = Time.time;
		var speed = velocity.magnitude;

		_speedSamples[_speedIndex] = speed;
		_sampleCount++;
		_speedIndex = (_speedIndex + 1) % SpeedSamples;
		var speedTotal = 0.0f;
		var speedCount = Math.Min(_sampleCount, SpeedSamples);
		for (var i = 0; i < speedCount; i++)
		{
			speedTotal += _speedSamples[i];
		}

		var averageSpeed = speedTotal / speedCount;
		//Debug.Log("Speed: " + speed + ", avg: " + averageSpeed + " from " + _body.position);
			
		if (averageSpeed >= MinimumSwingSpeed)
		{
			// Debug.Log("SWING? " + currentTime + " : " + (_lastSwingSound + SwingSoundTimeout) + ", speed: " + averageSpeed);
			if (currentTime < _lastSwingSound + SwingSoundTimeout) return;
			_lastSwingSound = currentTime;
			var clipIndex = Random.Range(0, SwingSounds.Length);
			SwingSource.PlayOneShot(SwingSounds[clipIndex]);
		}
	}
}
