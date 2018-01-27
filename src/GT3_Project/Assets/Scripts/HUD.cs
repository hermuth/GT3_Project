using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	public CarManager carManager;
	public RemoteController remoteController;

	public Text speedText;
	public Text urlText;
	public Text infoText;

	public RectTransform durabilityBackground;
	public RectTransform durabilityForeground;
	public Text durabilityText;

	private bool countDownPlayed;
	private bool destroyedTextDisplayed;
	private bool finishTextDisplayed;

	private void Start()
	{
		urlText.text = "";
		infoText.text = "Connect a Smartphone to start";

		foreach (string url in remoteController.httpUrls)
			urlText.text += url + "\n";

		countDownPlayed = false;
		destroyedTextDisplayed = false;
		finishTextDisplayed = false;
	}

	private void Update()
	{
		if (!countDownPlayed && carManager.ControllerConnected)
			StartCoroutine(CountDownCoroutine());

		if (!destroyedTextDisplayed && carManager.Durability <= 0.0f)
		{
			infoText.text = "You took to much damage.";
			destroyedTextDisplayed = true;
		}

		if (!finishTextDisplayed && carManager.Finished)
		{
			infoText.text = "Congratulations! You reached the Finish with";
			infoText.text += "\n " + carManager.Durability.ToString("0") + "% durability.";
			finishTextDisplayed = true;
		}

		speedText.text = carManager.Speed.ToString("0") + " km/h";
		durabilityText.text = carManager.Durability.ToString("0") + " %";

		durabilityForeground.sizeDelta = new Vector2(
			durabilityBackground.sizeDelta.x * carManager.Durability / 100.0f,
			durabilityBackground.sizeDelta.y
		);
	}

	private IEnumerator CountDownCoroutine()
	{
		countDownPlayed = true;

		infoText.text = "3";
		infoText.fontSize = 30;

		for (int i = 0; i < 10; ++i)
		{
			yield return new WaitForSecondsRealtime(0.1f);
			infoText.fontSize += 2;
		}

		infoText.text = "2";
		infoText.fontSize = 30;

		for (int i = 0; i < 10; ++i)
		{
			yield return new WaitForSecondsRealtime(0.1f);
			infoText.fontSize += 2;
		}

		infoText.text = "1";
		infoText.fontSize = 30;

		for (int i = 0; i < 10; ++i)
		{
			yield return new WaitForSecondsRealtime(0.1f);
			infoText.fontSize += 2;
		}

		carManager.ControllsEnabled = true;
		infoText.text = "GO!";
		infoText.fontSize = 30;

		for (int i = 0; i < 10; ++i)
			yield return new WaitForSecondsRealtime(0.1f);

		infoText.text = "";

		yield return null;
	}
}
