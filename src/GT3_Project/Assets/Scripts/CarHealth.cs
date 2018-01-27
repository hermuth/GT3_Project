using UnityEngine;

public class CarHealth : MonoBehaviour
{
	public float maxForceToBreak;

	public float Durability { get; private set; }
	
	private void Start()
	{
		Durability = 100.0f;
	}

	private void OnCollisionEnter(Collision collision)
	{
		Durability -= 100.0f / maxForceToBreak * collision.impulse.magnitude;

		if (Durability < 0.0f)
			Durability = 0.0f;
	}
}
