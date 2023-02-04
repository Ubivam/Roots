using UnityEngine;

public class LevelController : MonoBehaviour
{
	[SerializeField] private Level level;
	
	private Camera mainCamera;
	private LayerMask layerMask;
	
	private void Start()
	{
		mainCamera = Camera.main;
		layerMask = LayerMask.GetMask($"Tiles");
		var levelObject = level.InstantiateLevel();
		levelObject.transform.parent = transform;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out var hit, 1000f, layerMask ))
			{
				var tile = hit.transform.parent.GetComponent<TileComponent>();
				tile.Click();
			}
		}
	}
}
