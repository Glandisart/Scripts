using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// test
public class BoardManager : MonoBehaviour 
{
	public static BoardManager Instance{ get; set; }
	private bool [,] allowedMoves{ get; set; }

	public Card[,] CardBoard{ get; set; } // Le plateau 
	private Card selectedCard; // La carte séléctionnée par le joueur 

	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f; // taille des cases

	private int selectionX = -1;
	private int selectionY = -1; // Coordonnées de l'emplacement de la souris

	public List<GameObject> cards; // Liste des cartes existantes
	private List<GameObject> activeCards = new List<GameObject>();// Liste des cartes présentes dans le jeu

	public struct IntInt{
		public IntInt(int xvalue, int yvalue){
			x=xvalue;
			y=yvalue;
		}
		public int x;
		public int y;
	}// une structure qui contient 2 int

	private List<int> cardsIndexes = new List<int>(); // Cartes initialement présentes
	private List<IntInt> cardsPlaces = new List<IntInt> ();// Emplacement des cartes initialement présentes 

	public bool isBlueTurn = true;

	private void Update() //Fonction qui s'execute à chaque frame
	{
		UpdateSelection ();
		DrawBoard ();
		//Debug.Log (selectionX.ToString() + "  " + selectionY.ToString());
		if (Input.GetMouseButtonDown (0)) {
			//Debug.Log ("click");
			if(selectionX>=0 && selectionY>=0){
				if (selectedCard == null) {
					//select card
					SelectCard(selectionX,selectionY);
					//Debug.Log(selectedCard.ToString());
				} else {
					//move card
					MoveCard(selectionX,selectionY);
				}
			}
		}
	}
	private void SelectCard(int x, int y){
		//Debug.Log ("1");
		if (CardBoard [x, y] == null) 
			return;
		//Debug.Log ("2");
		if (CardBoard [x, y].IsBlue != isBlueTurn)
			return;
		//Debug.Log ("3");
		allowedMoves = CardBoard[x,y].PossibleMoves();
		selectedCard = CardBoard [x, y];
		BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves);
		//Debug.Log (selectedCard.ToString ());
	}
	private void MoveCard(int x, int y){
		if (allowedMoves[x,y]) {
			CardBoard[selectedCard.CurrentX, selectedCard.CurrentY] = null;
			selectedCard.transform.position = GetTileCenter (x, y);
			selectedCard.SetPosition (x, y);
			CardBoard [x, y] = selectedCard;
			isBlueTurn = !isBlueTurn;
		}
		BoardHighlights.Instance.HideHighlights ();
		selectedCard = null;
	}

	private void Start(){ // Fonction qui s'éxecute au lancement
		Instance=this;
		SpawnAllCards ();
	}

	private void DrawBoard()// Dessine le plateau et retourne là où est la souris
	{
		Vector2 widthLine = Vector2.right * 10;
		Vector2 heightLine = Vector2.up * 10;

		for (int i = 0; i <= 10; i++) 
		{
			Vector2 start = Vector2.up * i;
			Debug.DrawLine (start, start + widthLine);

			for (int j = 0; j <= 10; j++) 
			{
				start = Vector2.right * j;
				Debug.DrawLine (start, start + heightLine);
			}
		}
		//Draw selection
		if (selectionX >= 0 && selectionY >= 0) {
			//Debug.Log (selectionX.ToString() + "; " + selectionY.ToString());
			Debug.DrawLine (Vector2.right * selectionX + Vector2.up * selectionY,
				Vector2.right * (selectionX + 1) + Vector2.up * (selectionY+1));
			Debug.DrawLine (Vector2.right * (selectionX) + Vector2.up * (selectionY+1),
				Vector2.right * (selectionX + 1) + Vector2.up * selectionY);

		}

	}

	private void UpdateSelection()
	{
		if (!Camera.main)
			return;
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("BoardPlane"))) {
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.y;
		} 
		else 
		{
			selectionX = -1;
			selectionY = -1;
		}

	}

	private void SpawnCard(int index, int x, int y){
		Debug.Log (cards.ToArray().Length.ToString());
		GameObject go = Instantiate (cards [index], GetTileCenter(x,y),Quaternion.Euler(0,180,0) ) as GameObject;
		go.transform.SetParent (transform);
		CardBoard [x, y] = go.GetComponent<Card> ();
		CardBoard [x, y].SetPosition (x, y);
		activeCards.Add (go);
	}

	private Vector2 GetTileCenter(int x, int y){
		Vector2 origin = Vector2.zero;
		origin.x += (TILE_SIZE * x) + TILE_OFFSET;
		origin.y += (TILE_SIZE * y) + TILE_OFFSET;
		return origin;
	}

	private void SpawnAllCards(){
		GetCardsPositions ();
		CardBoard = new Card[10,10];
		for (int i = 0; i < cardsIndexes.Count; i++) {
			SpawnCard (cardsIndexes [i],cardsPlaces [i].x, cardsPlaces [i].y);
		}
	}

	private void GetCardsPositions (){
		cardsIndexes = new List<int>(new int[]{0,1,2,3,4,5,6,7});
		cardsPlaces = new List<IntInt> ();
		cardsPlaces.Add (new IntInt (1, 1));
		cardsPlaces.Add (new IntInt (1, 2));
		cardsPlaces.Add (new IntInt (1, 3));
		cardsPlaces.Add (new IntInt (1, 4));
		cardsPlaces.Add (new IntInt (2, 1));
		cardsPlaces.Add (new IntInt (2, 2));
		cardsPlaces.Add (new IntInt (2, 3));
		cardsPlaces.Add (new IntInt (2, 4));
	}


}


// test














