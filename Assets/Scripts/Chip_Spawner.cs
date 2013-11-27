﻿using UnityEngine;
using System.Collections;

public class Chip_Spawner : MonoBehaviour {

	public GameObject shape;
	
	Drag_Chip dragChip;
	Transform thisTransform;
	Vector3 originalLocalPosition;
	Vector3 onScreenPosition;
	Vector3 spawnPoint;
	bool isReady;
	bool isActivated;
	int spawnLayer;
	bool isOnScreen;
	
	void OnEnable()			
    {
        Messenger.AddListener("reset", OnReset);			// Register to the reset event on enable
    }
	
	void OnDisable()
    {
        Messenger.RemoveListener("reset", OnReset);			// Always make sure to unregister the event on disable
    }
	
	void Start()
	{
		// cache these things for performance reasons
		thisTransform = transform;
		originalLocalPosition = thisTransform.localPosition;
		onScreenPosition = new Vector3 (thisTransform.localPosition.x+1.5f,thisTransform.localPosition.y,thisTransform.localPosition.z);
		// reference Drag_Chip script, will need it to check if we are dragging this shape
		dragChip = GetComponent<Drag_Chip>();
		// setup a bitmask of the spawnlayer for a raycast check (can only spawn while within a spawn collider)
		spawnLayer = 1 << 11;
		// Wait 2 seconds before doing anything
		Invoke("SetIsReady", 2f);
	}
	
	void SetIsReady()
	{
		isReady = true;
	}
	
	void SetIsOnScreen()		// Called by Tracker_Spawn when this is moved on screen
	{
		isOnScreen = true;
	}
	
	void Update()
	{
		if(!isOnScreen) return;
		// If the user clicks down on the mouse and we havent done this before
		if(Input.GetMouseButtonUp(0) && isReady && !isActivated && dragChip.isDragging)
		{
			print("!@");
			// Use ScreenPointToRay to get world position of mouse, this will be the spawnPoint
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection(ray,1000f,spawnLayer);
			
			// If the mouse position hits a spawn collider spawn our shape
			if (hit.collider != null)
			{
				isActivated = true;
				spawnPoint = ray.origin + (ray.direction * -Camera.main.gameObject.transform.position.z);
		    	spawnPoint = new Vector3(spawnPoint.x,spawnPoint.y,0f);
			
				// transition tween moving spawner gameobject to spawn position of shape gameobject 
				LeanTween.move( gameObject, spawnPoint, .7f, new object[]{ "ease",LeanTweenType.easeInSine});
			
				Invoke("SpawnShapeAndMoveThis",.75f);
			}
			else
			{
				// Move the spawn shape back to its original position so the user can try and drag it again
				LeanTween.moveLocal( gameObject, onScreenPosition, .7f, new object[]{ "ease",LeanTweenType.easeInSine});
			}
		}
	}
	
	void SpawnShapeAndMoveThis()
	{
		// Spawn the actual shape where the mouse is
		Instantiate(shape, spawnPoint, transform.rotation);
		// Announce that we have successfully spawned
		Messenger.Invoke ("chipSpawned");
		// move this gameobject to it's original position off screen
		thisTransform.localPosition = originalLocalPosition;
		isActivated = false;
		isOnScreen = false;
	}
	
	void OnReset()
	// Resets this script completely for a new level or user reset
	{
		isActivated = false;
		print("reset2");
	}
}