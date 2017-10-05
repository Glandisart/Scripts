using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

public class CreationBoardManager : MonoBehaviour {
	public static CreationBoardManager Instance{ get; set; }
	public Card[,] CardBoard{ get; set; }
	public Card DragedCard;
	public GameObject DragedGameObject;
	public Deck NewDeck;
	public Button SaveButton;
	public Text AffichageValeurDeck;
	public float NewDeckValue;
	public Button CardButton;
	public Canvas canvas;
	public ButtonManager buttonManager;

	public List<Card> AllCards; 

	private int selectionX = -1;
	private int selectionY = -1;

	public Card selectedCard;

	private void Start(){ // Fonction qui s'éxecute au lancement
		AllCards = Resources.LoadAll<Card>("CartesDuJeu").ToList();
		SpawnButtons();
		Instantiate(NewDeck);
		Instance=this;
		CardBoard = new Card[Values.LargeurPlateau,Values.HauteurPlateau];
	}

	private void SpawnButtons(){
		int h = Screen.height;
		int w = Screen.width;
		//float HauteurTopButton = 180f;
		//float HauteurOneButton = 24f;
		//float LargeurColonne = 25f;
		int iterations = 0;
		int NbCartesParColones  = 8;


		foreach (Card c in AllCards) {
			GameObject go = Instantiate(CardButton.gameObject,Camera.main.ScreenToWorldPoint(new Vector3((float)(0.1+0.05*(float)((int)(iterations%NbCartesParColones)))*(float)w,(float)(0.4-0.1*(float)((int)(iterations/NbCartesParColones)))*(float)h,200)),Quaternion.Euler(0,0,0));
			go.transform.SetParent (canvas.transform);
			go.name = AllCards [iterations].Name;
			go.GetComponentInChildren<Text>().text = go.name;
			go.GetComponent<Button> ().onClick.AddListener (() => {
				buttonManager.DragAndDrop(c,go);
			});
			iterations++;

		}

		/*foreach (Card c in AllCards) {
			GameObject go = Instantiate(CardButton.gameObject,new Vector2(200f+iterations/NbCartesParColones * LargeurColonne, HauteurTopButton - iterations%NbCartesParColones * HauteurOneButton),Quaternion.Euler(0,0,0));
			go.transform.SetParent (canvas.transform);
			go.name = AllCards [iterations].Name;
			go.GetComponentInChildren<Text>().text = go.name;
			go.GetComponent<Button> ().onClick.AddListener (() => {
				buttonManager.DragAndDrop(c,go);
			});
			iterations++;

		}*/
	}

	private void Update() //Fonction qui s'execute à chaque frame
	{
		UpdateSelection ();
		DrawBoard ();
		if (Input.GetMouseButtonDown (0)) {
			//Debug.Log ("click");
			if(selectionX>=0 && selectionY>=0){
				if (selectedCard == null) {
					//select card
					SelectCard(selectionX,selectionY);
					//Debug.Log(selectedCard.ToString());
				} else {
					//move card
					if(DragedCard == null)MoveCard(selectionX,selectionY);
				}
			}
		}
		RemoveCard ();
		DragAndDrop ();
	} 

	private void DragAndDrop(){
		if (DragedCard == null)
			return;
		if (DragedGameObject != null) {
			DragedGameObject.transform.position = Input.mousePosition;
			DragedCard.transform.position = Input.mousePosition;
			//Debug.Log (DragedCard.transform.position);
		}
		if (selectionX < 0 || selectionY < 0)
			return;
		if (CardBoard [selectionX,selectionY] == null && Input.GetMouseButtonDown(0) && Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition) /*,25.0f, LayerMask.GetMask ("BoardPlane2")*/)) {
			NewDeckValue += DragedCard.Valeur;
			AffichageValeurDeck.text = NewDeckValue.ToString ();
			CardBoard [selectionX, selectionY] = Instantiate(DragedCard);
			CardBoard [selectionX, selectionY].transform.SetParent (canvas.transform);
			CardBoard [selectionX, selectionY].transform.localScale = new Vector3 (5, 5, 5);
			CardBoard [selectionX, selectionY].transform.position = GetTileCenter (selectionX, selectionY);
			CardBoard [selectionX, selectionY].transform.Rotate (0, 180, 0);
			CardBoard [selectionX, selectionY].SetPosition (selectionX, selectionY);
			DragedCard = null;
			Destroy (DragedGameObject);
		}
	}

	private void DrawBoard()// Dessine le plateau et retourne là où est la souris
	{
		Vector2 widthLine = Vector2.right * Values.LargeurPlateau;
		Vector2 heightLine = Vector2.up * Values.HauteurTerritoire;

		for (int i = 0; i <= Values.HauteurTerritoire; i++) 
		{
			Vector2 start = Vector2.up * i;
			Debug.DrawLine (start, start + widthLine);

			for (int j = 0; j <= Values.LargeurPlateau; j++) 
			{
				start = Vector2.right * j;
				Debug.DrawLine (start, start + heightLine);
			}
		}
		//Draw une croix sur la case où il y a la souris
		if (selectionX >= 0 && selectionY >= 0) {
			//Debug.Log (selectionX.ToString() + "; " + selectionY.ToString());
			Debug.DrawLine (Vector2.right * selectionX + Vector2.up * selectionY,
				Vector2.right * (selectionX + 1) + Vector2.up * (selectionY+1));
			Debug.DrawLine (Vector2.right * (selectionX) + Vector2.up * (selectionY+1),
				Vector2.right * (selectionX + 1) + Vector2.up * selectionY);

		}

	}


	private void MoveCard(int x, int y){
		if (CardBoard [x, y] == null) {
			CardBoard [selectedCard.CurrentX, selectedCard.CurrentY] = null;
			selectedCard.transform.position = GetTileCenter (x, y);
			selectedCard.SetPosition (x, y);
			CardBoard [x, y] = selectedCard;
		}
		selectedCard = null;
	}

	private void RemoveCard(){
		if (selectedCard == null)
			return;
		if (Input.GetKeyDown(KeyCode.Delete)) {
			NewDeckValue -= selectedCard.Valeur;
			AffichageValeurDeck.text = NewDeckValue.ToString ();
			DestroyImmediate (CardBoard [selectedCard.CurrentX, selectedCard.CurrentY].gameObject);
			CardBoard [selectedCard.CurrentX, selectedCard.CurrentY] = null;
			selectedCard = null;
		}
	}

	private Vector2 GetTileCenter(int x, int y){
		Vector2 origin = Vector2.zero;
		origin.x += (Values.TILE_SIZE * x) + Values.TILE_OFFSET;
		origin.y += (Values.TILE_SIZE * y) + Values.TILE_OFFSET;
		return origin;
	}

	private void SelectCard(int x, int y){
		if (CardBoard [x, y] == null) 
			return;
		selectedCard = CardBoard [x, y];
	}

	private void UpdateSelection()
	{
		if (!Camera.main)
			return;
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit /*,25.0f, LayerMask.GetMask ("BoardPlane2")*/)) {
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.y;
			//Debug.Log ("yoooo");
		} 
		else 
		{
			selectionX = -1;
			selectionY = -1;
		}

	}
}
