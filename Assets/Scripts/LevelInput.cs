using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelInput")]
public class LevelInput : ScriptableObject
{
	[NonSerialized] public string LevelName;
	[NonSerialized] public Level Level;
	[NonSerialized] public Action OnLevelFinished;
}
