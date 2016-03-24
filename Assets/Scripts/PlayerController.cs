using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private bool facingRight;
	private bool jump;
	private bool grounded;

	private int hp;

	private Text hp_text;

	private Rigidbody2D rb2d;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D>();
		hp_text = GameObject.Find ("HP Text").GetComponent<Text>();
	}

	void Start(){
		facingRight = true;
		jump = false;
		grounded = false;

		hp = GameController.instance.player_settings.hp;

		hp_text.text = "HP: " + hp;
	}

	void Update () {
		if (GameController.instance.CanUpdate ()) {
			if (Input.GetButtonDown ("Jump") && grounded) {
				jump = true;
			}

			if (grounded) {
				float speed = GameController.instance.platform_settings.scroll_speed;
				transform.position = transform.position + Vector3.left * Time.deltaTime * speed;
			}
		} else if (hp <= 0) {
			transform.position = transform.position + Vector3.down * Time.deltaTime;
		}
	}
		
	void FixedUpdate() {
		if (GameController.instance.CanUpdate()) {
			float h = Input.GetAxis ("Horizontal");

			float max_speed = GameController.instance.player_settings.max_speed;
			if (h * rb2d.velocity.x < max_speed) {
				float move_force = GameController.instance.player_settings.move_force;
				rb2d.AddForce (Vector2.right * h * move_force);
			}

			if (Mathf.Abs (rb2d.velocity.x) > max_speed) {
				rb2d.velocity = new Vector2 (Mathf.Sign (rb2d.velocity.x) * max_speed, rb2d.velocity.y);
			}

			if (h > 0 && !facingRight) {
				Flip ();
			} else if (h < 0 && facingRight) {
				Flip ();
			}
			
			if (jump) {
				float jump_force = GameController.instance.player_settings.jump_force;
				rb2d.AddForce (new Vector2 (0f, jump_force));
				jump = false;
				grounded = false;
			}
		}
	}

	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.CompareTag("Lava")) {
			grounded = true;
		}
		else if(coll.gameObject.CompareTag("Platform")) {
			grounded = true;
		}
	}

	void OnCollisionStay2D(Collision2D coll){
		if(coll.gameObject.CompareTag("Lava")){
			int damage = coll.gameObject.GetComponent<LavaController> ().damage;

			UpdateHP (damage);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.CompareTag("Falling")){
			int damage = other.gameObject.GetComponent<FallingController> ().damage;
			UpdateHP (damage);

			Destroy (other.gameObject);
		}
	}

	void UpdateHP(int n){
		hp -= n;

		if (hp <= 0) {
			hp = 0;
			GameController.instance.GameOver ();
		} else if (hp > 100) {
			float rate = GameController.instance.player_settings.scale_rate;
			float scale = 1 + hp / 100 * rate;

			transform.localScale = new Vector3 (scale, scale ,1);
		}

		if (hp > GameController.instance.score) {
			GameController.instance.score = hp;
		}

		hp_text.text = "HP: " + hp;
	}
}
