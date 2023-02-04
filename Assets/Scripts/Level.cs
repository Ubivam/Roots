using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class Level : ScriptableObject
{
	public List<LevelCell> Tiles;
	public int SideLength;

	public GameObject InstantiateLevel()
	{
		var gameObject = new GameObject($"Level {name}");

		var width = SideLength;
		var height = Tiles.Count / SideLength;

		if (width * height != Tiles.Count)
		{
			Debug.LogError("Wrong dimensions");
			return gameObject;
		}
		
		for (int index = 0; index < Tiles.Count; index++)
		{
			var cell = Tiles[index];
			var row = index % width;
			var column = index / width;
			
			var tile = cell.Tile.InstantiateTile();
			tile.name += $" [{column}, {row}]";
			tile.transform.parent = gameObject.transform;
			var deltaPosition = new Vector3(row - height / 2f + 0.5f, 0.05f, column - width / 2f + 0.5f);
			deltaPosition.x *= tile.transform.lossyScale.x;
			deltaPosition.z *= tile.transform.lossyScale.z;
			tile.transform.localPosition = deltaPosition;  
			tile.transform.localEulerAngles = new Vector3(90, cell.Rotation, 0);
		}

		return gameObject;
	}
}

[Serializable]
public class LevelCell
{
	public Tile Tile;
	public float Rotation;
}
