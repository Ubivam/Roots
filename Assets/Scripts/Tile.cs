using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tile"), Serializable]
public class Tile : ScriptableObject
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
	[SerializeField] private Sprite sprite;

	public GameObject InstantiateTile(TileComponent tilePrefab)
	{
		var tile = Instantiate(tilePrefab);
		tile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
		return tile.gameObject;
	}
	
	public bool GetConnectivity(Side side, float orientation)
	{
		var angle = (orientation + 360f) % 360f;

		var startIndex = angle switch
		{
			0 => 0,
			90 => 1,
			180 => 2,
			270 => 3,
			_ => 0
		};

		var index = (startIndex + (int)side + connectivity.Length) % connectivity.Length;

		return connectivity[index];
	}
}
