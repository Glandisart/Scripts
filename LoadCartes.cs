using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class LoadCartes : MonoBehaviour {

	public NormalCard CardToSave;

	// Use this for initialization
	void Start () {
		//AssetDatabase.ImportPackage ("C:\\Users\\RADES_Asus_2\\Documents\\JeuPayToWin\\CartesData\\Cartesv2",false);
		SaveImages ("Assets\\Resources\\CartesImportées\\Visuel", "Assets\\Resources\\CartesDuJeu\\Visuel");
		SaveCard2AsCard ("CartesImportées\\Données","Assets/Resources/CartesDuJeu/Données/");

	}
	
	public void SaveCard2AsCard(string PathCard2, string PathCard){
		foreach(NormalCard2 c2 in Resources.LoadAll<NormalCard2>(PathCard2)){
			CardToSave.Valeur = c2.Valeur;
			CardToSave.Force = c2.Force;
			CardToSave.Deplacement = c2.Deplacement;
			CardToSave.ImagePath = c2.ImagePath;
			CardToSave.ImageSprite = c2.ImageSprite;
			CardToSave.Revelee = c2.Revelee;
			CardToSave.Name = c2.Name;
			CardToSave.Description = c2.Description;
			CardToSave.Id = c2.Id;
			CardToSave.ShortName = c2.ShortName;
			CardToSave.VisuelPath = c2.ShortName;
			CardToSave.VisuelSprite = Resources.Load<Sprite> ("CartesImportées/Visuel/"+c2.ShortName);
			CardToSave.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("CartesImportées/Visuel/"+c2.ShortName);
			if(CardToSave.Name != "")
				PrefabUtility.CreatePrefab (PathCard + CardToSave.ShortName + ".prefab", CardToSave.gameObject);
				
		}

	}
	public void SaveImages(string PathSprite2, string PathSprite){
		foreach(string f in Directory.GetFiles(PathSprite2)){
			FileInfo fi = new FileInfo (f);
			File.Copy (fi.FullName, PathSprite + "\\" + Path.GetFileName (fi.FullName),true);
		}
	}
}
