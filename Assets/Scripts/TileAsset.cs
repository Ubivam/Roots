using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tile"), Serializable]
public class TileAsset : ScriptableObject
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

	[SerializeField] private bool isRoot;
	[SerializeField] private bool isPond;

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
			90 => 3,
			180 => 2,
			270 => 1,
			_ => -1
		};

		if (startIndex == -1)
			return false;

		var index = (startIndex + (int)side + connectivity.Length) % connectivity.Length;

		return connectivity[index];
	}
}
