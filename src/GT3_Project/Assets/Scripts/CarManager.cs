using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarManager : MonoBehaviour
{
	public float Durability { get; private set; }
	public float Speed { get; private set; }
	public bool Finished { get; private set; }
	public bool ControllerConnected { get; set; }
	public bool ControllsEnabled { get; set; }

	private CarController carController;
	private CarHealth carHealth;

	private void Start()
	{
		Durability = 0.0f;
		Speed = 0.0f;

		ControllerConnected = false;
		ControllsEnabled = false;

		carController = GetComponent<CarController>();
		carHealth = GetComponent<CarHealth>();
	}

	private void Update()
	{
		Durability = carHealth.Durability;
		Speed = carController.CurrentSpeed;

		if (Durability <= 0.0f)
			ControllsEnabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Finishline")
		{
			Finished = true;
			ControllsEnabled = false;
		}
	}
}
