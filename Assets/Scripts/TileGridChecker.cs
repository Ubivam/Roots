using UnityEngine;

public class TileGridChecker : MonoBehaviour
{
	public TileComponent[,] grid;
	private Vector2Int sourcePos;
	private Vector2Int destPos;

	private bool AreTilesConnected(Vector2Int firstPos, Vector2Int secondPos)
	{
		var horizontalDist = Mathf.Abs(firstPos.x - secondPos.x);
		var verticalDist = Mathf.Abs(firstPos.y - secondPos.y);

		if (horizontalDist > 1 || verticalDist > 1 || (horizontalDist == 1 && verticalDist == 1))
		{
			return false;
		}

		var firstTile = grid[firstPos.x, firstPos.y];
		var secondTile = grid[secondPos.x, secondPos.y];

		if (horizontalDist == 1)
		{
			if (firstPos.x < secondPos.x)
			{
				return firstTile.GetConnectivity(Tile.Side.Right) && secondTile.GetConnectivity(Tile.Side.Left);
			}
			else
			{
				return firstTile.GetConnectivity(Tile.Side.Left) && secondTile.GetConnectivity(Tile.Side.Right);
			}
		}
		else
		{
			if (firstPos.y < secondPos.y)
			{
				return firstTile.GetConnectivity(Tile.Side.Down) && secondTile.GetConnectivity(Tile.Side.Up);
			}
			else
			{
				return firstTile.GetConnectivity(Tile.Side.Up) && secondTile.GetConnectivity(Tile.Side.Down);
			}

		}
	}
	
	public void LogConnectedTiles()
	{
		for (int x = 0; x < grid.GetLength(0); x++)
		{
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				Vector2Int currentPos = new Vector2Int(x, y);
				Vector2Int[] neighboringPositions = { new Vector2Int(x - 1, y), new Vector2Int(x + 1, y), new Vector2Int(x, y - 1), new Vector2Int(x, y + 1) };
				foreach (Vector2Int neighborPos in neighboringPositions)
				{
					if (neighborPos.x >= 0 && neighborPos.x < grid.GetLength(0) && neighborPos.y >= 0 && neighborPos.y < grid.GetLength(1) && AreTilesConnected(currentPos, neighborPos))
					{
						Debug.Log("Tiles at " + currentPos + " and " + neighborPos + " are connected.");
					}
				}
			}
		}
	}
}
