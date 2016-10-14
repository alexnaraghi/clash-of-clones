using UnityEngine;
using System.Collections;

public class MaterialChange : MonoBehaviour 
{
	public Material[] mats;
	public GameObject footman;



	void Awake () 
	{
		footman.GetComponent<Renderer>().material = mats[0];
	}





	public void MatChangeToYellow ()
	{
		footman.GetComponent<Renderer>().material = mats[0];
	}

	public void MatChangeToRed ()
	{
		footman.GetComponent<Renderer>().material = mats[1];
	}

	public void MatChangeToBlue ()
	{
		footman.GetComponent<Renderer>().material = mats[2];
	}

	public void MatChangeToGreen ()
	{
		footman.GetComponent<Renderer>().material = mats[3];
	}
	
}
