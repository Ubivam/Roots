using System.Collections;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    [SerializeField] private float rotationAngle = -90f; // degrees
    [SerializeField] private float rotationTime = 0.5f; // seconds

    private bool isRotating;
    private Quaternion targetRotation = Quaternion.identity;
    private bool queuedRotation;

    public void Click()
    {
        if (!isRotating && !queuedRotation)
        {
            targetRotation *= Quaternion.Euler(0, rotationAngle, 0);
            StartCoroutine(Rotate());
        }
        else
        {
            queuedRotation = true;
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
            targetRotation *= Quaternion.Euler(0, rotationAngle, 0);
            StartCoroutine(Rotate());
        }
    }
}
