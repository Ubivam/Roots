using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEditor : MonoBehaviour
{
	[SerializeField] private int gridSize = 3;
	[SerializeField] private string levelName = "Rename me plz";
	[SerializeField] private float padding = 5f;

	public GameObject buttonPrefab;
	public TMP_Dropdown dropdown;

	private GridLayoutGroup gridLayouGroup;
	private Button[,] buttons;
	private Level level;

	void Start()
	{
		gridLayouGroup = GetComponent<GridLayoutGroup>();
		level = ScriptableObject.CreateInstance<Level>();

		gridLayouGroup.constraintCount = gridSize;
		gridLayouGroup.cellSize = new Vector2(Screen.width / (float)gridSize, Screen.height / (float)gridSize - padding);

		// Generate grid of buttons
		buttons = new Button[gridSize, gridSize];
		for (int i = 0; i < gridSize; i++)
		{
			for (int j = 0; j < gridSize; j++)
			{
				GameObject buttonObject = Instantiate(buttonPrefab);
				buttonObject.transform.SetParent(transform);

				Button button = buttonObject.GetComponent<Button>();

				int x = i;
				int y = j;
				button.onClick.AddListener(() => OnButtonClick(x, y));
				buttons[i, j] = button;
			}
		}
	}

	void OnButtonClick(int x, int y)
	{
		Button button = buttons[x, y];

		// Show dropdown list
		dropdown.gameObject.SetActive(true);

		dropdown.onValueChanged.AddListener(delegate {
			OnDropdownSelect(button);
		});

		// Set position of dropdown list
		RectTransform dropdownRect = dropdown.GetComponent<RectTransform>();
		Vector2 buttonPos = button.GetComponent<RectTransform>().anchoredPosition;
		dropdownRect.anchoredPosition = buttonPos;
	}

	void OnDropdownSelect(Button button)
	{
		button.image.sprite = dropdown.options[dropdown.value].image;

		// Hide dropdown list
		dropdown.onValueChanged.RemoveAllListeners();
		dropdown.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		AssetDatabase.CreateAsset(level, "Assets/Resources/Levels/" + levelName + ".asset");
	}
}
