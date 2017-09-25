using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	public void DragAndDrop(Card c){
		CreationBoardManager.Instance.selectedCard = null;
		CreationBoardManager.Instance.DragedCard = Instantiate(c);
		CreationBoardManager.Instance.DragedCard.transform.position = Input.mousePosition;
		CreationBoardManager.Instance.DragedCard.transform.rotation = Quaternion.Euler (0, 180, 0);
		//CreationBoardManager.Instance.DragedCard.SetPosition (Input.mousePosition.x,Input.mousePosition.y);
		CreationBoardManager.Instance.DragedGameObject = Instantiate(CreationBoardManager.Instance.DragedCard.gameObject, Input.mousePosition,Quaternion.Euler(0,180,0)) as GameObject;
		CreationBoardManager.Instance.DragedGameObject.transform.SetParent (transform);
	}

	public InputField DeckName;
	public void SaveNewDeck(){
		Deck d = CreationBoardManager.Instance.NewDeck;
		d.Abscisses = new List<int> ();
		d.Ordonnées = new List<int> ();
		d.Cartes = new List<Card> ();
		foreach (Card c in CreationBoardManager.Instance.CardBoard) {
			if (c != null) {
				d.Abscisses.Add (c.CurrentX); 
				d.Ordonnées.Add (c.CurrentY);
				d.Cartes.Add (c);
			}
		}
		//Object NewDeck
		System.Object prefab = PrefabUtility.CreatePrefab("Assets/Deck/"+DeckName.text+".prefab",CreationBoardManager.Instance.NewDeck.gameObject);
	}

	public void NewGameBtn (string NewGame) {

		SceneManager.LoadScene (NewGame);

	}

	public void ExitGameBtn() {

		Application.Quit ();

	}

}
