using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Unit : MonoBehaviour
{
	public string unitName;

	public int minDamage;
	public int maxDamage;

	public int maxHP;
	public int currentHP;

	private Vector3[] path;
	private int targetIndex;
	private float speed = 20.0f;

	private void Start()
	{
		Grid grid = FindObjectOfType<Grid>();

		Node node = grid.GetNodeFromWorldPoint(transform.position);
		transform.position = new Vector3(node.worldPosition.x, transform.position.y, node.worldPosition.z);
	}

	public bool TakeDamage(int damage)
	{
		currentHP -= damage;

		if (currentHP <= 0)
		{
			return true;
		}

		return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;

		if (currentHP >= maxHP)
		{
			currentHP = maxHP;
		}
	}

	public int GetDamage()
	{
		int damage = Random.Range(minDamage, maxDamage);
		return damage;
	}

	public void StartMove(Vector3 targetPosition)
	{
		PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
	}

	private void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful)
		{
			path = newPath;

			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	private IEnumerator FollowPath()
	{
		if (path.Length > 0)
		{
			Vector3 currentWaypoint = new Vector3(path[0].x, transform.position.y, path[0].z);

			while (true)
			{
				if (transform.position == currentWaypoint)
				{
					targetIndex++;
					if (targetIndex >= path.Length)
					{
						targetIndex = 0;
						yield break;
					}
					currentWaypoint = new Vector3(path[targetIndex].x, transform.position.y, path[targetIndex].z);
				}
				transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
				yield return null;
			}
		}
	}
}
