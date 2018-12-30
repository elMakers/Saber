using UnityEngine;

public class Slice : MonoBehaviour {
	// Properties
	public GameObject Sparks;
	public Extend Extend;
	
	void Start () 
	{
	}
	
	void Update ()
	{
		if (!Extend.Extended()) return;
		
		
	}

	void OnTriggerEnter(Collider collider)
	{
		Debug.Log("Triggered");
	}

	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collided");
		if (!Extend.Extended()) return;
		
		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal, Color.white);
		}
	}
}
