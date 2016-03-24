using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

	private bool fall = false;

	void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.CompareTag("Player")) {
			Invoke ("Fall", GameController.instance.platform_settings.fall_delay);
		}
	}

	void Fall() {
		fall = true;
	}

	void Update() {
		if (GameController.instance.CanUpdate()) {
			if (fall) {
				float fall_speed = GameController.instance.platform_settings.fall_speed;
				transform.position = transform.position + Vector3.down * Time.deltaTime * fall_speed;
			}

			float scroll_speed = GameController.instance.platform_settings.scroll_speed;
			transform.position = transform.position + Vector3.left * Time.deltaTime * scroll_speed;
		}
	}
}
