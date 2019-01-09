using MagicKit;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	// Properties
	public GameObject MenuLayer;
	public ControllerInput Controller;
	public Slice SliceBehavior;
	public float Distance = 1.0f;

	void Start ()
	{
		Controller.OnHomeUp += OnMenuPress;
	}

	public void OnMenuPress()
	{
		MenuLayer.SetActive(!MenuLayer.activeSelf);
		SliceBehavior.enabled = !MenuLayer.activeSelf;
		if (MenuLayer.activeSelf)
		{
			Vector3 position = Camera.main.transform.position;
			MenuLayer.transform.position = position + Camera.main.transform.forward * Distance;
			var targetRotation = Camera.main.transform.rotation.eulerAngles;
			targetRotation.x = 0;
			targetRotation.z = 0;
			MenuLayer.transform.rotation = Quaternion.Euler(targetRotation);
		}
	}
}
