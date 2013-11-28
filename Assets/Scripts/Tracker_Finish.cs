﻿using UnityEngine;
using System.Collections;

public class Tracker_Finish : MonoBehaviour {
	
	public Tracker_Spawn trackerSpawn;		// Need this to know how many chips we start with
	public int chipsNeeded;					// number of chips to finish the level
	public int goToLevel;
	public Transform cameraPositionsParent;		// get an array of this gameobjects children to move camera to next level
	
	Transform[] cameraPositionsArray;
	TextMesh needChipsText;					// "Needs x" Tracks how many Chips we need to move to next level
	int currentLevelInt;					// Tracks what level we are on
	int chipsCount;							// Tracks remaining chips needed for this level
	
	void OnEnable()			
    {
        Messenger.AddListener("reset", OnReset);			// Register to the reset event on enable
    }
	
	void OnDisable()
    {
        Messenger.RemoveListener("reset", OnReset);			// Always make sure to unregister the event on disable
    }
	
	void Start () 
	{
		
		needChipsText = GetComponent<TextMesh>();
		chipsCount = chipsNeeded;
		// Set chipsNeeded text
		needChipsText.text = "need " + chipsCount + " more chips";
		// make an array to hold the cameraPositions transform. Will use to move camera to next level
		cameraPositionsArray = new Transform[cameraPositionsParent.childCount];
		for (var i=0; i < cameraPositionsParent.childCount; i++){
			cameraPositionsArray[i] = cameraPositionsParent.GetChild(i);
		}
		if (goToLevel != 0) LeanTween.move( Camera.main.gameObject, cameraPositionsArray[goToLevel].position, 4f, new object[]{ "ease",LeanTweenType.easeInOutSine});

	}

	void LevelComplete()
	{
		currentLevelInt += 1;
		Messenger.Invoke("reset");
		LeanTween.move( Camera.main.gameObject, cameraPositionsArray[currentLevelInt].position, 4f, new object[]{ "ease",LeanTweenType.easeInOutSine});
	}
	
	void UpdateText()
	{
		needChipsText.text = "need " + chipsCount + " more chips";
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		if (col.gameObject.CompareTag("Chip"))
		{
			// Tell chip to despawn
			col.gameObject.SendMessage("FinishedDespawn",SendMessageOptions.DontRequireReceiver);
			// Reduce chipsNeeded count by 1
			chipsCount -= 1;
			UpdateText();
			if (chipsCount == 0)
				LevelComplete();
		}
	}
	
	void OnReset()
	{
		chipsCount = chipsNeeded;
		UpdateText();
	}
}
