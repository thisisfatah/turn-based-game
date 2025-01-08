using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	private PathRequestManager pathRequestManager;
	private Grid grid;

	private void Awake()
	{
		grid = GetComponent<Grid>();

		pathRequestManager = GetComponent<PathRequestManager>();
	}

	public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
	{
		StartCoroutine(FindPath(pathStart, pathEnd));
	}

	private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		Vector3[] waypoints = new Vector3[0];
		bool pathSucces = false;

		Node startNode = grid.GetNodeFromWorldPoint(startPos);
		Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

		if (startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();

			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();

				closedSet.Add(currentNode);

				foreach (Node neigbour in grid.GetNeighbours(currentNode))
				{
					if (!neigbour.walkable || closedSet.Contains(neigbour))
					{
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neigbour);
					if (newMovementCostToNeighbour <= neigbour.gCost || !openSet.Contains(neigbour))
					{
						neigbour.gCost = newMovementCostToNeighbour;
						neigbour.hCost = GetDistance(neigbour, targetNode);
						neigbour.parent = currentNode;

						if (!openSet.Contains(neigbour))
						{
							openSet.Add(neigbour);
						}
					}
				}

				if (currentNode == targetNode)
				{

					stopwatch.Stop();
					print($"Path found: {stopwatch.ElapsedMilliseconds} ms");

					pathSucces = true;
					break;
				}
			}
		}

		yield return null;

		if (pathSucces)
		{
			waypoints = RetracePath(startNode, targetNode);
		}
		pathRequestManager.FinishedProccessingPath(waypoints, pathSucces);
	}

	private Vector3[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);

		grid.ClearTracePath();

		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();

		Vector2 directionOld = Vector2.zero;

		if (path.Count > 0)
		{
			waypoints.Add(path[0].worldPosition);

			for (int i = 1; i < path.Count; i++)
			{
				Vector2 newDirection = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
				if (newDirection != directionOld)
				{
					waypoints.Add(path[i].worldPosition);
				}
				directionOld = newDirection;
			}
		}

		return waypoints.ToArray();
	}

	private int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}

		return 14 * dstX + 10 * (dstY - dstX);
	}
}
