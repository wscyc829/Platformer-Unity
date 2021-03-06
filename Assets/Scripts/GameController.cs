﻿using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
	public class CloundSettings
	{
		public float scroll_speed;
		public float scroll_range;

		public int damage;
	}

	[Serializable]
	public class PlayerSettings
	{
		public int hp = 100;
		public float move_force = 365f;
		public float max_speed = 5f;
		public float jump_force = 1000f;
		public float scale_rate = 0.25f;
	}

	//settings
	public PlayerSettings player_settings;
	public PlatformSettings platform_settings;
	public FallingSettings falling_settings;
	public LavaSettings lava_settings;
	public float scroll_speed;

	public static GameController instance = null;

	//privates
	private bool play_game;
	private bool game_start;
	private bool game_over;

	private Transform platforms_holder;
	private Transform fallings_holder;

	private ParticleSystem erruption;
	private ParticleSystem ashes;
	private ParticleSystem clounds;

	private GameObject blockImage;
	private GameObject game_over_image;

	private Text message_text;

	private GameObject map;

	[HideInInspector]
	public int score;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
			    
		DontDestroyOnLoad(gameObject);

		ShowMainMenuUI (true);
		ShowGameMap (false);
	}
		
	void Start () {
		play_game = false;
		score = player_settings.hp;
	}

	void OnLevelWasLoaded(int level) {
		score = player_settings.hp;

		ShowMainMenuUI (false);
		ShowIntroductionUI (true);
		ShowInGameUI (true);
		ShowGameMap (true);
		InitGame ();
	}

	public void InitGame(){
		game_start = false;
		game_over = false;

		message_text = GameObject.Find ("Message Text").GetComponent<Text>();

		message_text.text = "Welcome To The Olympus!\n\n" +
		"The world was ruined by the violent volcano.\n" +
		"Please help the rock hero - Osamu \nto escape from this chaotic place.";

		platforms_holder = GameObject.Find ("Platforms").transform;

		GameObject toInstantiate = platform_settings.platforms [0];
		(Instantiate (toInstantiate, new Vector3(-6.5f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);
		(Instantiate (toInstantiate, new Vector3(0f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);
		(Instantiate (toInstantiate, new Vector3(6.5f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);
		(Instantiate (toInstantiate, new Vector3(13f, -7.5f, 0), Quaternion.identity) as GameObject).gameObject.transform.SetParent(platforms_holder.transform);

		fallings_holder = GameObject.Find ("Fallings").transform;

		erruption = GameObject.Find ("Erruption").GetComponent<ParticleSystem> ();
		ashes = GameObject.Find ("Ashes").GetComponent<ParticleSystem> ();
		clounds = GameObject.Find ("Clounds").GetComponent<ParticleSystem> ();

		blockImage = GameObject.Find ("Block Image");
		game_over_image = GameObject.Find ("Game Over Image");
		game_over_image.SetActive (false);
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

	void Update(){
		if (play_game) {
			bool b;
			//#if UNITY_STANDALONE || UNITY_WEBPLAYER
				b = Input.GetKeyDown (KeyCode.R);
			//#else
			//	b = Input.touchCount > 0 ? true : false;
			//#endif

			if(!game_start && b){
				message_text.text = "";
				game_start = true;

				erruption.Play ();
				ashes.Play ();
				clounds.Play ();

				StartCoroutine (SpawnPlatforms ());
				StartCoroutine (SpawnFallings ());

				blockImage.SetActive (false);
				ShowIntroductionUI (false);
			}

			if(game_over && b){
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
	}


	public bool CanUpdate(){
		return game_start && !game_over;
	}

	public void GameOver() {
		game_over = true;
		message_text.text = "You are incinerated.\n" + 
			"You grow until " + score;

		blockImage.SetActive (true);
		game_over_image.SetActive (true);
	}

	public void PlayGame(bool b){
		play_game = b;
	}

	public void ShowMainMenuUI(bool b){
		GameObject obj = GameObject.Find ("MainMenu UI");

		foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) {
			child.gameObject.SetActive (b);
		}
		obj.SetActive (true);
	}

	public void ShowInstructionUI(bool b){
		GameObject obj = GameObject.Find ("Instruction UI");

		foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) {
			child.gameObject.SetActive (b);
		}
		obj.SetActive (true);
	}

	public void ShowInGameUI(bool b){
		GameObject obj = GameObject.Find ("InGame UI");

		foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) {
			child.gameObject.SetActive (b);
		}
		obj.SetActive (true);
	}

	public void ShowIntroductionUI(bool b){
		GameObject obj = GameObject.Find ("Introduction UI");

		foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) {
			child.gameObject.SetActive (b);
		}
		obj.SetActive (true);
	}


	public void ShowGameMap(bool b){
		GameObject obj = GameObject.Find ("GameMap");

		foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) {
			child.gameObject.SetActive (b);
		}
		obj.SetActive (true);
	}
	public void ExitGame(){
		Application.Quit();
	}

	public void PlayerMoveRight(){
		PlayerController pc = GameObject.Find ("Player").GetComponent<PlayerController> ();
		pc.MoveRight ();
	}

	public void PlayerMoveLeft(){
		PlayerController pc = GameObject.Find ("Player").GetComponent<PlayerController> ();
		pc.MoveLeft ();
	}

	public void PlayerJump(){
		PlayerController pc = GameObject.Find ("Player").GetComponent<PlayerController> ();
		pc.Jump ();
	}
}
