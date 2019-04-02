using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PacmanController : MonoBehaviour
{
	public Text scoreText;
	public Text restartText;
    public Text gameOverText;
	private int score;
	private bool gameOver;
    private bool restart;
	public float MovementSpeed = 0f;
	private Animator animator = null;
	private Vector3 up = Vector3.zero,
						 right = new Vector3(0, 90,0),
						 down = new Vector3(0, 180,0),
						 left = new Vector3(0, 270,0),
						 currentDirection = Vector3.zero;
						 
						 
	private Vector3 initialPosition = Vector3.zero;
	public AudioSource m_MovementAudio;
	public AudioClip pacmanDying;            // Audio to play when the pacman dies.
    public AudioClip pacmanMoving;           // Audio to play when the pacman is moving.
	
	public void Reset() {
		transform.position = initialPosition;
		animator.SetBool("isDead", false);
		animator.SetBool("isMoving", false);
		currentDirection = down;
	}
	
    // Start is called before the first frame update
    void Start()
    {
		gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
		score = 0;
        QualitySettings.vSyncCount = 0;
		initialPosition = transform.position;
		animator = GetComponent<Animator>();
		SetScoreText();
		Reset();
    }

    // Update is called once per frame
    void Update()
    {
		if (score == 2630) gameOver = true;
		if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        var isMoving = true;
		var isDead = animator.GetBool("isDead");
		
		if(isDead) { 
			isMoving = false;
			// m_MovementAudio.clip = pacmanDying;
			// m_MovementAudio.PlayOneShot(pacmanDying);
        } else if (Input.GetKey(KeyCode.UpArrow)) currentDirection = up;
        else if (Input.GetKey(KeyCode.RightArrow)) currentDirection = right;
        else if (Input.GetKey(KeyCode.DownArrow)) currentDirection = down;
        else if (Input.GetKey(KeyCode.LeftArrow)) currentDirection = left;
        else isMoving = false;
		
		
        transform.localEulerAngles = currentDirection;
		animator.SetBool("isMoving", isMoving);
		
		
        if (isMoving) {
            transform.Translate(Vector3.forward * MovementSpeed * Time.deltaTime);
		}
		
		if (gameOver)
            {
				GameOver();
                restartText.text = "Press 'R' for Restart";
                restart = true;
            }
    }
	
	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Enemy")) {
			m_MovementAudio.PlayOneShot(pacmanDying);
			animator.SetBool("isDead", true);
		}
		if(other.CompareTag("LWall")) 
			transform.position = new Vector3(23.0f, 1.0f, -5.0f);
		
		if(other.CompareTag("RWall")) 
			transform.position = new Vector3(-21.0f, 1.0f, -5.0f);

        if (other.CompareTag("Enemy"))
            animator.SetBool("isDead", true);

        if (other.gameObject.CompareTag("Pickup"))
        {
			m_MovementAudio.PlayOneShot(pacmanMoving);
            other.gameObject.SetActive(false);
            score = score + 10;
            SetScoreText();
        }
    }
	
	void SetScoreText ()
	{
		scoreText.text = "Score: " + score.ToString ();
	}
	
	public void GameOver()
    {
        gameOverText.text = "You Win!";
        gameOver = true;
    }
}
