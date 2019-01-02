using System;
using MagicKit;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

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
		_input.OnTouchUp += OnTouchUp;
	}

	void OnTouchUp()
	{
		if (_animating) return;
		if (_input.Controller.TouchpadGesture.Type != MLInputControllerTouchpadGestureType.Swipe) return;

		if (_input.Controller.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
		{
			StartRetracting();
		}
		else if (_input.Controller.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
		{
			StartExtending();
		}
	}

	void OnTriggerDown()
	{
		if (_animating) return;

		if (_extended)
		{
			StartRetracting();
		}
		else
		{
			StartExtending();
		}
	}

	private void ChangeState()
	{
		_animationStart = Time.time;
		_animating = true;
	}

	private void StartExtending()
	{
		if (_animating || _extended) return;
		ChangeState();
		ExtendSound.Play();
		_extended = true;
	}

	private void StartRetracting()
	{
		if (_animating || !_extended) return;
		ChangeState();
		RetractSound.Play();
		_extended = false;
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
		return _extended || (!_extended && _animating);
	}
}
