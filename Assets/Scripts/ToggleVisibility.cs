using MagicKit;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour {
	public Material Occlusion;
	public Material Wireframe;
	public ControllerInput Input;
	
	void Start ()
	{
		Input.OnBumperDown += OnBumperDown;
		Input.OnBumperUp += OnBumperUp;
	}
	
	void OnBumperDown()
	{
		SwapMaterial(Wireframe);
	}
	
	void OnBumperUp()
	{
		SwapMaterial(Occlusion);
	}

	private void SwapMaterial(Material material)
	{
		foreach (Transform child in transform)
		{
			
			var renderer = child.gameObject.GetComponent<MeshRenderer>();
			Material[] materials = renderer.materials;
			materials[0] = material;
			renderer.materials = materials;
		}
	}
}
