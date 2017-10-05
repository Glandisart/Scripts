using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidCard : NormalCard {
	public override bool[,] PossibleMoves ()
	{
		bool[,] r = new bool[BoardManager.Instance.LargeurPlateau, BoardManager.Instance.HauteurPlateau];
		r = LigneDroitePossibleMoves (r, 1, 1);//Vertical haut
		return r;
	}

}
