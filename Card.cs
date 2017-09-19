using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
	public int CurrentX{ get; set;}
	public int CurrentY{ get; set;}
	public bool IsBlue;
	public int Deplacement;
	public int Force;
	public bool Revelee;

	public void SetPosition(int x, int y){
		CurrentX = x;
		CurrentY = y;
	}

	public virtual string WinsWhenAttacks(Card c){
		if (this.Force > c.Force)
			return "1";
		else if (this.Force < c.Force)
			return "0";
		else
			return "1/2";
	}

	public virtual bool[,] PossibleMoves(){
		bool[,] r = new bool[10, 10];

		for (int i = 1; i <= Deplacement; i++) {
			try{
				Card c = BoardManager.Instance.CardBoard [CurrentX, CurrentY + i];
				if (c == null || c.IsBlue!=this.IsBlue) {
					r [CurrentX, CurrentY + i] = true;
				} else {
					i = Deplacement + 1;
				}
			}
			catch{
				i = Deplacement + 1;
			}
		}
		for (int i = 1; i <= Deplacement; i++) {
			try{
				Card c = BoardManager.Instance.CardBoard [CurrentX, CurrentY - i];
				if (c == null || c.IsBlue!=this.IsBlue) {
					r [CurrentX, CurrentY - i] = true;
				} else {
					i = Deplacement + 1;
				}
			}
			catch{
				i = Deplacement + 1;
			}
		}
		for (int i = 1; i <= Deplacement; i++) {
			try{
				Card c = BoardManager.Instance.CardBoard [CurrentX + i , CurrentY];
				if (c == null || c.IsBlue!=this.IsBlue) {
					r [CurrentX + i, CurrentY] = true;
				} else {
					i = Deplacement + 1;
				}
			}
			catch{
				i = Deplacement + 1;
			}
		}
		for (int i = 1; i <= Deplacement; i++) {
			try{
				Card c = BoardManager.Instance.CardBoard [CurrentX - i , CurrentY];
				if (c == null || c.IsBlue!=this.IsBlue) {
					r [CurrentX - i, CurrentY] = true;
				} else {
					i = Deplacement + 1;
				}
			}
			catch{
				i = Deplacement + 1;
			}
		}

		return r;
	}
}
