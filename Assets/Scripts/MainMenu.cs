using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AllLevels levels;
    [SerializeField] private LevelInput levelInput;
    [SerializeField] private bool resetToFirstLevel;
    private string PlayerPrefsKey => $"{levels.name}_LastFinishedIndex";

    public void OnPlayClicked()
    {
        if (resetToFirstLevel)
        {
            PlayerPrefs.SetInt(PlayerPrefsKey,0);
        }
        var lastFinishedIndex = PlayerPrefs.GetInt(PlayerPrefsKey, -1);
        var levelToPlayIndex = lastFinishedIndex;


        if (levelToPlayIndex >= 0)
        {
			levelToPlayIndex %= levels.Levels.Count;

			levelInput.LevelName = $"Level {levelToPlayIndex + 1}";
            levelInput.Level = levels.Levels[levelToPlayIndex];
            levelInput.OnLevelFinished = () =>
            {
                PlayerPrefs.SetInt(PlayerPrefsKey, levelToPlayIndex);
                PlayerPrefs.Save();
            };

            SceneManager.LoadScene("Game");
        }
    }

    public AllLevels GetAllLevels()
    {
        return levels;
    }
}