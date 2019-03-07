using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
/* 






*/ 

//TODO: Add in FSM for Player / Spawning / ETC ...
public enum PlayerState {
    Dead,
    Normal, 
    Super
}

public class Player : MonoBehaviour {

    private float m_speed = 10.0f;
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
    private float m_distance = 1f; // look ahead 1 tile

    public int m_score = 0; //TODO: move up into GM
    public int m_dotsRemain; //TODO: move up into GM
    private int m_killstreak = 0; //TODO: move up into GM
    public PlayerState m_state = PlayerState.Normal;
    private bool m_arcadeControl = true;
    private AudioSource[] m_pacmanSounds; 
    private AudioSource dotSound; 
    private AudioSource energizerSound;
    private AudioSource fruitSound; 
    private AudioSource deadSound; 
    private List<GameObject> m_ghosts; 
    private Text guiScore; 
    private Text guiDotsRemain;
    
    void Start() {
        m_dest = transform.position;
        m_pacmanSounds = GetComponents<AudioSource>();
        dotSound = m_pacmanSounds[0];
        energizerSound = m_pacmanSounds[1];
        fruitSound = m_pacmanSounds[2];
        deadSound = m_pacmanSounds[3];

        GameObject canvasObject = GameObject.FindGameObjectWithTag("HUDCanvas"); //TAG
        Transform textTr = canvasObject.transform.Find("ScoreText");
        guiScore = textTr.GetComponent<Text>();

        Transform dotTextTr = canvasObject.transform.Find("DotCount");
        guiDotsRemain = dotTextTr.GetComponent<Text>();
       
    }

    void Update() {
         
        //TODO: move to a better home (dBugging code)
        if(Input.GetKeyDown("t")) {
            // toggle arcade controls (world space for top down (arcade) / local space for 3rd person )
            m_arcadeControl = !m_arcadeControl;
            if(m_arcadeControl) { Debug.Log("Arcade Mode Active"); } else { Debug.Log("Arcade Mode DeActivated"); }
         }

         guiScore.text = "Score: " + m_score.ToString();
         guiDotsRemain.text = "DotsRemaining: " + m_dotsRemain.ToString();
    }

    void FixedUpdate() {
        ReadInputAndMove();
    }
    
    public void AddToScore(int amount) {
        m_score += amount;
        m_dotsRemain--;
	}

    public void AteDot() {
        dotSound.Play();
    }

    public void GotFruit() {
        fruitSound.Play();
    }

    public void Energize() {
        foreach (GameObject g in m_ghosts) {
            g.GetComponent<GhostController>().setState(GhostState.Scared);
        }
        energizerSound.Play();
    }

    public void GotCaught() {
        deadSound.Play();

    }


    public void Init() {
         m_ghosts = new List<GameObject>();
    }

    public void AddGhost(GameObject ghost){
        //Debug.Log("Adding Ghost: " + ghost.name);
        m_ghosts.Add(ghost);
    }

    void ReadInputAndMove() {
        // move closer to destination
        Vector3 p = Vector3.MoveTowards(transform.position, m_dest, m_speed * Time.deltaTime);
        GetComponent<Rigidbody>().MovePosition(p);  
       
        if(m_arcadeControl) {   
            // up moves up, down is down, right is right, etc .. (uses world space)
            if (Input.GetAxis("Horizontal") > 0) { m_nextDir = Vector3.right; }
            if (Input.GetAxis("Horizontal") < 0) { m_nextDir = -Vector3.right; }
            if (Input.GetAxis("Vertical") > 0) { m_nextDir = Vector3.forward; }
            if (Input.GetAxis("Vertical") < 0) { m_nextDir = -Vector3.forward; }
    
        } else {
            // force forward 
           m_nextDir = transform.forward; 
            // players perspective (uses local space)
            if (Input.GetAxis("Horizontal") > 0) { m_nextDir = transform.right; }
            if (Input.GetAxis("Horizontal") < 0) { m_nextDir = -transform.right; }
            if (Input.GetAxis("Vertical") > 0) { m_nextDir = transform.forward; }
            if (Input.GetAxis("Vertical") < 0) { m_nextDir = -transform.forward; }
        }

        if (Vector3.Distance(m_dest, transform.position) < 0.0001f) {
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

     bool Valid(Vector3 direction) {
        // cast line from 'next to pacman' to pacman 
        Vector3 pos = transform.position;
        direction += new Vector3(direction.x * m_distance, 0, direction.z * m_distance);
        RaycastHit hit; 
        Physics.Linecast(pos + direction, pos, out hit);
        
        //TODO: Clean this shit up 
        if(hit.collider != null) {
            return hit.collider.tag == "Ghost" || hit.collider.name == "warp" || hit.collider.name == "fruit" || hit.collider.name == "energizer" || hit.collider.name == "dot" || (hit.collider == GetComponent<Collider>());
        } else {
            return false;
        }
    }

    // accessors & mutators 
    public Vector3 getDir() { return m_dir; }
    public void setDir(Vector3 newDir) { m_dir = newDir; }
    public Vector3 getDest() { return m_dest; }
    public void setDest(Vector3 newDest){ m_dest = newDest; }
    
}
