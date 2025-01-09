using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
	[Header("UI Component")]
	[SerializeField]
	private TextMeshProUGUI dialogueText;
	[SerializeField]
	private TextMeshProUGUI gameoverText;
	[SerializeField]
	private BattleHUD player_1_HUD;
	[SerializeField]
	private BattleHUD player_2_HUD;
	[SerializeField]
	private GameObject gameplayPanel;
	[SerializeField]
	private GameObject tutorialPanel;
	[SerializeField]
	private GameObject gameOverPanel;

	public void SetDialogText(string text)
	{
		dialogueText.text = text;
	}

	public void StartBattle()
	{
		gameplayPanel.SetActive(true);
		tutorialPanel.SetActive(false);
		gameOverPanel.SetActive(false);
	}

	public void EndBattle(string text)
	{
		gameplayPanel.SetActive(false);
		gameOverPanel.SetActive(true);

		gameoverText.text = "GAME OVER \n" + text;
	}

	public BattleHUD GetBattleHUD_Player1()
	{
		return player_1_HUD;
	}

	public BattleHUD GetBattleHUD_Player2()
	{
		return player_2_HUD;
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
