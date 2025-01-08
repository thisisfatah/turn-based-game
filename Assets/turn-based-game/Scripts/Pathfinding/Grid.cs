using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	[SerializeField]
	private bool displayGridGizmoz = true;
	[SerializeField]
	private LayerMask unwalkableMask;
	[SerializeField]
	private Vector2 gridWorldSize;
	[SerializeField]
	private float nodeRadius;
	[SerializeField]
	private Transform tilePrefab;

	private Node[,] grid;

	private float nodeDiamater;
	private int gridSizeX;
	private int gridSizeY;

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}


	private void Awake()
	{
		nodeDiamater = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamater);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamater);

		CreateGrid();
	}

	private void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiamater + nodeRadius) + Vector3.forward * (y * nodeDiamater + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				Vector3 tilePosition =  new Vector3(worldPoint.x, worldPoint.y - 1, worldPoint.z);
				Transform tileTransform = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
				Tile tile = tileTransform.GetComponent<Tile>();
				grid[x, y] = new Node(walkable, worldPoint, x, y, tile);
			}
		}
	}

	public void ClearTracePath()
	{
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				grid[x, y].tile.HideImage();
			}
		}
	}

	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node GetNodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid[x, y];
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, nodeRadius * 2, gridWorldSize.y));

		if (grid != null && displayGridGizmoz)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable ? Color.white : Color.red);
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiamater - 0.1f));
			}
		}
	}
}
