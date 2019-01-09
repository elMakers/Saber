using MagicKit;
using UnityEngine;

public class DeployDrone : MonoBehaviour
{
	// Properties
	public GameObject DroneObject;
	
	// Components
	private ControllerInput _controller;
	private Drone _drone;

	void Start()
	{
		_controller = GetComponent<ControllerInput>();
		_controller.OnBumperUp += OnBumperUp;

		_drone = DroneObject.GetComponent<Drone>();
	}

	void OnBumperUp()
	{
		DroneObject.SetActive(!DroneObject.activeSelf);
		if (DroneObject.activeSelf)
		{
			_drone.Reset();
			_drone.Resume();
		}
	}
}
