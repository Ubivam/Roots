using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{
	private const int MAX_GRID_SIZE = 8;
	private const int DEFAULT_ASSET_ID = 0; // Straight is default

	[SerializeField] private GameObject tileButtonPrefab;

	[SerializeField] private TMP_Dropdown tileDropdownMenu;
	[SerializeField] private TMP_InputField levelNameInput;
	[SerializeField] private TMP_InputField gridSizeInput;

	[SerializeField] private Button saveButton;
	[SerializeField] private Button rotateButton;

	[SerializeField] private AllTilesAsset tiles;
	[SerializeField] private TileComponent tilePrefab;

	private GridLayoutGroup gridLayout;
	private Button[,] tileButtons;
	private Level level;

	private Button selectedTileButton;
	private int selectedTileButtonRow;
	private int selectedTileButtonColumn;
	private int selectedTileButtonAssetId;

	private void Start()
	{
		saveButton.onClick.AddListener(() => OnLevelSave());
		rotateButton.onClick.AddListener(() => OnTileRotate());

		var options = new List<TMP_Dropdown.OptionData>();

		foreach (var tileAsset in tiles.assets)
		{
			options.Add(new TMP_Dropdown.OptionData(tileAsset.name));
		}

		options.Add(new TMP_Dropdown.OptionData("NONE"));

		tileDropdownMenu.AddOptions(options);
	}

	private void Update()
	{
		int gridSize = 0;

		// Detect change in grid size
		if (tileButtons == null ||
			(int.TryParse(gridSizeInput.text, out gridSize) &&
			gridSize > 0 && gridSize <= MAX_GRID_SIZE &&
			tileButtons.GetLength(0) != gridSize))
		{
			tileButtons = new Button[gridSize, gridSize];
		}
		else
		{
			return;
		}

		ResetTileDropdown();

		// Cleanup
		selectedTileButton = null;
		selectedTileButtonRow = -1;
		selectedTileButtonColumn = -1;
		selectedTileButtonAssetId = -1;

		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		// Generate level
		gridLayout = GetComponent<GridLayoutGroup>();
		level = ScriptableObject.CreateInstance<Level>();
		level.Tiles = new List<LevelCell>();
		level.SideLength = gridSize;
		level.TilePrefab = tilePrefab;

		for (int i = 0; i < gridSize * gridSize; ++i)
		{
			var levelCell = new LevelCell();
			levelCell.Tile = tiles.assets[DEFAULT_ASSET_ID];
			levelCell.Rotation = 0f;
			level.Tiles.Add(levelCell);
		}

		// Update grid size
		gridLayout.constraintCount = gridSize;

		Vector2 cellSize = new Vector2(Screen.width / (float)gridSize, Screen.height / (float)gridSize);
		float minCellSide = Mathf.Min(cellSize.x, cellSize.y);
		gridLayout.cellSize = new Vector2(minCellSide, minCellSide);

		// Generate grid of buttons
		for (int i = 0; i < gridSize; i++)
		{
			for (int j = 0; j < gridSize; j++)
			{
				GameObject buttonObject = Instantiate(tileButtonPrefab);
				buttonObject.transform.SetParent(transform);

				Button button = buttonObject.GetComponent<Button>();

				int buttonRow = i;
				int buttonColumn = j;
				button.onClick.AddListener(() => OnButtonClick(buttonRow, buttonColumn));

				button.image.sprite = tiles.assets[DEFAULT_ASSET_ID].sprite;

				tileButtons[i, j] = button;
			}
		}
	}

	void OnButtonClick(int buttonRow, int buttonColumn)
	{
		Button tileButton = tileButtons[buttonRow, buttonColumn];
		selectedTileButtonAssetId = tileDropdownMenu.value;

		// Update selected button
		if (selectedTileButton)
		{
			selectedTileButton.GetComponent<Image>().color = Color.white;
		}

		selectedTileButton = tileButton;
		selectedTileButtonRow = buttonRow;
		selectedTileButtonColumn = buttonColumn;

		selectedTileButton.GetComponent<Image>().color = Color.red;

		// Show dropdown list
		ResetTileDropdown();

		tileDropdownMenu.gameObject.SetActive(true);
		tileDropdownMenu.onValueChanged.AddListener(delegate {
			OnDropdownSelect(tileButton);
		});

		// Set position of dropdown list
		RectTransform dropdownRect = tileDropdownMenu.GetComponent<RectTransform>();
		Vector2 buttonPos = tileButton.GetComponent<RectTransform>().anchoredPosition;
		dropdownRect.anchoredPosition = buttonPos;
	}

	void OnDropdownSelect(Button tileButton)
	{
		selectedTileButtonAssetId = tileDropdownMenu.value;
		tileButton.image.sprite = tiles.assets[selectedTileButtonAssetId].sprite;

		UpdateLevelTileCell();
		ResetTileDropdown();
	}

	private void OnLevelSave()
	{
		ResetTileDropdown();

		var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Levels/" + levelNameInput.text + ".asset");
		Debug.Log(uniqueFileName);

		AssetDatabase.CreateAsset(level, uniqueFileName);

		// Create another level
		var oldLevel = level;
		level = ScriptableObject.CreateInstance<Level>();
		level.Tiles = oldLevel.Tiles;
		level.SideLength = oldLevel.SideLength;
		level.TilePrefab = oldLevel.TilePrefab;
	}

	private void OnTileRotate()
	{
		if (!selectedTileButton)
		{
			return;
		}

		var rect = selectedTileButton.transform;
		rect.Rotate(new Vector3(0f, 0f, -90f));

		UpdateLevelTileCell();
		ResetTileDropdown();
	}

	private void ResetTileDropdown()
	{
		tileDropdownMenu.onValueChanged.RemoveAllListeners();
		tileDropdownMenu.gameObject.SetActive(false);
		tileDropdownMenu.value = 5; // Empty tile by default
	}

	private void UpdateLevelTileCell()
	{
		int gridSize = tileButtons.GetLength(0);
		int row = gridSize - selectedTileButtonRow - 1; // Compensate for flipped levels
		int column = selectedTileButtonColumn;
		int tileAssetIndex = gridSize * row + column;

		if (selectedTileButtonAssetId != 5)
		{
			level.Tiles[tileAssetIndex].Tile = tiles.assets[selectedTileButtonAssetId];
		}

		level.Tiles[tileAssetIndex].Rotation = 360f - tileButtons[selectedTileButtonRow, selectedTileButtonColumn].GetComponent<RectTransform>().localEulerAngles.z;
	}
}
