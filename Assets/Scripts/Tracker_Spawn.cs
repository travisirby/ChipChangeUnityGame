﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class Tracker_Spawn: MonoBehaviour {
	
	Transform[] childrenArray;
	Text_Typewriter textEffect;
	int totalSpawnCount;
	int currentSpawnCount;
	Vector3 spawnerMovePosition;
	
	void OnEnable()			
    {
        Messenger.AddListener("reset", OnReset);			// Register to the reset event on enable
		Messenger.AddListener("chipSpawned", OnChipSpawned);
		Messenger.AddListener ("levelComplete", OnLevelComplete);
    }
	
	void OnDisable()
    {
        Messenger.RemoveListener("reset", OnReset);			// Always make sure to unregister the event on disable
		Messenger.RemoveListener("chipSpawned", OnChipSpawned);
		Messenger.RemoveListener ("levelComplete", OnLevelComplete);
    }
	
	void Start () 
	{
		// Linq statement that selects all the children of this gameobject *Oops LINQ doesnt work in flash which im going to build to
		//childrenArray = GetComponentsInChildren<Transform>().Except(new [] { transform }).Select(t=>t).ToArray();
		
		// Make an array of Transforms of this gameobjects children
		childrenArray = new Transform[transform.childCount];
		// add each child to the array
		for (var i=0; i < transform.childCount; i++){
			childrenArray[i] = transform.GetChild(i);
		}
		
		// Get the text mesh of this gameobject so we can update the score
		textEffect = GetComponent<Text_Typewriter>();
		// Give unity a bit of time to setup (usually a good idea)
		Invoke("Setup",1f);
	}
	
	void Setup()
	{
		// Get the number of children of this gameobject
		totalSpawnCount = childrenArray.Length;
		currentSpawnCount = totalSpawnCount;
		// Setup the spawn count text
		textEffect.ShowText("x" + (currentSpawnCount - 1), 0f);
		// Setup the position where spawn shapes will slide into (they start offscreen)
		spawnerMovePosition = new Vector3 (childrenArray[0].localPosition.x + 1.5f, childrenArray[0].localPosition.y, childrenArray[0].localPosition.z);
		// Move the first child spawn shape into position
		MoveChipSpawnerToScreen(0);
	}
	
	void MoveChipSpawnerToScreen(int spawnerToMove)
	{
		LeanTween.moveLocal( childrenArray[spawnerToMove].gameObject, spawnerMovePosition, .7f, new object[]{ "ease",LeanTweenType.easeOutSine});
	}
	
	void OnChipSpawned () 
	{
		// This method is called by the Chip_Spawner when they spawn a chip
		// Reduce our spawn count by 1.
		if (currentSpawnCount != 0)
			currentSpawnCount -= 1;
		// figure out which child we need to move in next.
		int spawnerToMove = childrenArray.Length - currentSpawnCount;
		// move em in (making sure the spawnerToMove is within the childrenArray size)
		if (spawnerToMove < childrenArray.Length)
		{
			MoveChipSpawnerToScreen(spawnerToMove);
		}
		if (currentSpawnCount >= 1)
		{
			// update spawn count text
			textEffect.TextToShow = "x" + (currentSpawnCount - 1);
			textEffect.RemoveText(true);
		}
	}
	
	void OnReset()
	{
		// Bring a new chip spawner on screen if the current spawn count is zero
		if (currentSpawnCount <= 0)
		{
			MoveChipSpawnerToScreen(0);
		}
		// Reset spawn count and text	
		currentSpawnCount = totalSpawnCount;
		textEffect.TextToShow = "x" + (currentSpawnCount - 1);
		textEffect.RemoveText (true);

	}

	void OnLevelComplete()
	{
		textEffect.RemoveText();
		Invoke("OnReset",6f);
	}
}
