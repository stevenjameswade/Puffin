using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveReposition : MonoBehaviour {


	private BoxCollider2D waveCollider;
	private float waveHorizontalLength;

	//private waveMovement moving;
	private int direction;
	public bool scrollRight;
	private float startpositionX;


	// Use this for initialization
	void Start () 
	{
		
		startpositionX = transform.position.x; 
		//moving = GetComponent<waveMovement>;
		//direction = moving.direction;

		direction = scrollRight== true ? 1 : -1;
		waveCollider = GetComponent<BoxCollider2D> ();
		waveHorizontalLength = waveCollider.size.x * transform.parent.parent.localScale.x;
		print (waveCollider.size.x);

		//print (startpositionX + waveHorizontalLength * 2f);
	}

	// Update is called once per frame
	void Update () 
	{// this logic doesn't work %100
		if (direction==-1 && transform.position.x < startpositionX - waveHorizontalLength*.5f)//-waveHorizontalLength*5f)
		{
			RepositionBackground ();
		}
		else if (direction==1 && transform.position.x > startpositionX + waveHorizontalLength*.5f)
		{
			RepositionBackground ();
			print ("Reposition true");
		}

		/*
		 * 
		 if (direction==-1 && transform.position.x < 0f)//-waveHorizontalLength*5f)
		{
			RepositionBackground ();
		}
		else if (direction==1 && transform.position.x > waveHorizontalLength*5f)
		{
			RepositionBackground ();
		}
		 */
	}

	private void RepositionBackground()
	{
		//print (waveHorizontalLength);
		Vector2 waveOffset = new Vector2 (waveHorizontalLength * 1f, 0);
		transform.position = (Vector2)transform.position - waveOffset*direction;
	}
}