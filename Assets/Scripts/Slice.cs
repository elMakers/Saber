using UnityEngine;

public class Slice : MonoBehaviour {
	// Properties
	public GameObject Sparks;
	public GameObject Source;
	public float Length = 0.5f;
	public float Width = 0.017f;
	public Renderer Blade;
	public AudioSource Sound;
	public float MinPitch = 0.5f;
	public float MaxPitch = 0.5f;
	public int ParticleQueueSize = 50;
	
	// Components
	public Extend _extend;
	public RaycastHit _hit;
	public Vector3 _extents;
	public GameObject[] _particleQueue;
	public int _particleQueueIndex;
	
	void Start()
	{
		_extend = GetComponent<Extend>();
		_extents = new Vector3(Width, Width, Width);
		_particleQueue = new GameObject[ParticleQueueSize];
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
			//Debug.Log("Hit : " + _hit.collider.name);
			//Debug.DrawLine(_hit.point, _hit.point + _hit.normal * Length, Color.red, 5.0f, false);
			Sound.pitch = Random.Range(MinPitch, MaxPitch);
			Sound.transform.position = _hit.point;
			Sound.PlayOneShot(Sound.clip);

			var particles = _particleQueue[_particleQueueIndex];
			if (particles == null)
			{
				particles = Instantiate(Sparks, _hit.point, Quaternion.LookRotation(_hit.normal));
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
				system.transform.rotation = Quaternion.LookRotation(_hit.normal);
				system.Play();
			}
			_particleQueueIndex = (_particleQueueIndex + 1) % ParticleQueueSize;
		}
	}
}
