using UnityEngine;
using Random = UnityEngine.Random;

public class Slice : MonoBehaviour {
	// Properties
	public GameObject Sparks;
	public GameObject Source;
	public float Length = 0.5f;
	public float Width = 0.017f;
	public AudioSource Sound;
	public float MinPitch = 0.5f;
	public float MaxPitch = 1.5f;
	public int ParticleQueueSize = 50;
	public float SliceSoundMinCooldown = 0.1f;
	public float SliceSoundMaxCooldown = 0.5f;
	public ParticleSystem InnerDecalParticleSystem;
	public ParticleSystem OuterDecalParticleSystem;
	public float ParticleOffset = 0.001f;
	
	// Components
	private Extend _extend;
	private RaycastHit _hit;
	private Vector3 _extents;
	private GameObject[] _particleQueue;
	private int _particleQueueIndex;
	private float _lastSound;
	private float _sliceSoundCooldown;
	
	void Start()
	{
		_extend = GetComponent<Extend>();
		_extents = new Vector3(Width, Width, Width);
		_particleQueue = new GameObject[ParticleQueueSize];
		ResetSliceCooldown();
	}

	void ResetSliceCooldown()
	{
		_sliceSoundCooldown = Random.Range(SliceSoundMinCooldown, SliceSoundMaxCooldown);
	}
	
	void Update()
	{
		if (!_extend.Extended()) return;

		var transform = Source.transform;
		var position = transform.position;
		var direction = transform.up;
		
		// Debug.DrawLine(position, position + direction * Length, Color.yellow, 2.0f, false);
			
		var hitting = Physics.BoxCast(position, _extents, direction, out _hit, transform.rotation, Length);
		if (hitting)
		{
			// Debug.DrawLine(_hit.point, _hit.point + _hit.normal * Length, Color.red, 5.0f, false);
			// Debug.Log("Hit: " + _hit.normal);
			
			// Play Sound
			if (Time.time > _lastSound + _sliceSoundCooldown)
			{
				ResetSliceCooldown();
				_lastSound = Time.time;
				Sound.pitch = Random.Range(MinPitch, MaxPitch);
				Sound.transform.position = _hit.point;
				Sound.PlayOneShot(Sound.clip);
			}

			// Play spark particles
			var particleRotation = Quaternion.LookRotation(_hit.normal);
			var particles = _particleQueue[_particleQueueIndex];
			if (particles == null)
			{
				particles = Instantiate(Sparks, _hit.point, particleRotation);
				_particleQueue[_particleQueueIndex] = particles;
			}
			else
			{
				ParticleSystem system = particles.GetComponent<ParticleSystem>();
				if (system.isPlaying)
				{
					system.Stop();
				}

				system.transform.position = _hit.point;
				system.transform.rotation = particleRotation;
				system.Play();
			}
			_particleQueueIndex = (_particleQueueIndex + 1) % ParticleQueueSize;
			
			// Add decal particle
			var mainConfig = InnerDecalParticleSystem.main;
			var rotation = Mathf.Deg2Rad * particleRotation.eulerAngles;
			mainConfig.startRotationX = rotation.x;
			mainConfig.startRotationY = rotation.y;
			mainConfig.startRotationZ = rotation.z;
			InnerDecalParticleSystem.transform.position = _hit.point + _hit.normal * ParticleOffset;
			InnerDecalParticleSystem.Emit(1);

			mainConfig = OuterDecalParticleSystem.main;
			mainConfig.startRotationX = rotation.x;
			mainConfig.startRotationY = rotation.y;
			mainConfig.startRotationZ = rotation.z;
			OuterDecalParticleSystem.transform.position = _hit.point + _hit.normal * ParticleOffset * 2;
			OuterDecalParticleSystem.Emit(1);
		}
	}
}
