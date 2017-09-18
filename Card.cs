using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
	public int CurrentX{ get; set;}
	public int CurrentY{ get; set;}
	public bool IsBlue;
	public int Deplacement=1;

	public void SetPosition(int x, int y){
		CurrentX = x;
		CurrentY = y;
	}

	public virtual bool[,] PossibleMoves(){
		/*if (BoardManager.CardBoard [x, y] != null)
			return false;*/
		return new bool[10,10];
	}
}
