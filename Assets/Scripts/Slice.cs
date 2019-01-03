using UnityEngine;
using Random = UnityEngine.Random;

public class Slice : MonoBehaviour {
	// Properties
	public GameObject Sparks;
	public GameObject Source;
	public float Length = 0.5f;
	public float Width = 0.017f;
	public int AdjacentCount = 8;
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
	private GameObject[] _particleQueue;
	private int _particleQueueIndex;
	private float _lastSound;
	private float _sliceSoundCooldown;
	private ParticleSystem.Particle[] _particles;
	private int _particlePurgeIndex;
	private RaycastHit[] _hits; 
	private RaycastHit[] _adjacentHits; 
	
	void Start()
	{
		_extend = GetComponent<Extend>();
		_particleQueue = new GameObject[ParticleQueueSize];
		_particles = new ParticleSystem.Particle[InnerDecalParticleSystem.main.maxParticles];
		_hits = new RaycastHit[8];
		_adjacentHits = new RaycastHit[1];
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
		
		// Raycast to check for surfaces to slice
		var hitCount = Physics.RaycastNonAlloc(position, direction, _hits, Length);
		if (hitCount == 0) return;

		var firstHit = _hits[0];
		// TODO: Check for distance travelled
		
		// Check for particle limit
		var count = InnerDecalParticleSystem.GetParticles(_particles);
		if (count == InnerDecalParticleSystem.main.maxParticles) {
			_particles[_particlePurgeIndex].remainingLifetime = -1;
			InnerDecalParticleSystem.SetParticles(_particles, count);
			
			count = OuterDecalParticleSystem.GetParticles(_particles);
			if (count == OuterDecalParticleSystem.main.maxParticles)
			{
				_particles[_particlePurgeIndex].remainingLifetime = -1;
				OuterDecalParticleSystem.SetParticles(_particles, count);
			}

			_particlePurgeIndex = (_particlePurgeIndex + 1) % count;
		}		
		// Debug.DrawLine(position, position + direction * Length, Color.yellow, 2.0f, false);
		
		// If the middle hit, check surface of blade to see if we are at an edge
		var adjacentHitCount = 0;
		// TODO: Don't use distance, check for same object instead
		for (var i = 0; i < AdjacentCount; i++)
		{
			var angle = new Vector3(Mathf.Cos(Mathf.PI * i / AdjacentCount), Mathf.Sin(Mathf.PI * i / AdjacentCount), 0);
			var tangent = Vector3.Cross(direction, angle);
			var adjacentPosition = position + (tangent * Width);
			if (Physics.RaycastNonAlloc(adjacentPosition, direction, _adjacentHits, Length) != 0)
			{
				if (_adjacentHits[0].collider == firstHit.collider)
				{
					adjacentHitCount++;
				}
			}
			// Debug.DrawLine(adjacentPosition, adjacentPosition + direction * distance, Color.yellow, 2.0f, false);
		}
		
		// Play Sound
		if (Time.time > _lastSound + _sliceSoundCooldown)
		{
			ResetSliceCooldown();
			_lastSound = Time.time;
			Sound.pitch = Random.Range(MinPitch, MaxPitch);
			Sound.transform.position = firstHit.point;
			Sound.PlayOneShot(Sound.clip);
		}
		
		foreach (var hit in _hits)
		{
			// Debug.DrawLine(hit.point, hit.point + hit.normal * Length, Color.red, 5.0f, false);
			// Debug.Log("Hit: " + hit.normal + " adjacent: " + adjacentHitCount);

			// Play spark particles
			var particleRotation = Quaternion.LookRotation(hit.normal);
			var particles = _particleQueue[_particleQueueIndex];
			if (particles == null)
			{
				particles = Instantiate(Sparks, hit.point, particleRotation);
				_particleQueue[_particleQueueIndex] = particles;
			}
			else
			{
				ParticleSystem system = particles.GetComponent<ParticleSystem>();
				if (system.isPlaying)
				{
					system.Stop();
				}

				system.transform.position = hit.point;
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
			InnerDecalParticleSystem.transform.position = hit.point + hit.normal * ParticleOffset;
			InnerDecalParticleSystem.Emit(1);

			// Add outer decal if not near an edge
			if (adjacentHitCount == AdjacentCount)
			{
				mainConfig = OuterDecalParticleSystem.main;
				mainConfig.startRotationX = rotation.x;
				mainConfig.startRotationY = rotation.y;
				mainConfig.startRotationZ = rotation.z;
				OuterDecalParticleSystem.transform.position = hit.point + hit.normal * ParticleOffset * 2;
				OuterDecalParticleSystem.Emit(1);
			}
		}
	}
}
