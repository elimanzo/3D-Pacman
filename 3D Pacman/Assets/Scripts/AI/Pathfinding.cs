using UnityEngine; 
using System.Collections.Generic; 
/* 
    A* Pathfinding algorithm 

    OPEN    //  the set of nodes to be evaluated 
    CLOSED  // the set of nodes already evaluated 
    add the start node to OPEN 

    loop 
        current = node in OPEN with the lowese f_cost 
        remove current from OPEN 
        add current to CLOSED 

        if current is the target // path found 
            return 

        for each neighbour of the current node 
            if neighbour is not traversable or neighbour is in CLOSED 
                skip to the next neighbour 

            if new path to neighbour is shorter OR neighbour is not in OPEN 
                set f_cost of neighbour 
                set parent of neighbour to current 
                if neighbour is not in OPEN 
                    add neighbour to OPEN 

    --------- 
    NodeFromWorldPoint and GetNeighbours are in the Level 
*/ 


public class Pathfinding : MonoBehaviour {

    public bool showPath = true;
    
    private Level m_levelScript; 
    private Transform m_target;
    public List<Node> m_path; 
    private Transform m_seeker; 
    private Color m_seekerColor; 

    void Start() {
       m_seeker = transform;
       m_seekerColor = GetComponent<GhostController>().getColor();
    }

    public void setLevel(Level levelScript) { m_levelScript = levelScript; }
    public void setTarget(Transform newTarget) { m_target = newTarget; }
    
    void Update() {
        if (m_target != null) {
            FindPath(m_seeker.position, m_target.position);
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos){
        if (m_levelScript != null) {
            Node startNode = m_levelScript.NodeFromWorldPoint(startPos);
            Node targetNode = m_levelScript.NodeFromWorldPoint(targetPos);
            Heap<Node> openSet = new Heap<Node>(m_levelScript.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while(openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                if(currentNode == targetNode) {
                    RetracePath(startNode, targetNode);
                    return; //TODO: cleanup
                }
                foreach (Node neighbour in m_levelScript.GetNeighbours(currentNode)) {

                    if (!neighbour.m_walkable || closedSet.Contains(neighbour)) {
                        continue;
                    }
                    int newMovementCostToNeighbour = currentNode.m_gCost + GetDistance(currentNode, neighbour);
                    if(newMovementCostToNeighbour < neighbour.m_gCost || !openSet.Contains(neighbour)) {
                        neighbour.m_gCost = newMovementCostToNeighbour;
                        neighbour.m_hCost = GetDistance(neighbour, targetNode);
                        neighbour.m_parent = currentNode;
                        
                        if(!openSet.Contains(neighbour) && (neighbour.m_gCost - currentNode.m_gCost == 10)){ 
                            openSet.Add(neighbour);
                        }
                        // if you want diaganols 
                        // if(!openSet.Contains(neighbour)){ 
                        //     openSet.Add(neighbour);
                        // } else { 
                        //     openSet.UpdateItem(neighbour);
                        // }
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode; 

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.m_parent;
        }
        path.Reverse();
        m_path = path;
    }

    private int GetDistance(Node nodeA, Node nodeB) {
        int retVal = 0;
        int distX = Mathf.Abs(nodeA.m_gridX - nodeB.m_gridX);
        int distZ = Mathf.Abs(nodeA.m_gridZ - nodeB.m_gridZ);
        
        if (distX > distZ) {
            retVal = 14 * distZ + 10 * (distX - distZ);
        } else {
            retVal = 14 * distX + 10 * (distZ - distX);
        }

        return retVal;
    }

	void OnDrawGizmos() {
        Vector3 pathHeightAdjusment = Vector3.zero; 
        pathHeightAdjusment.y += 1;
        if(showPath) { 
	 		Node seekerNode = m_levelScript.NodeFromWorldPoint(m_seeker.position);
            foreach( Node n in m_levelScript.getGrid()) { 
	 			if(m_path != null) {
	 				if (m_path.Contains(n)) {
	 					Gizmos.color = m_seekerColor;
                        Gizmos.DrawCube(n.m_worldPos + pathHeightAdjusment, Vector3.one * (.6f));   
	 				}
	 			}
	 			if(seekerNode == n) { 
                    Gizmos.color = m_seekerColor;
                    Gizmos.DrawCube(n.m_worldPos, Vector3.one * (.9f));     
                }
             }
         }
    }
}