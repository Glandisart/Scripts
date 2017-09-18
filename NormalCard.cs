using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCard : Card {
	
	public override bool[,] PossibleMoves ()
	{
		bool[,] r = new bool[10, 10];

		for (int i = 1; i <= Deplacement; i++) {
			if (BoardManager.Instance.CardBoard [CurrentX, CurrentY + i] == null) {
				r [CurrentX, CurrentY + i] = true;
			} else {
				i = Deplacement + 1;
			}
		}

		return r;
	}
}
