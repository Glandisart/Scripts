using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck : MonoBehaviour{
	public Card[,] CardsAndPlaces;

	public List<Card> Cartes;
	public List<IntInt> Emplacements;
	public List<int> CartesId;
	public List<int> Abscisses;
	public List<int> Ordonnées;
	public List<bool> PorteurDrapeau;

	public void GetCardsAndPlaces(){
		CardsAndPlaces = new Card[BoardManager.Instance.LargeurPlateau,BoardManager.Instance.HauteurTerritoire];
		for(int i = 0; i<Cartes.Count;i++) {
			//Debug.Log (i.ToString (), Abscisse [i].ToString());
			this.CardsAndPlaces [Abscisses [i], Ordonnées [i]] = Cartes [i];
			CardsAndPlaces [Abscisses [i], Ordonnées [i]].PorteDrapeau = PorteurDrapeau [i];
		}
	}

	public void GetCardsAndPlacesById(){
		CardsAndPlaces = new Card[BoardManager.Instance.LargeurPlateau,BoardManager.Instance.HauteurTerritoire];
		for(int i = 0; i<CartesId.Count;i++) {
			//Debug.Log (i.ToString (), Abscisse [i].ToString());
			this.CardsAndPlaces [Abscisses [i], Ordonnées [i]] = BoardManager.Instance.AllCards[CartesId[i]].GetComponent<Card>();
			CardsAndPlaces [Abscisses [i], Ordonnées [i]].PorteDrapeau = PorteurDrapeau [i];
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
		int result=0;
		foreach (bool b in PorteurDrapeau)
			if (b)
				result++;
		return result;
	}

	public bool IsCorrect(){
		if (this.NbDrapeaux () > 0 && this.Valeur () <= BoardManager.Instance.ValeurDeckMax)
			return true;
		else
			return false;
	}

}
	