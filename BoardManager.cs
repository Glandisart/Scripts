using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// test
public class BoardManager : MonoBehaviour 
{
	public Deck deckRouge;
	public Deck deckBleu;

	public int LargeurPlateau = 11;
	public int HauteurPlateau = 10;
	public int HauteurTerritoire = 4;

	public static BoardManager Instance{ get; set; }
	private bool [,] allowedMoves{ get; set; }

	public Card[,] CardBoard{ get; set; } // Le plateau 
	private Card selectedCard; // La carte séléctionnée par le joueur 

	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f; // taille des cases

	private int selectionX = -1;
	private int selectionY = -1; // Coordonnées de l'emplacement de la souris

	public List<GameObject> obstacles;//Liste des obstacles placés sur le plateau
	public List<GameObject> cards; // Liste des cartes existantes
	private List<GameObject> activeCards = new List<GameObject>();// Liste des cartes présentes dans le jeu

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
		//Debug.Log (selectedCard.Revelee.ToString ());
	}
	private void MoveCard(int x, int y){
		if (allowedMoves[x,y]) {
			if (CardBoard [x, y] == null) {
				CardBoard[selectedCard.CurrentX, selectedCard.CurrentY] = null;
				selectedCard.transform.position = GetTileCenter (x, y);
				selectedCard.SetPosition (x, y);
				CardBoard [x, y] = selectedCard;
			}
			else  {
				if (selectedCard.WinsWhenAttacks (CardBoard [x, y]) == "1") {
					//Destroy an ennemy card
					activeCards.Remove (CardBoard [x, y].gameObject);
					Destroy(CardBoard[x,y].gameObject);
					CardBoard[selectedCard.CurrentX, selectedCard.CurrentY] = null;
					selectedCard.transform.position = GetTileCenter (x, y);
					selectedCard.SetPosition (x, y);
					CardBoard [x, y] = selectedCard;
					selectedCard.Revelee = true;
				} else if (selectedCard.WinsWhenAttacks (CardBoard [x, y]) == "0") {
					activeCards.Remove (selectedCard.gameObject);
					Destroy(selectedCard.gameObject);
					CardBoard [x, y].Revelee = true;
				} else {
					activeCards.Remove (CardBoard [x, y].gameObject);
					Destroy(CardBoard[x,y].gameObject);
					activeCards.Remove (selectedCard.gameObject);
					Destroy(selectedCard.gameObject);
					try{
						selectedCard.Revelee=true;
						CardBoard[x,y].Revelee = true;
					}
					catch{}
				}
			}
			isBlueTurn = !isBlueTurn;
		}

		BoardHighlights.Instance.HideHighlights ();
		selectedCard = null;
	}

	private void Start(){ // Fonction qui s'éxecute au lancement
		Instance=this;
		CardBoard = new Card[LargeurPlateau,HauteurPlateau];
		SpawnAllObstacles ();
		SpawnDecks ();
		//SpawnAllCards ();
	}

	private void DrawBoard()// Dessine le plateau et retourne là où est la souris
	{
		Vector2 widthLine = Vector2.right * LargeurPlateau;
		Vector2 heightLine = Vector2.up * HauteurPlateau;

		for (int i = 0; i <= HauteurPlateau; i++) 
		{
			Vector2 start = Vector2.up * i;
			Debug.DrawLine (start, start + widthLine);

			for (int j = 0; j <= LargeurPlateau; j++) 
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
		//Debug.Log (cards.ToArray().Length.ToString());
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
		CardBoard = new Card[LargeurPlateau,HauteurPlateau];
		SpawnAllObstacles ();
		for (int i = 0; i < cardsIndexes.Count; i++) {
			SpawnCard (cardsIndexes [i],cardsPlaces [i].x, cardsPlaces [i].y);
		}
	}

	private void SpawnDecks(){
		CardBoard = new Card[LargeurPlateau,HauteurPlateau];
		deckBleu.GetCardsAndPlaces ();
		deckRouge.GetCardsAndPlaces ();
		//Deck bleu
		for (int i = 0; i < LargeurPlateau; i++) {
			for (int j = 0; j < HauteurTerritoire; j++) {
				try{
				GameObject go = Instantiate (deckBleu.CardsAndPlaces[i,j].gameObject, GetTileCenter(i,j),Quaternion.Euler(0,180,0) ) as GameObject;
				go.transform.SetParent (transform);
				CardBoard [i, j] = go.GetComponent<Card> ();
				CardBoard [i, j].SetPosition (i, j);
				activeCards.Add (go);
				}
				catch{}
			}
		}
		//Deck rouge
		for (int i = 0; i < LargeurPlateau; i++) {
			for (int j = HauteurPlateau - HauteurTerritoire; j < HauteurPlateau; j++) {
				try{
					GameObject go = Instantiate (deckRouge.CardsAndPlaces[LargeurPlateau-i-1,HauteurPlateau-j-1].gameObject, GetTileCenter(i,j),Quaternion.Euler(0,180,0) ) as GameObject;
				go.transform.SetParent (transform);
				CardBoard [i, j] = go.GetComponent<Card> ();
				CardBoard [i, j].SetPosition (i, j);
				activeCards.Add (go);
				}
				catch{}
			}
		}
	}

	public void SpawnObstacle(int x, int y){
		GameObject go = Instantiate(obstacles [0], GetTileCenter(x,y),Quaternion.Euler(0,180,0) ) as GameObject;
		go.transform.SetParent (transform);
		CardBoard [x, y] = go.GetComponent<Card> ();
		CardBoard [x, y].SetPosition (x, y);
	}
	public void SpawnAllObstacles(){
		SpawnObstacle (3, 4);
		SpawnObstacle (4, 4);
		SpawnObstacle (6, 4);
		SpawnObstacle (7, 4);
		SpawnObstacle (3, 5);
		SpawnObstacle (4, 5);
		SpawnObstacle (6, 5);
		SpawnObstacle (7, 5);
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














