using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelController : MonoBehaviour
{
	[SerializeField] private LevelInput levelInput;
	[SerializeField] private MainMenu _mainMenu;
	[SerializeField] private AudioSource _audioSource;
	[FormerlySerializedAs("level")] [SerializeField] private Level backupLevel;
	private Level Level => levelInput.Level != null ? levelInput.Level : backupLevel;
	
	private Camera mainCamera;
	private LayerMask layerMask;
	private List<TileComponent> tiles;
	private Queue<TileComponent> _rootedTiles;
	private List<TileComponent> _rootedTilesList;
	private bool _isEndGame;

	private string PlayerPrefsKey => $"{_mainMenu.GetAllLevels().name}_LastFinishedIndex";
	private void Start()
	{
		mainCamera = Camera.main;
		layerMask = LayerMask.GetMask($"Tiles");

		_audioSource = GetComponent<AudioSource>();

		tiles = Level.InstantiateLevel(transform);
		ReinitializeTiles();
	}

	private void Update()
	{
		HandleClick();
		
		if (tiles == null)
		{
			return;
		}
		ReinitializeTiles();
		foreach (var tile in tiles)
		{
			tile.gameObject.transform.GetChild(0).transform.gameObject.SetActive(false);
		}
		
		foreach (var (t0, t1) in GetConnectedTiles())
		{
			t0.gameObject.transform.GetChild(0).transform.gameObject.SetActive(true);
			t1.gameObject.transform.GetChild(0).transform.gameObject.SetActive(true);
			
		}
		foreach (var tree in Level.trees)
		{
			if (tree.isWatered)
			{
				tree.gameObject.transform.GetChild(0).transform.gameObject.SetActive(true);
			}
			else
			{
				tree.gameObject.transform.GetChild(0).transform.gameObject.SetActive(false);
			}
		}
		CheckEndGame();
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
			tile.isWatered = false;
		}
		_rootedTiles = new Queue<TileComponent>();
		_rootedTilesList = new List<TileComponent>();
		foreach (var tree in Level.trees)
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
		foreach (var tree in Level.trees)
		{
			tree.isUnderRoot = true;
		}
	}
	private void  ExpandRootTiles()
	{
		_isEndGame = true;
		foreach (var tree in Level.trees)
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

			if (IsPondRooted())
			{
				tree.isWatered = true;
			}
			_isEndGame &= IsPondRooted();
		}
	}

	private bool IsPondRooted()
	{
		bool flag = false;
		foreach (var pond in Level.ponds)
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
			var lastFinishedIndex = PlayerPrefs.GetInt(PlayerPrefsKey, -1);
			lastFinishedIndex++;
			PlayerPrefs.SetInt(PlayerPrefsKey, lastFinishedIndex);
			_mainMenu.OnPlayClicked();

			_audioSource.enabled = true;
			_audioSource.Play();
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
			if (p.x < 0 || p.x >= Level.Width)
				return false;
			if (p.y < 0 || p.y >= Level.Height)
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

	private TileComponent GetTile(Vector2Int pos)
	{
		var index = VectorToIndex(pos);
		return tiles[index];
	}

	private Vector2Int IndexToVector(int index) => new(index % Level.Width, index / Level.Width);

	private int VectorToIndex(Vector2Int pos) => Level.Width * pos.y + pos.x;
}
