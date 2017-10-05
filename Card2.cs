using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card2 : MonoBehaviour {
	public int CurrentX{ get; set;}
	public int CurrentY{ get; set;}
	public bool IsBlue;
	public int Deplacement;
	public int Force;
	public bool Revelee;
	public float Valeur;
	public bool PorteDrapeau;
	public string Name;
	public string ShortName;
	public int Id;
	public string Description;
	public string ImagePath;
	public Sprite ImageSprite;
	public string VisuelPath;
	public Sprite VisuelSprite;

	public void SetPosition(int x, int y){
		CurrentX = x;
		CurrentY = y;
	}

	public virtual string WinsWhenAttacks(Card2 c){
		if (this.Force > c.Force)
			return "1";
		else if (this.Force < c.Force)
			return "0";
		else
			return "1/2";
	}
		


}
	