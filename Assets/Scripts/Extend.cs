using System;
using MagicKit;
using UnityEngine;

public class Extend : MonoBehaviour {
	// Properties
	public AudioSource ExtendSound;
	public AudioSource RetractSound;
	public AudioSource HumSound;
	public float Duration = 2.0f;
	public GameObject Blade;
	
	// References
	private ControllerInput _input;
	
	// State
	private Boolean _extended;
	private Boolean _animating;
	private float _animationStart;

	void Start () 
	{
		_input = GetComponent<ControllerInput>();
		_input.OnTriggerDown += OnTriggerDown;
	}

	void OnTriggerDown()
	{
		if (_animating) return;

		if (_extended)
		{
			RetractSound.Play();
		}
		else
		{
			ExtendSound.Play();
		}

		_animationStart = Time.time;
		_animating = true;
		_extended = !_extended;
	}

	void Update () 
	{
		if (_animating && Time.time > _animationStart + Duration)
		{
			_animating = false;
			if (_extended)
			{
				HumSound.Play();
				Blade.transform.localScale = Vector3.one;
			}
			else
			{
				HumSound.Pause();
				Blade.transform.localScale = Vector3.zero;
			}
		}

		if (_animating)
		{
			var length = (Time.time - _animationStart) / Duration;
			if (!_extended)
			{
				length = 1.0f - length;
			}
			length = Math.Max(0, length);
			Blade.transform.localScale = new Vector3(1.0f, length, 1.0f);
		}
	}

	public Boolean Extended()
	{
		return _extended && !_animating;
	}
}
