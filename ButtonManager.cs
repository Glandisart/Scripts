using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
	public static ButtonManager Instance{ get; set;}

	public void DragAndDrop(Card c, GameObject sender){
		//try{(sender.GetComponent ("Halo") as Behaviour).enabled = true;}
		//catch{}
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
		d.PorteurDrapeau = new List<bool> ();
		d.Cartes = new List<NormalCard> ();
		d.CartesShortName = new List<string> ();
		foreach (Card c in CreationBoardManager.Instance.CardBoard) {
			if (c != null) {
				d.Abscisses.Add (c.CurrentX); 
				d.Ordonnées.Add (c.CurrentY);
				d.CartesId.Add (c.Id);
				d.PorteurDrapeau.Add (c.PorteDrapeau);
				d.Cartes.Add (c as NormalCard);
				d.CartesShortName.Add (c.ShortName);
			}
		}
		//Object NewDeck
		/*System.Object prefab = */PrefabUtility.CreatePrefab("Assets/Resources/Decks/"+DeckName.text+".prefab",CreationBoardManager.Instance.NewDeck.gameObject);
	}

	public void NewGameBtn (string NewGame) {

		SceneManager.LoadScene (NewGame);

	}

	public void ExitGameBtn() {

		Application.Quit ();

	}

}
