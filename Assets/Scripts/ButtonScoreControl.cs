using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScoreControl : MonoBehaviour {

	public Sprite unHatched;
	public Sprite hatched;

	public Image egg1;
	public Image egg2;
	public Image egg3;

	private int buttonLevelIndex;
	private int scoreEggsHatched;

	// Use this for initialization
	void Start () 
	{
		print("The button is called " +gameObject.name);
		buttonLevelIndex	= GameManager.instance.data1.level.FindIndex (x => x.levelName == gameObject.name);
		if (buttonLevelIndex < 0)
		{
			scoreEggsHatched = 0;
		} else
		{
			scoreEggsHatched = GameManager.instance.data1.level [buttonLevelIndex].puffletsHatched;
		}

		if(scoreEggsHatched >=1)
		{
			egg1.sprite = hatched;
		}
		if(scoreEggsHatched >=2)
		{
			egg2.sprite = hatched;
		}
		if(scoreEggsHatched >=3)
		{
			egg3.sprite = hatched;
		}
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
