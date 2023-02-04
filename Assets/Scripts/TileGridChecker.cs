using UnityEngine;

public class TileGridChecker : MonoBehaviour
{
	public Tile[,] grid;
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

		Tile firstTile = grid[firstPos.x, firstPos.y];
		Tile secondTile = grid[secondPos.x, secondPos.y];

		if (horizontalDist == 1)
		{
			if (firstPos.x < secondPos.x)
			{
				return firstTile.GetConnectivity(Tile.Side.Right, 0) && secondTile.GetConnectivity(Tile.Side.Left, 0);
			}
			else
			{
				return firstTile.GetConnectivity(Tile.Side.Left, 0) && secondTile.GetConnectivity(Tile.Side.Right, 0);
			}
		}
		else
		{
			if (firstPos.y < secondPos.y)
			{
				return firstTile.GetConnectivity(Tile.Side.Down, 0) && secondTile.GetConnectivity(Tile.Side.Up, 0);
			}
			else
			{
				return firstTile.GetConnectivity(Tile.Side.Up, 0) && secondTile.GetConnectivity(Tile.Side.Down, 0);
			}

		}
	}
}
