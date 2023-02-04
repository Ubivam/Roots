using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	[SerializeField] private Level level;
	
	private Camera mainCamera;
	private LayerMask layerMask;
	private List<TileComponent> tiles;
	private List<TileComponent> _rootedTiles;

	[SerializeField] private Vector2Int drawGizmosForIndex;

	private void Start()
	{
		mainCamera = Camera.main;
		layerMask = LayerMask.GetMask($"Tiles");
		tiles = level.InstantiateLevel(transform);
		_rootedTiles = new List<TileComponent>();
		foreach (var tree in level.trees)
		{
			tree.Tile.IsUnderRoot = true;
			_rootedTiles.Add(tree);
		}
	}

	private void Update()
	{
		HandleClick();
	}

	private void HandleClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out var hit, 1000f, layerMask))
			{
				var tile = hit.transform.parent.GetComponent<TileComponent>();
				tile.Click();
			}
			ReinitializeTiles();
		}
	}

	private IEnumerable<(TileComponent, TileComponent)> GetConnectedTiles()
	{
		GetNeighborsFromTrees();
		foreach (var root in _rootedTiles)
		{
			foreach (var neighbor in GetNeighbors(root.Pos))
			{
				if (AreTilesConnected(root.Pos, neighbor))
				{
					yield return (root, tiles[VectorToIndex(neighbor)]);
				}
			}
		}
	}

	private void ReinitializeTiles()
	{
		foreach (var tile in tiles)
		{
			tile.Tile.IsUnderRoot = false;
		}
		_rootedTiles = new List<TileComponent>();
		foreach (var tree in level.trees)
		{
			tree.Tile.IsUnderRoot = true;
			_rootedTiles.Add(tree);
		}
	}
	private void  GetNeighborsFromTrees()
	{
		for (int i = 0; i < _rootedTiles.Count; i++)
		{
			var rooted = _rootedTiles[i];
			foreach (var neighbor in GetNeighbors(rooted.Pos))
			{
				if (AreTilesConnected(rooted.Pos, neighbor))
				{
					if (!tiles[VectorToIndex(neighbor)].Tile.IsUnderRoot)
					{
						_rootedTiles.Add(tiles[VectorToIndex(neighbor)]);
						tiles[VectorToIndex(neighbor)].Tile.IsUnderRoot = true;
					}
				}
			}	
		}
	}
	

	private IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
	{
		var x = pos.x;
		var y = pos.y;
		
		Vector2Int left = new(x - 1, y);
		if (IsValid(left))
			yield return left;
		Vector2Int right = new(x + 1, y);
		if (IsValid(right))
			yield return right;
		Vector2Int up = new(x, y + 1);
		if (IsValid(up))
			yield return up;
		Vector2Int down = new(x, y - 1);
		if (IsValid(down))
			yield return down;

		bool IsValid(Vector2Int p)
		{
			if (p.x < 0 || p.x >= level.Width)
				return false;
			if (p.y < 0 || p.y >= level.Height)
				return false;
			var i = VectorToIndex(p);
			return i >= 0 && i < tiles.Count && (pos - p).sqrMagnitude == 1f;
		}
	}

	private bool AreTilesConnected(Vector2Int firstPos, Vector2Int secondPos)
	{
		var horizontalDist = Mathf.Abs(firstPos.x - secondPos.x);
		var verticalDist = Mathf.Abs(firstPos.y - secondPos.y);

		if (horizontalDist > 1 || verticalDist > 1 || (horizontalDist == 1 && verticalDist == 1))
		{
			return false;
		}

		var firstTile = GetTile(firstPos);
		var secondTile = GetTile(secondPos);

		if (horizontalDist == 1)
		{
			if (firstPos.x < secondPos.x)
			{
				return firstTile.GetConnectivity(TileAsset.Side.Right) && secondTile.GetConnectivity(TileAsset.Side.Left);
			}
			else
			{
				return firstTile.GetConnectivity(TileAsset.Side.Left) && secondTile.GetConnectivity(TileAsset.Side.Right);
			}
		}
		else
		{
			if (firstPos.y < secondPos.y)
			{
				return firstTile.GetConnectivity(TileAsset.Side.Up) && secondTile.GetConnectivity(TileAsset.Side.Down);
			}
			else
			{
				return firstTile.GetConnectivity(TileAsset.Side.Down) && secondTile.GetConnectivity(TileAsset.Side.Up);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (tiles == null)
		{
			return;
		}
		
		foreach (var (t0, t1) in GetConnectedTiles())
		{
			Gizmos.color = Color.red;
			var offset = new Vector3(0, 0.1f, 0);
			Gizmos.DrawLine(t0.transform.position + offset, t1.transform.position + offset);
		}
	}

	private TileComponent GetTile(Vector2Int pos)
	{
		var index = VectorToIndex(pos);
		return tiles[index];
	}

	private Vector2Int IndexToVector(int index) => new(index % level.Width, index / level.Width);

	private int VectorToIndex(Vector2Int pos) => level.Width * pos.y + pos.x;
}
