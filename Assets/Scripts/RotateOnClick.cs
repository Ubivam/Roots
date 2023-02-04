using UnityEngine;
using System.Collections;

public class RotateOnClick : MonoBehaviour
{
	[SerializeField] private float rotationAngle = -90f; // degrees
	[SerializeField] private float rotationTime = 0.5f; // seconds

	private Camera mainCamera;
	private bool isRotating;
	private Quaternion targetRotation;
	private bool queuedRotation;

	private void Start()
	{
		mainCamera = FindObjectOfType<Camera>();
		targetRotation = Quaternion.identity;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

			if (boxCollider.OverlapPoint(mousePos))
			{
				if (!isRotating && !queuedRotation)
				{
					targetRotation *= Quaternion.Euler(0, 0, rotationAngle);
					StartCoroutine(Rotate());
				}
				else
				{
					queuedRotation = true;
				}
			}
		}
	}

	IEnumerator Rotate()
	{
		isRotating = true;
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = targetRotation;
		float startTime = Time.time;
		float endTime = startTime + rotationTime;

		while (Time.time < endTime)
		{
			float t = (Time.time - startTime) / rotationTime;
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
			yield return null;
		}

		transform.rotation = endRotation;
		isRotating = false;

		if (queuedRotation)
		{
			queuedRotation = false;
			targetRotation *= Quaternion.Euler(0, 0, rotationAngle);
			StartCoroutine(Rotate());
		}
	}
}
