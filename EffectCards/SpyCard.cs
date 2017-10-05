using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyCard : NormalCard {
	public int SpyLevel;
	public override string WinsWhenAttacks (Card c)
	{
		if (this.Force > c.Force || c.Force>SpyLevel)
			return "1";
		else if (this.Force < c.Force)
			return "0";
		else
			return "1/2";
	}


}
