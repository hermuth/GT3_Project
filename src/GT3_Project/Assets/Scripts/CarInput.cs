using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarInput : MonoBehaviour
{
	private CarManager carManager;
	private CarController carController;
	
	private void Awake()
	{
		carManager = GetComponent<CarManager>();
		carController = GetComponent<CarController>();
	}

	private void FixedUpdate()
	{
		//float h = Input.GetAxis("Horizontal");
		//float v = Input.GetAxis("Vertical");
		float h = RemoteController.instance.steerH;
		float v = RemoteController.instance.steerV;
		float handbrake = Input.GetAxis("Jump");
		
		if (carManager.ControllsEnabled)
			carController.Move(h, v, v, handbrake);
		else
			carController.Move(0.0f, 0.0f, 0.0f, handbrake);
	}
}
