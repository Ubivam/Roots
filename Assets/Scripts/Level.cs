using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class Level : ScriptableObject
{
	public List<Tile> Tiles;
	public int SideLength;
}
