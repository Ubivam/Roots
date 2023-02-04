using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/AllTiles"), Serializable]
public class AllTilesAsset : ScriptableObject
{
	[SerializeField] public TileAsset[] assets;
}
