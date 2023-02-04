using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	[SerializeField] private Level level;
	
	private Camera mainCamera;
	private LayerMask layerMask;
	private List<TileComponent> tiles;
	private Queue<TileComponent> _rootedTiles;
	private List<TileComponent> _rootedTilesList;
	private bool _isEndGame;
	[SerializeField] private Vector2Int drawGizmosForIndex;

	private void Start()
	{
		mainCamera = Camera.main;
		layerMask = LayerMask.GetMask($"Tiles");
		tiles = level.InstantiateLevel(transform);
		ReinitializeTiles();
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
		}
	}

	private IEnumerable<(TileComponent, TileComponent)> GetConnectedTiles()
	{
		ExpandRootTiles();
		foreach (var root in _rootedTilesList)
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
			tile.isUnderRoot = false;
		}
		_rootedTiles = new Queue<TileComponent>();
		_rootedTilesList = new List<TileComponent>();
		foreach (var tree in level.trees)
		{
			tree.isUnderRoot = true;
		}
	}
	private void ClearTilesOnChangingToTheNextTree()
	{
		foreach (var tile in tiles)
		{
			tile.isUnderRoot = false;
		}
		_rootedTiles = new Queue<TileComponent>();
		foreach (var tree in level.trees)
		{
			tree.isUnderRoot = true;
		}
	}
	private void  ExpandRootTiles()
	{
		_isEndGame = true;
		foreach (var tree in level.trees)
		{
			ClearTilesOnChangingToTheNextTree();
			_rootedTiles.Enqueue(tree);
			_rootedTilesList.Add(tree);
			while (_rootedTiles.Count != 0)
			{
				var rooted = _rootedTiles.Dequeue();
				foreach (var neighbor in GetNeighbors(rooted.Pos))
				{
					if (AreTilesConnected(rooted.Pos, neighbor))
					{
						if (!tiles[VectorToIndex(neighbor)].isUnderRoot)
						{
							_rootedTilesList.Add(tiles[VectorToIndex(neighbor)]);
							_rootedTiles.Enqueue(tiles[VectorToIndex(neighbor)]);
							tiles[VectorToIndex(neighbor)].isUnderRoot = true;
						}
					}
				}
			}
			_isEndGame &= IsPondRooted();
		}
	}

	private bool IsPondRooted()
	{
		bool flag = false;
		foreach (var pond in level.ponds)
		{
			if (pond.isUnderRoot)
			{
				flag = true;
			}
		}

		return flag;
	}
	private void CheckEndGame()
	{
		if (_isEndGame)
		{
			Debug.Log("End Game!");
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
		CheckEndGame();
	}

	private TileComponent GetTile(Vector2Int pos)
	{
		var index = VectorToIndex(pos);
		return tiles[index];
	}

	private Vector2Int IndexToVector(int index) => new(index % level.Width, index / level.Width);

	private int VectorToIndex(Vector2Int pos) => level.Width * pos.y + pos.x;
}
