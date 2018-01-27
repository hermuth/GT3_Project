using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
	public GameObject startLine;
	public GameObject finishLine;
	public GameObject[] straights;
	public GameObject[] lefts;
	public GameObject[] rights;

	public float avarageSize;
	public int numberOfElements;

	private List<GameObject> trackParts;

	private Vector3 currentPosition;
	private Vector3 currentRotation;

	private bool lockLeft;
	private bool lockRight;

	private void Start()
	{
		trackParts = new List<GameObject>();

		currentPosition = Vector3.zero;
		currentRotation = Vector3.zero;

		lockLeft = false;
		lockRight = false;

		BuildTrack();

		float grassScaling = avarageSize * numberOfElements;
		transform.Find("Grass").transform.localScale = new Vector3(grassScaling, 1.0f, grassScaling);
	}

	private void BuildTrack()
	{
		for (int i = 0; i < numberOfElements;)
		{
			int random = Mathf.RoundToInt(Random.Range(1.0f, 3.0f));
			bool leftConflict = CheckForFutureConflictsWhenPlacingLeft();
			bool straightConflict = CheckForFutureConflictsWhenPlacingStraight();
			bool rightConflict = CheckForFutureConflictsWhenPlacingRight();

			if (i == 0)
			{
				AddTrackPart(startLine);
				UpdatePosition();
				++i;
			}
			else if (i == numberOfElements - 1)
			{
				AddTrackPart(finishLine);
				++i;
			}
			else if (random == 1 && !lockLeft && !leftConflict)
			{
				AddTrackPart(lefts[0]);
				currentRotation.y -= 90.0f;
				UpdatePosition();
				++i;
			}
			else if (random == 2 && !straightConflict)
			{
				AddTrackPart(straights[0]);
				UpdatePosition();
				++i;
			}
			else if (random == 3 && !lockRight && !rightConflict)
			{
				AddTrackPart(rights[0]);
				currentRotation.y += 90.0f;
				UpdatePosition();
				++i;
			}
			else if (DetectDeadlockForTrackBuild(leftConflict, straightConflict, rightConflict, lockLeft, lockRight))
			{
				AddTrackPart(finishLine);
				i = numberOfElements;
			}
		}
	}

	private bool DetectDeadlockForTrackBuild(bool leftConflict, bool straightConflict, bool rightConflict, bool lockLeft, bool lockRight)
	{
		if (leftConflict && straightConflict && rightConflict)
			return true;
		else if (straightConflict)
		{
			if (leftConflict && !rightConflict)
			{
				if (lockRight)
					return true;
			}
			else if (!leftConflict && rightConflict)
			{
				if (lockLeft)
					return true;
			}
		}

		return false;
	}

	private bool CheckForFutureConflictsWhenPlacingLeft()
	{
		Vector3 rotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z);
		rotation.y -= 90.0f;

		Vector3 position = CalculatePosition(rotation);
		
		return CheckForConflicts(position);
	}

	private bool CheckForFutureConflictsWhenPlacingStraight()
	{
		Vector3 position = CalculatePosition(currentRotation);

		return CheckForConflicts(position);
	}

	private bool CheckForFutureConflictsWhenPlacingRight()
	{
		Vector3 rotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z);
		rotation.y += 90.0f;

		Vector3 position = CalculatePosition(rotation);

		return CheckForConflicts(position);
	}

	private bool CheckForConflicts(Vector3 position)
	{
		foreach (GameObject gameObject in trackParts)
		{
			if (gameObject.transform.position == position)
				return true;
		}
		return false;
	}

	private void AddTrackPart(GameObject trackPart)
	{
		GameObject trackPartGameObject = Instantiate(trackPart, currentPosition, Quaternion.Euler(currentRotation), transform);
		SetLayerRecursive(trackPartGameObject, gameObject.layer);
		trackParts.Add(trackPartGameObject);
	}

	private void UpdatePosition()
	{
		lockLeft = false;
		lockRight = false;

		currentPosition = CalculatePosition(currentRotation);

		if (currentRotation.y == 180.0f)
			lockRight = true;
		else if (currentRotation.y == -180.0f)
			lockLeft = true;
	}

	private Vector3 CalculatePosition(Vector3 rotation)
	{
		Vector3 position = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z); ;

		if (rotation.y == 0.0f)
			position.x += avarageSize;
		else if (rotation.y == -90.0f)
			position.z += avarageSize;
		else if (rotation.y == 90.0f)
			position.z -= avarageSize;
		else if (rotation.y == 180.0f)
			position.x -= avarageSize;
		else if (rotation.y == -180.0f)
			position.x -= avarageSize;

		return position;
	}

	private void SetLayerRecursive(GameObject go, LayerMask layer)
	{
		if (go == null)
			return;

		go.layer = layer;

		foreach (Transform child in go.transform)
		{
			if (child == null)
				continue;

			SetLayerRecursive(child.gameObject, layer);
		}
	}
}
