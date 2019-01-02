using MagicKit;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour {
	public Material Occlusion;
	public Material Wireframe;
	public ControllerInput Input;
	
	private Renderer _renderer;
	private Material[] _materials; 

	void Start ()
	{
		_renderer = GetComponent<MeshRenderer>();
		Input.OnBumperDown += OnBumperDown;
		Input.OnBumperUp += OnBumperUp;
	}
	
	void OnBumperDown()
	{

		Material[] materials1 = _renderer.materials;
		materials1[0] = Wireframe;
		_renderer.materials = materials1;
	}
	
	void OnBumperUp()
	{
		Material[] materials2 = _renderer.materials;
		materials2[0] = Occlusion;
		_renderer.materials = materials2;
	}
}
