using System.Collections;
using UnityEngine;
using TMPro;
using System;

public enum BattleState
{
	START,
	PLAYER_1_TURN,
	PLAYER_2_TURN,
	WAITING,
	PLAYER_1_WON,
	PLAYER_2_WON
}

public class GameplayManager : MonoBehaviour
{
	public const string PLAYER1_TAG = "Player 1";
	public const string PLAYER2_TAG = "Player 2";

	public BattleState state;

	[Header("Battle Component")]
	[SerializeField]
	private Transform heroPrefab;
	[SerializeField]
	private Transform enemyPrefab;
	[Space(10)]
	[SerializeField]
	private Transform playerBattleParent;
	[SerializeField]
	private Transform enemyBattleParent;

	[Header("Game UI")]
	[SerializeField]
	private GameUI gameUI;

	private Player player_1_Unit;
	private Player player_2_Unit;

	private Player selectedPlayer_1;
	private Player selectedPlayer_2;
	private Vector3 targetLocation;

	private void Start()
	{
		state = BattleState.START;

		SetupBattle();
	}

	private void SetupBattle()
	{
		Transform heroTransform = Instantiate(heroPrefab, playerBattleParent);
		player_1_Unit = heroTransform.GetComponent<Player>();
		player_1_Unit.battleHUD = gameUI.GetBattleHUD_Player1();

		Transform enemyTransform = Instantiate(enemyPrefab, enemyBattleParent);
		player_2_Unit = enemyTransform.GetComponent<Player>();
		player_2_Unit.battleHUD = gameUI.GetBattleHUD_Player2();

		gameUI.SetDialogText($"WAIT FOR IT...");
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButtonDown(0))
		{
			if (state == BattleState.START)
			{
				gameUI.StartBattle();

				Player_1_Turn();
				return;
			}
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				if (state == BattleState.PLAYER_1_TURN)
				{
					Player_1_Action(hit);
				}
				else if (state == BattleState.PLAYER_2_TURN)
				{
					Player_2_Action(hit);
				}
			}
		}

		if (state == BattleState.PLAYER_1_TURN)
		{
			SelectedTileToMove(ray, selectedPlayer_1);
		}
		else if (state == BattleState.PLAYER_2_TURN)
		{
			SelectedTileToMove(ray, selectedPlayer_2);
		}

	}

	private void SelectedTileToMove(Ray ray, Player unit)
	{
		if (unit != null)
		{
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Grid grid = FindObjectOfType<Grid>();

				if (Vector3.Distance(hit.transform.position, unit.transform.position) < 5.0f)
				{
					grid.ClearTracePath();
					Node node = grid.GetNodeFromWorldPoint(hit.transform.position);
					Color imageColor;
					if (unit == selectedPlayer_1)
					{
						if (hit.collider.CompareTag(PLAYER2_TAG))
						{
							imageColor = Color.yellow;
							node.tile.ShowImage(imageColor);

							return;
						}
					}
					else if (unit == selectedPlayer_2)
					{
						if (hit.collider.CompareTag(PLAYER1_TAG))
						{
							imageColor = Color.yellow;
							node.tile.ShowImage(imageColor);

							return;
						}
					}

					imageColor = node.walkable ? Color.green : Color.red;
					node.tile.ShowImage(imageColor);

					targetLocation = node.worldPosition;
				}
			}
		}
	}

	private void Player_1_Action(RaycastHit hit)
	{
		if (selectedPlayer_1 == null)
		{
			if (hit.collider.CompareTag(PLAYER1_TAG))
			{
				selectedPlayer_1 = hit.collider.GetComponent<Player>();
			}
		}
		else
		{
			if (hit.collider.CompareTag(PLAYER2_TAG))
			{
				if (Vector3.Distance(hit.transform.position, selectedPlayer_1.transform.position) < 5.0f)
				{
					Grid grid = FindObjectOfType<Grid>();
					grid.ClearTracePath();

					selectedPlayer_1.transform.LookAt(hit.transform.position);

					StartCoroutine(Player_1_Attack());

					selectedPlayer_1 = null;
					return;
				}
			}

			selectedPlayer_1.StartMove(targetLocation);
			selectedPlayer_1 = null;

			Player_2_Turn();
		}
	}

	private void Player_2_Action(RaycastHit hit)
	{
		if (selectedPlayer_2 == null)
		{
			if (hit.collider.CompareTag(PLAYER2_TAG))
			{
				selectedPlayer_2 = hit.collider.GetComponent<Player>();
			}
		}
		else
		{
			if (hit.collider.CompareTag(PLAYER1_TAG))
			{
				if (Vector3.Distance(hit.transform.position, selectedPlayer_2.transform.position) < 5.0f)
				{
					Grid grid = FindObjectOfType<Grid>();
					grid.ClearTracePath();

					selectedPlayer_2.transform.LookAt(hit.transform.position);

					StartCoroutine(Player_2_Attack());
					selectedPlayer_2 = null;
					return;
				}
			}
			selectedPlayer_2.StartMove(targetLocation);
			selectedPlayer_2 = null;

			Player_1_Turn();
		}
	}

	private void Player_1_Turn()
	{
		state = BattleState.PLAYER_1_TURN;
		gameUI.SetDialogText($"{player_1_Unit.unitName} CHOOSE AN ACTION: ");
	}

	private void Player_2_Turn()
	{
		state = BattleState.PLAYER_2_TURN;
		gameUI.SetDialogText($"{player_2_Unit.unitName} CHOOSE AN ACTION: ");
	}

	private IEnumerator Player_1_Attack()
	{
		int damage = player_1_Unit.GetDamage();
		bool isDead = player_2_Unit.TakeDamage(damage);
		DamagePopup.Create(player_2_Unit.transform.position, damage, false);
		gameUI.SetDialogText($"{player_1_Unit.unitName} THE ATTACK IS \n SUCCESFUL! ");

		state = BattleState.WAITING;

		yield return new WaitForSeconds(2f);

		// Check if the enemy is dead
		// Change state based on what happened
		if (isDead)
		{
			// End Battle
			state = BattleState.PLAYER_1_WON;
			StartCoroutine(EndBattle());
		}
		else
		{
			// Player 2 Turn
			Player_2_Turn();
		}
	}

	private IEnumerator Player_2_Attack()
	{
		int damage = player_2_Unit.GetDamage();
		bool isDead = player_1_Unit.TakeDamage(damage);
		DamagePopup.Create(player_1_Unit.transform.position, damage, false);
		gameUI.SetDialogText($"{player_2_Unit.unitName} THE ATTACK IS \n SUCCESFUL! ");

		state = BattleState.WAITING;

		yield return new WaitForSeconds(2f);

		// Check if the player 1 is dead
		// Change state based on what happened
		if (isDead)
		{
			// End Battle
			state = BattleState.PLAYER_2_WON;
			StartCoroutine(EndBattle());
		}
		else
		{
			// Player 1 Turn
			Player_1_Turn();
		}
	}


	private IEnumerator Player_1_Heal()
	{
		player_1_Unit.Heal(5);

		gameUI.SetDialogText($"{player_1_Unit.unitName} FEEL RENEWED \n STRENGTH!");

		state = BattleState.WAITING;

		yield return new WaitForSeconds(2f);

		Player_2_Turn();
	}

	private IEnumerator Player_2_Heal()
	{
		player_2_Unit.Heal(5);

		gameUI.SetDialogText($"{player_2_Unit.unitName} FEEL RENEWED \n STRENGTH!");

		state = BattleState.WAITING;

		yield return new WaitForSeconds(2f);

		Player_1_Turn();
	}

	private IEnumerator EndBattle()
	{
		string text = "";
		if (state == BattleState.PLAYER_1_WON)
		{
			text = $"{player_1_Unit.unitName} WON THE BATTLE!";
		}
		else if (state == BattleState.PLAYER_2_WON)
		{
			text = $"{player_2_Unit.unitName} WON THE BATTLE!";
		}

		gameUI.SetDialogText(text);

		yield return new WaitForSeconds(1.0f);

		gameUI.EndBattle(text);
	}

	public void OnHealButton()
	{
		if (state == BattleState.PLAYER_1_TURN)
		{
			StartCoroutine(Player_1_Heal());
		}
		else if (state == BattleState.PLAYER_2_TURN)
		{
			StartCoroutine(Player_2_Heal());
		}
	}
}
