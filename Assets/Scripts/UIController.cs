using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
	public static UIController instance = null;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
}
