using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	[Serializable]
	public class PlatformSettings
	{
		public GameObject[] platforms;

		public float spawn_x;
		public float spawn_y;
		public float spawn_start_wait;
		public float spawn_min_wait;
		public float spawn_max_wait;

		public float scroll_speed;
		public float fall_delay = 1f;
		public float fall_speed = 1f;
	}

	[Serializable]
	public class FallingSettings
	{
		public GameObject[] fallings;

		public float spawn_x;
		public float spawn_y;
		public int max_spawn;
		public float spawn_rate;
		public float spawn_wait;
		public float spawn_start_wait;

		public float fall_speed;
	}

	[Serializable]
	public class LavaSettings
	{
		public float scroll_speed;
		public float scroll_range;

		public int damage;
	}

	[Serializable]
	public class PlayerSettings
	{
		public GameObject player;
		public int hp = 100;
		public float move_force = 365f;
		public float max_speed = 5f;
		public float jump_force = 1000f;
	}

	//settings
	public PlayerSettings player_settings;
	public PlatformSettings platform_settings;
	public FallingSettings falling_settings;
	public LavaSettings lava_settings;
	public float scroll_speed;

	public GameObject[] stones;

	public static GameController instance = null;

	//privates
	private bool game_start = false;
	private bool game_over = false;

	private GUIText hp_text;
	private GUIText instruction_text;

	private Transform platforms_holder;
	private Transform fallings_holder;

	private GameObject player;

	private float score;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
			    
		DontDestroyOnLoad(gameObject);
	}
		
	void Start () {
		InitGame ();
	}

	void OnLevelWasLoaded(int level) {
		InitGame ();
	}

	void InitGame(){
		game_start = false;
		game_over = false;

		instruction_text = GameObject.Find ("Instruction Text").GetComponent<GUIText>();

		instruction_text.text = "Use 'Left & Right Arrow' To Move the Player\n" + 
			"'Space' To Jump\n" + "Press 'R' To Start";

		platforms_holder = new GameObject ("Platforms").transform;
		GameObject toInstantiate = platform_settings.platforms [0];
		(Instantiate (toInstantiate, new Vector3(-6.5f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);
		(Instantiate (toInstantiate, new Vector3(0f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);
		(Instantiate (toInstantiate, new Vector3(6.5f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);
		(Instantiate (toInstantiate, new Vector3(13f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);

		fallings_holder = new GameObject ("Fallings").transform;

		player = Instantiate (player_settings.player, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject; 
	}

	IEnumerator SpawnPlatforms() {

		yield return new WaitForSeconds (platform_settings.spawn_start_wait);

		while(true) {
			Vector2 spawnPosition = new Vector2(platform_settings.spawn_x, platform_settings.spawn_y);

			GameObject toInstantiate = platform_settings.platforms[Random.Range(0, platform_settings.platforms.Length)];
			GameObject instance = Instantiate (toInstantiate, spawnPosition, Quaternion.identity) as GameObject;
			instance.transform.SetParent (platforms_holder);

			yield return new WaitForSeconds (Random.Range(platform_settings.spawn_min_wait, platform_settings.spawn_max_wait));

			if (!CanUpdate()) {
				break;
			}
		}
	}

	IEnumerator SpawnFallings() {
		yield return new WaitForSeconds (falling_settings.spawn_start_wait);

		while (true) {
			for (int i = 0; i < falling_settings.max_spawn; i++) {
				Vector2 spawnPosition = new Vector2(Random.Range(-falling_settings.spawn_x, falling_settings.spawn_x), falling_settings.spawn_y);
				GameObject toInstantiate = falling_settings.fallings[Random.Range(0, falling_settings.fallings.Length)];

				GameObject instance = Instantiate (toInstantiate, spawnPosition, Quaternion.identity) as GameObject;
				instance.transform.SetParent (fallings_holder);

				yield return new WaitForSeconds (falling_settings.spawn_rate);

				if (!CanUpdate()) {
					break;
				}
			}

			yield return new WaitForSeconds (falling_settings.spawn_wait);

			if (!CanUpdate()) {
				break;
			}
		}
	}

	public void SpawnStones(){
		
	}


	void Update(){
		if(!game_start && Input.GetKeyDown(KeyCode.R)){
			instruction_text.text = "";
			game_start = true;
			ParticleSystem errupt = GameObject.Find ("Erruption").GetComponent<ParticleSystem> ();
			errupt.Play ();
			StartCoroutine (SpawnPlatforms ());
			StartCoroutine (SpawnFallings ());
		}

		if(game_over && Input.GetKeyDown(KeyCode.R)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}


	public bool CanUpdate(){
		return game_start && !game_over;
	}

	public void GameOver() {
		game_over = true;
		instruction_text.text = "Press 'R' To Restart";
		ParticleSystem errupt = GameObject.Find ("Erruption").GetComponent<ParticleSystem> ();
		errupt.Stop ();
		//	enabled = false;
	}
}
