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

	public virtual bool[,] LigneDroitePossibleMoves(bool[,] r, int bLargeur0Hauteur1, int bHautDroite1BasGaucheMoins1){
		bool[,] r2 = new bool[BoardManager.Instance.LargeurPlateau, BoardManager.Instance.HauteurPlateau];
		r2 = r;
		for (int i = 1; i <= Deplacement; i++) {
			try{
				Card c = BoardManager.Instance.CardBoard [CurrentX + (1-bLargeur0Hauteur1)*bHautDroite1BasGaucheMoins1*i, CurrentY + bLargeur0Hauteur1*bHautDroite1BasGaucheMoins1*i];
				if (c == null) {
					r2 [CurrentX + (1-bLargeur0Hauteur1)*bHautDroite1BasGaucheMoins1*i, CurrentY + bLargeur0Hauteur1*bHautDroite1BasGaucheMoins1*i] = true;
				} else {
					if(c.IsBlue!=this.IsBlue && c.GetType() != typeof(Obstacle))
						r2 [CurrentX + (1-bLargeur0Hauteur1)*bHautDroite1BasGaucheMoins1*i, CurrentY + bLargeur0Hauteur1*bHautDroite1BasGaucheMoins1*i] = true;
					i = Deplacement + 1;
				}
			}
			catch{
				i = Deplacement + 1;
			}
		}
		return r2;
	}

	public virtual bool[,] PossibleMoves(){
		bool[,] r = new bool[BoardManager.Instance.LargeurPlateau, BoardManager.Instance.HauteurPlateau];

		r = LigneDroitePossibleMoves (r, 1, 1);
		r = LigneDroitePossibleMoves (r, 1, -1);
		r = LigneDroitePossibleMoves (r, 0, 1);
		r = LigneDroitePossibleMoves (r, 0, -1);

		return r;
	}
}
	