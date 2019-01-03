using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slice : MonoBehaviour
{
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
	public float DecalMinDistance = 0.005f;

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
	private Vector3 _lastHit;
	private bool _isFirstHit = true;

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

	void PlaySparks(RaycastHit hit, Quaternion particleRotation)
	{
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
	}

	void CheckHits(int hitCount, Vector3 position, Vector3 direction)
	{
		// If the middle hit, check surface of blade to see if we are at an edge
		var firstHit = _hits[0];
		var adjacentHitCount = 0;
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
		
		for (var i = 0; i < hitCount; i++)
		{
			// Debug.DrawLine(hit.point, hit.point + hit.normal * Length, Color.red, 5.0f, false);
			// Debug.Log("Hit: " + hit.normal + " adjacent: " + adjacentHitCount);

			// Check for missing normal
			var hit = _hits[i];
			if (hit.normal == Vector3.zero) continue;
			
			// Play spark particles
			var particleRotation = Quaternion.LookRotation(hit.normal);
			PlaySparks(hit, particleRotation);
			
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

	void PurgeParticles(int amountToPurge)
	{
		var count = InnerDecalParticleSystem.GetParticles(_particles);
		if (count + amountToPurge < InnerDecalParticleSystem.main.maxParticles)
		{
			return;
		}
		for (var i = 0; i < amountToPurge; i++)
		{
			var index = (_particlePurgeIndex + i) % count;
			_particles[index].remainingLifetime = -1;
		}
		InnerDecalParticleSystem.SetParticles(_particles, count);
		
		count = OuterDecalParticleSystem.GetParticles(_particles);
		for (var i = 0; i < amountToPurge; i++)
		{
			var index = (_particlePurgeIndex + i) % count;
			_particles[index].remainingLifetime = -1;
		}
		OuterDecalParticleSystem.SetParticles(_particles, count);
		_particlePurgeIndex = (_particlePurgeIndex + 1) % amountToPurge;
	}

	void Update()
	{
		if (!_extend.Extended()) return;

		var transform = Source.transform;
		var position = transform.position;
		var direction = transform.up;
		
		// Raycast to check for surfaces to slice
		
		// Debug.DrawLine(position, position + direction * Length, Color.yellow, 2.0f, false);
		var hitCount = Physics.RaycastNonAlloc(position, direction, _hits, Length);
		if (hitCount == 0) return;
		var firstHit = _hits[0];
		Vector3[] interpolateTargets = null;
		
		// Play Sound
		if (Time.time > _lastSound + _sliceSoundCooldown)
		{
			ResetSliceCooldown();
			_lastSound = Time.time;
			Sound.pitch = Random.Range(MinPitch, MaxPitch);
			Sound.transform.position = firstHit.point;
			Sound.PlayOneShot(Sound.clip);
		}

		// Check for distance travelled and interpolate
		if (!_isFirstHit)
		{
			var interpolateDirection = (firstHit.point - _lastHit);
			var distance = interpolateDirection.magnitude;
			if (distance <= DecalMinDistance)
			{
				// Not moved far enough for decals
				// Still want some sparks though
				var particleRotation = Quaternion.LookRotation(firstHit.normal);
				PlaySparks(firstHit, particleRotation);
				return;
			}

			if (distance >= DecalMinDistance  * 2)
			{
				interpolateDirection = interpolateDirection.normalized * DecalMinDistance;
				interpolateTargets = new Vector3[(int)Math.Ceiling(distance / DecalMinDistance) - 1];
				Vector3 targetLocation = _lastHit + interpolateDirection;
				for (int i = 0; i < interpolateTargets.Length; i++)
				{
					interpolateTargets[i] = targetLocation;
					targetLocation += interpolateDirection;
				}				
			}
		}

		_isFirstHit = false;
		_lastHit = firstHit.point;
		
		// Check for particle limit
		var particleCount = hitCount;
		if (interpolateTargets != null)
		{
			particleCount *= (interpolateTargets.Length + 1);
		}
		PurgeParticles(particleCount);
		
		// Check current location
		CheckHits(hitCount, position, direction);
		
		// Check interpolated locations
		if (interpolateTargets != null)
		{
			foreach (Vector3 target in interpolateTargets)
			{
				var interpolateDirection = target - position;
				hitCount = Physics.RaycastNonAlloc(position, interpolateDirection, _hits, Length);
				if (hitCount != 0)
				{
					CheckHits(hitCount, position, interpolateDirection);
				}
			}
		}
	}
}
