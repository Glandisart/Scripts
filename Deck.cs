using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
	public Card[,] CardsAndPlaces;

	public List<Card> Cartes;
	public List<IntInt> Emplacements;
	public List<int> Abscisse;
	public List<int> Ordonnées;
	public List<bool> PorteurDrapeau;

	public void GetCardsAndPlaces(){
		CardsAndPlaces = new Card[BoardManager.Instance.LargeurPlateau,BoardManager.Instance.HauteurTerritoire];
		for(int i = 0; i<Cartes.Count;i++) {
			//Debug.Log (i.ToString (), Abscisse [i].ToString());
			this.CardsAndPlaces [Abscisse [i], Ordonnées [i]] = Cartes [i];
			CardsAndPlaces [Abscisse [i], Ordonnées [i]].PorteDrapeau = PorteurDrapeau [i];
		}
	}

	public float Valeur(){
		float result = 0;
		foreach (Card c in this.CardsAndPlaces) {
			if (c != null) {
				result += c.Valeur;
			}
		}
		return result;
	}
	public int NbDrapeaux(){
		return 0;
	}

}

/*Card[,] result = new Card[BoardManager.Instance.LargeurPlateau,BoardManager.Instance.HauteurTerritoire]();
		for (int i = 0; i < BoardManager.Instance.LargeurPlateau; i++) {
			for (int j = 0; j < BoardManager.Instance.HauteurTerritoire; j++) {
				result[i,j] = 
			}
		}*/