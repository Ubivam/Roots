using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TileComponent : MonoBehaviour
{
    [SerializeField] private float rotationAngle = -90f; // degrees
    [SerializeField] private float rotationTime = 0.5f; // seconds
	[SerializeField] private AnimationCurve rotationCurve;

    private bool isRotating;
    private Quaternion targetRotation = Quaternion.identity;
    private bool queuedRotation;
    private AudioSource audioSource;
    
    public bool isUnderRoot;
    public bool isWatered;

    public TileAsset Tile { get; private set; }
    public int Y { get; private set; }
    public int X { get; private set; }

    public Vector2Int Pos => new(X, Y);

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void Init(int x, int y, TileAsset tile)
    {
        X = x;
        Y = y;
        Tile = tile;
    }

    public void Click()
    {
        if (!isRotating && !queuedRotation)
        {
            targetRotation *= Quaternion.Euler(0, rotationAngle, 0);
            StartCoroutine(Rotate());
			audioSource.Play();
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
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationCurve.Evaluate(t));
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

    public bool GetConnectivity(TileAsset.Side side)
    {
        return Tile.GetConnectivity(side, transform.localRotation.eulerAngles.y);
    }

#if UNITY_EDITOR

    private static GUIStyle debugStyle;
    
    private void OnGUI()
    {
        debugStyle = new GUIStyle
        {
            normal = new GUIStyleState
            {
                textColor = Color.yellow
            }
        };
        
        Vector3 point = Camera.main.WorldToScreenPoint(transform.position);
        var rect = new Rect
        {
            x = point.x,
            y = Screen.height - point.y,
            height = 50,
            width = 300
        };
        GUI.Label(rect, name, debugStyle); // display its name, or other string
    }
#endif
}
