using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

	public void NewGameBtn (string NewGame) {

		SceneManager.LoadScene (NewGame);

	}

	public void ExitGameBtn() {

		Application.Quit ();

	}

}
