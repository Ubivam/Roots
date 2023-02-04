using UnityEngine;

public class Tile : MonoBehaviour
{
	public enum Side
	{
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3
	}

	// Represents connectivity for each side of a tile. Indexed with Side. Layout: [Up, Right, Down, Left]
	[SerializeField] private bool[] connectivity = new bool[4];

	public bool GetConnectivity(Side side)
	{
		var angle = (transform.localEulerAngles.z + 360f) % 360f;

		var startIndex = angle switch
		{
			0 => 0,
			90 => 1,
			180 => 2,
			270 => 3
		};

		var index = (startIndex + (int)side + connectivity.Length) % connectivity.Length;

		return connectivity[index];
	}
}
