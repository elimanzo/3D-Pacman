using UnityEngine;
using System.Collections.Generic;
/* 
	Shadow 	- Blinky 	- “pursuer” or “chaser” 	- Red 
	Speedy 	- Pinky 	- “ambusher” 				- Pink 
	Bashful	- Inky 		- “whimsical” 				- Blue 
	Pokey	- Clyde 	- “feigning ignorance” 		- Orange

	“This is the heart of the game. I wanted each ghostly enemy to have a specific character 
	and its own particular movements, so they weren’t all just chasing after Pac Man in single 
	file, which would have been tiresome and flat.”
															- Toru Toru Iwatani, Pac-Man creator 
															- Pac-Man was released in 1980 
															- 1 year, 5 months (a five man team)
															- Masaya Nakamura founded Namco (recently passed)
*/
public enum GhostName {
	Blinky,
	Pinky, 
	Inky,
	Clyde
}

public enum GhostState {
	Dead,
	Wander, 
	Scatter,
	Chase, 
	Scared
}

public class GhostController : MonoBehaviour {
	public GhostName persona; 

	// public float floatVariant = 0.2f;
	// public float floatSpeed = 1.0f;
	public float moveSpeed = 1.5f;


	// movement variables 
	private Vector3 m_dest = Vector3.zero;
	private Vector3 m_dir = Vector3.zero;
	private Vector3 m_nextDir = Vector3.zero;
	private float m_distance = 0.5f; // look ahead 1 tile

	// states 
	private GhostState m_state;
	private GhostState m_lastState; 

	// targets 	
	private GameObject m_target;
	private GameObject m_homeTarget;
	private GameObject m_scatterTarget;

	// audio 
	private AudioSource m_deathSound;
	
	
	// manage visuals for state
	private Color m_ghostColor; 
	private Renderer m_renderer;
	private float m_blinkTimer = 0.0f;
	private bool m_altColor = false;
	private float m_scaredTimer = 0.0f;
	private float m_chaseTimer = 0.0f;
	
	private Level m_levelScript; // Reference to the instanced Level script
	private Player m_pacMan; // Reference to the instanced Player script (used for targeting)
	
	
	void Start() {

		m_dest = transform.position;
		m_state = GhostState.Scatter;
		m_lastState = m_state;
		m_renderer = GetComponent<Renderer>();	
		m_deathSound = GetComponent<AudioSource>();


		if(m_target == null) {
			m_target = new GameObject();
			m_target.name = "Target";
		}		

		if(m_homeTarget == null) {
			m_homeTarget = new GameObject();
			m_homeTarget.name = "Home Target";
			m_homeTarget.transform.position = transform.position;
		}

		if(m_scatterTarget == null) {
			m_scatterTarget = new GameObject();
			m_scatterTarget.name = "Scatter Target";
		}		
		
		switch(persona) {
			case GhostName.Blinky: 
				m_ghostColor = new Color32(255, 0, 0, 204);	
				m_scatterTarget.transform.position = new Vector3(24, 0, 30);
				break; 
			case GhostName.Pinky:
				m_ghostColor = new Color32(255, 0, 247, 204); 
				m_scatterTarget.transform.position = new Vector3(3, 0, 30);
				break;
			case GhostName.Inky: 
				m_ghostColor = new Color32(73 , 179, 219, 204);		
				m_scatterTarget.transform.position = new Vector3(24, 0, 2);
				break;
			case GhostName.Clyde:
				m_ghostColor = new Color32(255, 132, 0, 204);
				m_scatterTarget.transform.position = new Vector3(3, 0, 2);	
				break;
		}
		m_renderer.material.color = m_ghostColor;

		// Pathfinding
		//m_pathToTarget = m_pathfinder.findBestPath(m_level.getPathMap(), m_pos, m_pacMan.pos);
			
	}

	void Update() {
		
		switch(m_state) {
			case GhostState.Dead: 
				GoHome(); // back to the ghost house with him
				FollowPath();
				break; 
			case GhostState.Scatter:
				Scatter();
				FollowPath();
				break;
			case GhostState.Wander: 
				GetComponent<Pathfinding>().setTarget(m_scatterTarget.transform);
				Wander();
				break; 
			case GhostState.Chase: 
				GetComponent<Pathfinding>().setTarget(m_pacMan.transform);
				m_chaseTimer += Time.deltaTime;
				if(m_chaseTimer > 10.0f) { 
					m_state = m_lastState;
					m_chaseTimer = 0;
				}
				FollowPath();
				break; 
			case GhostState.Scared:
				m_scaredTimer += Time.deltaTime;
				m_blinkTimer += Time.deltaTime;
				if (m_blinkTimer > .2f ) {
					m_altColor = !m_altColor; 
					m_renderer.material.color = (m_altColor) ? Color.white : Color.blue;
					m_blinkTimer = 0;
				} 
				if(m_scaredTimer > 10.0f) { 
					m_renderer.material.color = m_ghostColor;
					m_state = m_lastState;
					if(m_state == GhostState.Scatter) { 
						FollowPath(); 
					}
					m_scaredTimer = 0;
				}
				Wander(); // switch to wander after being scared 

				break;
			default: 
				Debug.LogWarning("Gost Controller doesn't know what state it's in");
				break;
		}
	}

	void FixedUpdate() {

		switch(persona) {
			case GhostName.Blinky: 
				//Wander();
				break; 
			case GhostName.Pinky:
				//Wander();
				break;
			case GhostName.Inky: 
				//Wander();
				break;
			case GhostName.Clyde:
				//Wander();
				break;
		}		
	}


	void OnTriggerEnter(Collider other) {
		if (other.name == "player") {
			if (m_state != GhostState.Scared) {
				// PacMan dies
				Debug.Log("Player Died!");
				
				other.GetComponent<Player>().GotCaught();

			} else {
				// Ghost dies
				Debug.Log("Ghost Died!");
				//GetComponent<Renderer>().enabled = false;
				m_deathSound.Play();
				m_state = GhostState.Dead;
				m_renderer.enabled = false;
			}
		}
	}

	
	void GoHome() {	GetComponent<Pathfinding>().setTarget(m_homeTarget.transform); 	}
	void Scatter() { GetComponent<Pathfinding>().setTarget(m_scatterTarget.transform); }

	void Chase() {
		//TODO: work this out 
	}


	void FollowPath() {
		
		List<Node> path  = GetComponent<Pathfinding>().m_path; 

		if (path != null) {
			Vector3 p = Vector3.MoveTowards(transform.position, m_dest, moveSpeed * Time.deltaTime);
			GetComponent<Rigidbody>().MovePosition(p);

			if(path.Count > 0) {
				m_nextDir = path[0].m_worldPos; 	
				transform.LookAt(m_nextDir);
				m_nextDir = transform.forward;
				path.RemoveAt(0);
			} else {
				// target reached
				if(m_state == GhostState.Dead) {
					m_renderer.enabled = true; 
					m_renderer.material.color = m_ghostColor;
					m_state = GhostState.Scatter;
				} else if(m_state == GhostState.Scatter) {
					m_state = GhostState.Chase; 
				} 
				
			}

			if (Vector3.Distance(m_dest, transform.position) < 0.00001f) {
			
				if (Valid(m_nextDir)) { 
					m_dest = (Vector3)transform.position + m_nextDir;
					m_dir = m_nextDir;
					//TODO: rethink the following
				} else {   // nextDir NOT valid
					if (Valid(m_dir)) {  // and the prev. direction is valid
						m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
					}
					// otherwise, do nothing ?
				}
			}
			transform.LookAt(m_dest);
		}
	}

	

	void Wander() {
        
		//move closer to destination
        Vector3 p = Vector3.MoveTowards(transform.position, m_dest, moveSpeed * Time.deltaTime);
        GetComponent<Rigidbody>().MovePosition(p);

		
		Vector3[] choices = { Vector3.right, -Vector3.right, Vector3.forward, -Vector3.forward };
		int howManyChoices = choices.Length;
		int myRandomIndex;
		
		
		// 0 - up 
		// 1 - down 
		// 3 - right 
		// 4 - left 

		Vector3 reversed = Vector3.zero;		
		if (!Valid(m_nextDir)) {
			do {
				myRandomIndex = Random.Range( 0, 4 );
			} while (choices[myRandomIndex] == reversed);
		
			m_nextDir = choices[ myRandomIndex ];
		
			if (m_nextDir == Vector3.forward) {
				reversed = -Vector3.forward;
			} else if (m_nextDir == -Vector3.forward) {
				reversed = Vector3.forward;
			} else if (m_nextDir == Vector3.right) {
				reversed = -Vector3.right;
			} else  {
				reversed = Vector3.right;
			}
		}
		
  		if (Vector3.Distance(m_dest, transform.position) < 0.00001f) {
            if (Valid(m_nextDir)) { 
                m_dest = (Vector3)transform.position + m_nextDir;
                m_dir = m_nextDir;
                //TODO: rethink the following
            } else {   // nextDir NOT valid
                if (Valid(m_dir)) {  // and the prev. direction is valid
                    m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
                }
                // otherwise, do nothing ?
            }
        }
        transform.LookAt(m_dest);
        
    }

	private bool isOppositeDirection(Vector3 direction) {
		bool retVal = false;
		if (((m_dir == Vector3.forward) && (direction == -Vector3.forward)) || 
			((m_dir == -Vector3.forward) && (direction == Vector3.forward)) || 
			((m_dir == Vector3.right) && (direction == -Vector3.right)) || 
			((m_dir == -Vector3.right) && (direction == Vector3.right))) {
				retVal =  true;
		} 
		return retVal;
	}

	bool Valid(Vector3 direction) {
        bool retVal = false;
	
		// cast line from 'next to pacman' to pacman not from directly the center of next tile but just a little further from center of next tile
        Vector3 pos = transform.position;
        direction += new Vector3(direction.x * m_distance, 0, direction.z * m_distance);
        RaycastHit hit; 
        Physics.Linecast(pos + direction, pos, out hit);
        
        if(hit.collider != null) {
            retVal = hit.collider.name == "player" || hit.collider.name == "door" || hit.collider.name == "fruit" || hit.collider.tag == "Ghost" || hit.collider.name == "energizer" || hit.collider.name == "dot" || (hit.collider == GetComponent<Collider>());
        } 
		return retVal;
    }


	public void setLevel(Level theLevel){ m_levelScript = theLevel; }
	public Color getColor() {return m_ghostColor; }
	public void setPacMan(Player player) { m_pacMan = player; }
	public Player getPacMan() { return m_pacMan; }

	public void setState(GhostState newState) {
		if ((m_state != GhostState.Dead)) {
			m_state = newState;
		}
		
		if (m_state == GhostState.Scared) {
			m_renderer.material.color = Color.blue;
			m_scaredTimer = 0;
		}
	}
	
}
