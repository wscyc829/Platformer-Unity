using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {
	void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag ("Player")) {
			GameController.instance.GameOver ();
		} else {
			Destroy (other.gameObject);
		}
	}
}
