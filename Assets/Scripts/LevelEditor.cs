using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEditor : MonoBehaviour
{
	private const int MAX_GRID_SIZE = 8;

	[SerializeField] private GameObject tileButtonPrefab;

	[SerializeField] private TMP_Dropdown tileDropdownMenu;
	[SerializeField] private TMP_InputField levelNameInput;
	[SerializeField] private TMP_InputField gridSizeInput;

	[SerializeField] private Button saveButton;
	[SerializeField] private Button rotateButton;

	private GridLayoutGroup gridLayout;
	private Button[,] tileButtons;
	private Level level;
	private Button selectedTileButton;

	private void Start()
	{
		saveButton.onClick.AddListener(() => OnLevelSave());
		rotateButton.onClick.AddListener(() => OnTileRotate());
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

		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		// Generate level
		gridLayout = GetComponent<GridLayoutGroup>();
		level = ScriptableObject.CreateInstance<Level>();

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

				tileButtons[i, j] = button;
			}
		}
	}

	void OnButtonClick(int buttonRow, int buttonColumn)
	{
		ResetTileDropdown();

		Button tileButton = tileButtons[buttonRow, buttonColumn];

		// Update selected button
		if (selectedTileButton)
		{
			selectedTileButton.GetComponent<Image>().color = Color.white;
		}

		selectedTileButton = tileButton;
		selectedTileButton.GetComponent<Image>().color = Color.red;

		// Show dropdown list
		tileDropdownMenu.gameObject.SetActive(true);
		tileDropdownMenu.onValueChanged.AddListener(delegate {
			OnDropdownSelect(tileButton);
		});

		// Set position of dropdown list
		RectTransform dropdownRect = tileDropdownMenu.GetComponent<RectTransform>();
		Vector2 buttonPos = tileButton.GetComponent<RectTransform>().anchoredPosition;
		dropdownRect.anchoredPosition = buttonPos;
	}

	void OnDropdownSelect(Button button)
	{
		button.image.sprite = tileDropdownMenu.options[tileDropdownMenu.value].image;

		ResetTileDropdown();
	}

	private void OnLevelSave()
	{
		var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Levels/" + levelNameInput.text + ".asset");
		AssetDatabase.CreateAsset(level, uniqueFileName);
	}

	private void OnTileRotate()
	{
		if (!selectedTileButton)
		{
			return;
		}

		var rect = selectedTileButton.transform;
		rect.Rotate(new Vector3(0f, 0f, -90f));
	}

	private void ResetTileDropdown()
	{
		tileDropdownMenu.value = 5; // Empty tile by default
		tileDropdownMenu.onValueChanged.RemoveAllListeners();
		tileDropdownMenu.gameObject.SetActive(false);
	}
}
