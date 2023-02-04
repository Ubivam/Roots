using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class Level : ScriptableObject
{
	public List<LevelCell> Tiles;
	public int SideLength;
	public TileComponent TilePrefab;
	
	public List<TileComponent> trees;
	public List<TileComponent> ponds;

	public int Width => SideLength;
	public int Height => Tiles.Count / Width;

	public List<TileComponent> InstantiateLevel(Transform parent)
	{
		var width = SideLength;
		var height = Tiles.Count / SideLength;

		if (width * height != Tiles.Count)
		{
			Debug.LogError("Wrong dimensions");
			return new List<TileComponent>();
		}

		var tiles = new List<TileComponent>();
		trees = new List<TileComponent>();
		ponds = new List<TileComponent>();
		
		for (int index = 0; index < Tiles.Count; index++)
		{
			var cell = Tiles[index];
			var x = index % width;
			var y = index / width;
			
			var tileGameObject = cell.Tile.InstantiateTile(TilePrefab);
			tileGameObject.name = $"[{x}, {y}]";
			tileGameObject.transform.parent = parent;
			var deltaPosition = new Vector3(x - height / 2f + 0.5f, 0.05f, y - width / 2f + 0.5f);
			deltaPosition.x *= tileGameObject.transform.lossyScale.x;
			deltaPosition.z *= tileGameObject.transform.lossyScale.z;
			tileGameObject.transform.localPosition = deltaPosition;  
			tileGameObject.transform.localEulerAngles = new Vector3(0, cell.Rotation, 0);
			var tileComponent = tileGameObject.GetComponent<TileComponent>();
			tileComponent.Init(x, y, cell.Tile);
			tiles.Add(tileComponent);
			tileComponent.Tile.IsUnderRoot = false;
			if (tileComponent.Tile.isTree)
			{
				trees.Add(tileComponent);
			}

			if (tileComponent.Tile.isPond)
			{
				ponds.Add(tileComponent);
			}
		}

		return tiles;
	}
}

[Serializable]
public class LevelCell
{
	public TileAsset Tile;
	public float Rotation;
}
