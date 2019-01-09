using MagicKit;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	// Properties
	public GameObject MenuLayer;
	public ControllerInput Controller;
	public Slice SliceBehavior;
	public DeployDrone DeployDroneBehavior;
	public Drone DroneBehavior;
	public ToggleVisibility ToggleVisibilityBehavior;

	void Start ()
	{
		Controller.OnHomeUp += OnMenuPress;
	}

	public void OnMenuPress()
	{
		MenuLayer.SetActive(!MenuLayer.activeSelf);
		SliceBehavior.enabled = !MenuLayer.activeSelf;
		DeployDroneBehavior.enabled = !MenuLayer.activeSelf;
		ToggleVisibilityBehavior.enabled = MenuLayer.activeSelf;
		if (MenuLayer.activeSelf)
		{
			DroneBehavior.Pause();
		}
		else
		{
			DroneBehavior.Resume();
		}
		if (MenuLayer.activeSelf)
		{
			var distance = SliceBehavior.Length / 2;
			var transform = SliceBehavior.Source.transform;
			var position = transform.position;
			var direction = transform.up;
			MenuLayer.transform.position = position + direction * distance;
			var targetRotation = Camera.main.transform.rotation.eulerAngles;
			targetRotation.x = 0;
			targetRotation.z = 0;
			MenuLayer.transform.rotation = Quaternion.Euler(targetRotation);
		}
	}

	public bool IsActive()
	{
		return MenuLayer.activeSelf;
	}
}
