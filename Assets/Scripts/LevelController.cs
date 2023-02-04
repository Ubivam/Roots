using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	[SerializeField] private Level level;

	private void Start()
	{
		var levelObject = level.InstantiateLevel();
		levelObject.transform.parent = transform;
	}
}
