using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
	public Tile tile;

	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;

	public Node parent;

	private int heapIndex;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, Tile tile)
	{
		this.walkable = walkable;
		this.worldPosition = worldPosition;
		this.gridX = gridX;
		this.gridY = gridY;
		this.tile = tile;
	}

	public int fCost
	{
		get { return gCost + hCost; }
	}

	public int HeapIndex { get => heapIndex; set => heapIndex = value; }

	public int CompareTo(Node other)
	{
		int compare = fCost.CompareTo(other.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(other.hCost);
		}
		return -compare;
	}
}
