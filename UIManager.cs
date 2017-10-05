using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class UIManager : MonoBehaviour {
	public InputField ifForce;
	public InputField ifDeplacement;
	public InputField ifCout;
	public InputField ifNom;
	public InputField ifText;
	public Toggle tRevelee;
	public InputField ifImageCarte;
	public Image imImageCarte;
	public InputField ifShortName;

	public Text tForce;
	public Text tDeplacement;
	public Text tCout;
	public Text tNom;
	public Text tText;
	public Image imImageCarteAffichee;

	public NormalCard2 NewCard;
	public GameObject NewVisuelCard;
	public RawImage rimVisuel;
	public GameObject CoinHG;
	public GameObject CoinHD;
	public GameObject CoinBG;
	public GameObject CoinBD;

	public void UpdateCarte(){
		tForce.text = ifForce.text;
		tDeplacement.text = ifDeplacement.text;
		tCout.text = ifCout.text;
		tNom.text = ifNom.text;
		tText.text = ifText.text;
		if (tRevelee.isOn)
			tText.text = "Révélée.\n" + tText.text;
		try{
			imImageCarte.sprite = Resources.Load<Sprite> (ifImageCarte.text);
			imImageCarteAffichee.sprite = Resources.Load<Sprite> (ifImageCarte.text);
		}
		catch{}
	}

	public void SaveNewCard(){
		try{
			NewCard.Force = int.Parse (tForce.text);
			NewCard.Deplacement = int.Parse (tDeplacement.text);
			NewCard.Valeur = float.Parse (tCout.text);
		}
		catch{
			Debug.Log ("Inserez valeurs");
		}
		NewCard.ShortName = ifShortName.text;
		NewCard.VisuelPath = ifShortName.text;
		NewCard.ImageSprite = imImageCarteAffichee.sprite;
		NewCard.Name = tNom.text;
		NewCard.Description = tText.text;
		NewCard.Revelee = tRevelee.isOn;
		NewCard.ImagePath = ifImageCarte.text;

		if (NewCard.Name == "")
			return;
		
		PrefabUtility.CreatePrefab("Assets/Cards/Données/"+NewCard.ShortName+".prefab",NewCard.gameObject);
		//PrefabUtility.CreatePrefab("Assets/Cards/Visuel/"+NewCard.Name+".prefab",NewVisuelCard);
		

		StartCoroutine (ScreenShot ());

		
	}

	IEnumerator ScreenShot(){
		yield return new WaitForEndOfFrame();
		float h = (float)Screen.height;
		float w = (float)Screen.width;

		Texture2D mytexture = new Texture2D((int)(0.36*w),(int)(0.9*h));
		mytexture.ReadPixels (new Rect ((int)(0.58*w), (int)(0.05*h), (int)(0.36*w), (int)(0.91*h)), 0, 0,false);

		byte[] textureBuffer = mytexture.EncodeToPNG ();
		File.WriteAllBytes ("C:\\Users\\RADES_Asus_2\\Documents\\JeuPayToWin\\Cartes\\"+NewCard.ShortName+".png", textureBuffer);
		File.WriteAllBytes ("C:\\Users\\RADES_Asus_2\\Documents\\JeuPayToWin\\Créateur de cartes\\Assets\\Cards\\Visuel\\"+NewCard.ShortName+".png", textureBuffer);
		//Sprite.Create (mytexture, new Rect ((int)(0.58 * w), (int)(0.05 * h), (int)(0.36 * w), (int)(0.91 * h)), new Vector2 (0, 0));

	}

}
