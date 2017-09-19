using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
	public int CurrentX{ get; set;}
	public int CurrentY{ get; set;}
	public bool IsBlue;
	public int Deplacement;

	public void SetPosition(int x, int y){
		CurrentX = x;
		CurrentY = y;
	}

	public virtual bool[,] PossibleMoves(){
		bool[,] r = new bool[10, 10];

		for (int i = 1; i <= Deplacement; i++) {
			try{
				if (BoardManager.Instance.CardBoard [CurrentX, CurrentY + i] == null) {
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
				if (BoardManager.Instance.CardBoard [CurrentX, CurrentY - i] == null) {
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
				if (BoardManager.Instance.CardBoard [CurrentX + 1, CurrentY] == null) {
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
				if (BoardManager.Instance.CardBoard [CurrentX - 1, CurrentY] == null) {
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
