using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class Level : ScriptableObject
{
	public List<LevelCell> Tiles;
	public int SideLength;
}

[Serializable]
public class LevelCell
{
	public Tile Tile;
	public float Rotation;
}
