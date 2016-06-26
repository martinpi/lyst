using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour {

	void Start () {

	}

	public void DoLoadNextLevel() {
		SceneManager.LoadScene("bla");
	}
}
