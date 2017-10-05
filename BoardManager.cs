using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Linq;

public class BoardManager : MonoBehaviour 
{
	public Deck deckRouge;
	public Deck deckBleu;
	public bool GameEnCours;

	public int LargeurPlateau = 11;
	public int HauteurPlateau = 10;
	public int HauteurTerritoire = 4;
	public int LargeurCamp = 3;
	public int AbscisseGaucheCamp = 4;
	public int HauteurCamp = 2;
	public int ValeurDeckMax = 50;
	public bool JoueurBleu = true;

	public static BoardManager Instance{ get; set; }
	private bool [,] allowedMoves{ get; set; }

	public Card[,] CardBoard{ get; set; } // Le plateau 
	private Card selectedCard; // La carte séléctionnée par le joueur 

	public const float TILE_SIZE = 1.0f;
	public const float TILE_OFFSET = 0.5f; // taille des cases

	private int selectionX = -1;
	private int selectionY = -1; // Coordonnées de l'emplacement de la souris

	public List<GameObject> obstacles;//Liste des obstacles placés sur le plateau
	public List<GameObject> AllCards; // Liste des cartes existantes
	private List<GameObject> activeCards = new List<GameObject>();// Liste des cartes présentes dans le jeu
	//private List<GameObject> cimetiereBleu = new List<GameObject>();
	//private List<GameObject> cimetiereRouge = new List<GameObject>();

	//private List<int> cardsIndexes = new List<int>(); // Cartes initialement présentes
	//private List<IntInt> cardsPlaces = new List<IntInt> ();// Emplacement des cartes initialement présentes 

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
		if (CardBoard [x, y] == null) 
			return;
		if (CardBoard [x, y].IsBlue != isBlueTurn)
			return;
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
					//win si drapeau détruit
					if (CardBoard [x, y].PorteDrapeau)
						EndGame (CardBoard [x, y].IsBlue ? "rouge MangeDrapeau" : "bleu MangeDrapeau");
					DestroyImmediate (CardBoard[x,y].gameObject);
					CardBoard[selectedCard.CurrentX, selectedCard.CurrentY] = null;
					selectedCard.transform.position = GetTileCenter (x, y);
					selectedCard.SetPosition (x, y);
					CardBoard [x, y] = selectedCard;
					selectedCard.transform.Rotate (0,(selectedCard.IsBlue && JoueurBleu) ? 0:(selectedCard.Revelee ? 0:180) , 0);
					selectedCard.Revelee = true;
				} else if (selectedCard.WinsWhenAttacks (CardBoard [x, y]) == "0") {
					activeCards.Remove (selectedCard.gameObject);
					//win si drapeau détruit
					if (selectedCard.PorteDrapeau)
						EndGame (selectedCard.IsBlue ? "rouge MangeDrapeau" : "bleu MangeDrapeau");
					DestroyImmediate(selectedCard.gameObject);
					CardBoard[x,y].transform.Rotate (0,(CardBoard[x,y].IsBlue && JoueurBleu) ? 0:(CardBoard[x,y].Revelee ? 0:180) , 0);
					CardBoard [x, y].Revelee = true;
				} else {
					activeCards.Remove (CardBoard [x, y].gameObject);
					//win si drapeau détruit
					if (selectedCard.PorteDrapeau)
						EndGame (selectedCard.IsBlue ? "rouge MangeDrapeau" : "bleu MangeDrapeau");
					if (CardBoard [x, y].PorteDrapeau)
						EndGame (CardBoard [x, y].IsBlue ? "rouge MangeDrapeau" : "bleu MangeDrapeau");
					DestroyImmediate(CardBoard[x,y].gameObject);
					activeCards.Remove (selectedCard.gameObject);
					DestroyImmediate(selectedCard.gameObject);
					try{
						selectedCard.transform.Rotate (0,(selectedCard.IsBlue && JoueurBleu) ? 0:(selectedCard.Revelee ? 0:180) , 0);
						CardBoard[x,y].transform.Rotate (0,(CardBoard[x,y].IsBlue && JoueurBleu) ? 0:(CardBoard[x,y].Revelee ? 0:180) , 0);
						selectedCard.Revelee=true;
						CardBoard[x,y].Revelee = true;
					}
					catch{}
				}
			}
			isBlueTurn = !isBlueTurn;
			CheckVictory();
		}
		BoardHighlights.Instance.HideHighlights ();
		selectedCard = null;
	}

	private void CheckVictory(){
		if(selectedCard != null && BroughtDrapeau(selectedCard)) 
			EndGame (isBlueTurn ? "rouge BroughtDrapeau" : "bleu BroughtDrapeau");
		if (!CanPlay ())
			EndGame (isBlueTurn ? "rouge CanPlay" : "bleu CanPlay");
	} 

	private void Start(){ // Fonction qui s'éxecute au lancement
		AllCards = Resources.LoadAll<GameObject>("CartesDuJeu/Données").ToList();
		Instance=this;
		GameEnCours = true;
		CardBoard = new Card[LargeurPlateau,HauteurPlateau];
		//SpawnAllObstacles ();
		SpawnDecks ();
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

	/*private void SpawnCard(int index, int x, int y){
		//Debug.Log (cards.ToArray().Length.ToString());
		GameObject go = Instantiate (cards [index], GetTileCenter(x,y),Quaternion.Euler(0,180,0) ) as GameObject;
		go.transform.SetParent (transform);
		CardBoard [x, y] = go.GetComponent<Card> ();
		CardBoard [x, y].SetPosition (x, y);
		activeCards.Add (go);
	}*/

	private Vector2 GetTileCenter(int x, int y){
		Vector2 origin = Vector2.zero;
		origin.x += (TILE_SIZE * x) + TILE_OFFSET;
		origin.y += (TILE_SIZE * y) + TILE_OFFSET;
		return origin;
	}

	/*private void SpawnAllCards(){
		GetCardsPositions ();
		CardBoard = new Card[LargeurPlateau,HauteurPlateau];
		SpawnAllObstacles ();
		for (int i = 0; i < cardsIndexes.Count; i++) {
			SpawnCard (cardsIndexes [i],cardsPlaces [i].x, cardsPlaces [i].y);
		}
	}*/

	private void SpawnDecks(){
		CardBoard = new Card[LargeurPlateau,HauteurPlateau];
		deckBleu.GetCardsAndPlacesById ();
		foreach (Card c in deckBleu.CardsAndPlaces)
			if(c!=null)
				c.IsBlue = true;
		deckRouge.GetCardsAndPlacesById ();
		foreach (Card c in deckRouge.CardsAndPlaces)
			if(c!=null)
				c.IsBlue = false;
		//Deck bleu
		for (int i = 0; i < LargeurPlateau; i++) {
			for (int j = 0; j < HauteurTerritoire; j++) {
				try{
					GameObject c = deckBleu.CardsAndPlaces[i,j].gameObject;
					GameObject go = Instantiate (c, GetTileCenter(i,j),Quaternion.Euler(0,(c.GetComponent<Card>()).Revelee? 180:(!JoueurBleu ? 0:180),0) ) as GameObject;
					go.transform.SetParent (transform);
					CardBoard [i, j] = go.GetComponent<Card> ();
					CardBoard [i, j].SetPosition (i, j);
					CardBoard [i, j].transform.localScale = new Vector3((float)0.25,(float)0.25,1);
					CardBoard [i, j].IsBlue = true;
					if(CardBoard[i,j].PorteDrapeau)CardBoard[i,j].Deplacement=1;
					activeCards.Add (go);
				}
				catch{}
			}
		}
		//Deck rouge
		for (int i = 0; i < LargeurPlateau; i++) {
			for (int j = HauteurPlateau - HauteurTerritoire; j < HauteurPlateau; j++) {
				try{
					GameObject c=deckRouge.CardsAndPlaces[LargeurPlateau-i-1,HauteurPlateau-j-1].gameObject;
					GameObject go = Instantiate (c, GetTileCenter(i,j),Quaternion.Euler(0,(c.GetComponent<Card>()).Revelee? 180:(JoueurBleu ? 0:180),0) ) as GameObject;
					go.transform.SetParent (transform);
					CardBoard [i, j] = go.GetComponent<Card> ();
					CardBoard [i, j].SetPosition (i, j);
					CardBoard [i, j].transform.localScale = new Vector3((float)0.25,(float)0.25,1);
					CardBoard [i, j].IsBlue = false;
					if(CardBoard[i,j].PorteDrapeau)CardBoard[i,j].Deplacement=1;
					activeCards.Add (go);
				}
				catch{}
			}
		}
	}

	public void SpawnObstacle(int x, int y){
		GameObject go = Instantiate(obstacles [0], GetTileCenter(x,y),Quaternion.Euler(0,180,0)) as GameObject;
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

	public void EndGame(string winner){
		if (GameEnCours == false)
			return;
		if (winner.Contains("bleu"))
			Debug.Log ("bleu gagne");
		if (winner.Contains("rouge"))
			Debug.Log ("rouge gagne");
		GameEnCours = false;
		SceneManager.LoadScene ("Menu");
	}

	public bool CanPlay(){
		foreach (Card c in CardBoard) {
			try{
				if (c.IsBlue == isBlueTurn && c!=null) {
					foreach(bool move in c.PossibleMoves()){
						if (move)
							return true;
					}
				}
			}
			catch{}
		}
		return false;
	}

	public bool BroughtDrapeau(Card c){
		if (!c.PorteDrapeau)
			return false;
		if (!c.IsBlue && (c.CurrentX < AbscisseGaucheCamp ||c.CurrentX >= AbscisseGaucheCamp + LargeurCamp || c.CurrentY>=HauteurCamp))
			return false;
		if (c.IsBlue && (c.CurrentX < AbscisseGaucheCamp ||c.CurrentX >= AbscisseGaucheCamp + LargeurCamp || c.CurrentY<=HauteurPlateau-HauteurCamp-1))
			return false;
		return true;
	}

	/*private void GetCardsPositions (){
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
	}*/

}














