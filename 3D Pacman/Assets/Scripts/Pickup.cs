using UnityEngine;

public enum pickupType {
	None,
	Dot, 
	Energizer,
	Fruit
}

public class Pickup : MonoBehaviour {

	public pickupType type;
	private int pickupValue = 0; 

	
	private AudioSource[] audioSource; 
	private AudioSource pickupSound; 



	void Start() {
		//audioSource = GetComponents<AudioSource>();
		//pickupSound = audioSource[0]; 		
		pickupSound = GetComponent<AudioSource>();
	}


	void OnTriggerEnter(Collider other) {
		if (other.name == "player") {

			switch(type) {
				case pickupType.Dot:
					// normal points (10 per dot)
					pickupValue = 10;
					other.GetComponent<Player>().AteDot();
					other.GetComponent<Player>().AddToScore(pickupValue);
					break;
				case pickupType.Energizer:
					// powerup the pacman and scare the ghosts 
					other.GetComponent<Player>().Energize();
					break; 
				case pickupType.Fruit:
					Debug.Log("Fruit");
					// multiplier code 
					pickupValue = 200;
					other.GetComponent<Player>().GotFruit();
					other.GetComponent<Player>().AddToScore(pickupValue);
					break; 
			}
			//pickupSound.Play();
			//Destroy(gameObject, pickupSound.clip.length);
			Destroy(gameObject);
		}
	}
}
