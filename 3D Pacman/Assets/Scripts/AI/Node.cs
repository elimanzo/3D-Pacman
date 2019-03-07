using UnityEngine;

/* 
	Node class for the PacMan game. 
	The level class implements a multi-dimensional array of these bad-boys for the pathfinding. 
	Over-simplification in exchange of performance done intentionally for educational purposes.
	Do NOT optimize. 

	Last Updated Feb 9th 2017. 
*/

public class Node : IHeapItem<Node> {

	public bool m_walkable; 
	public Vector3 m_worldPos;
	public int m_gridX; 
	public int m_gridZ;

	public int m_gCost; 
	public int m_hCost;
	public Node m_parent;
	
	private int m_heapIndex;

	public Node(bool walkable, Vector3 worldPos, int gridZ, int gridX) {
		m_walkable = walkable;
		m_worldPos = worldPos;
		m_gridX = gridX;
		m_gridZ = gridZ;
	}

	public int m_fCost {
		get { return m_gCost + m_hCost; }
	}

	public int HeapIndex {
		get { return m_heapIndex; }
		set { m_heapIndex = value; }
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = m_fCost.CompareTo(nodeToCompare.m_fCost); 
		if (compare == 0) {
			compare = m_hCost.CompareTo(nodeToCompare.m_hCost);
		}
		return -compare;
	}
}
