using MagicKit;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour {
	// Properties
	public Material Occlusion;
	public Material Wireframe;
	public ControllerInput Input;
	
	// State
	private bool _visible;
	
	void Start ()
	{
		Input.OnBumperUp += OnBumperUp;
	}
	
	void OnBumperUp()
	{
		if (!enabled) return;
		_visible = !_visible;
		if (_visible)
		{
			SwapMaterial(Wireframe);
		}
		else
		{
			SwapMaterial(Occlusion);
		}
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
