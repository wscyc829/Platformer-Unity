using UnityEngine;
using System.Collections;

public class LavaController : MonoBehaviour {
	public int damage;
	private Vector3 startPosition;

	void Start () {
		startPosition = transform.position;
	}

	void Update () {
		if (GameController.instance.CanUpdate()) {
			float scroll_speed = GameController.instance.lava_settings.scroll_speed;
			float scroll_range = GameController.instance.lava_settings.scroll_range;
			float newPosition = Mathf.Repeat (Time.time * scroll_speed, scroll_range);

			transform.position = startPosition + Vector3.left * newPosition;
		}
	}
}
