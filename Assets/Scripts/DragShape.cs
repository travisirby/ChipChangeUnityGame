﻿using UnityEngine;
using System.Collections;


public class DragShape : MonoBehaviour
{
	private Transform thisTransform;
	private Vector3 screenPoint;
	private Vector3 offset;
	private Vector3 originalPos;
	private bool isReady;
	private bool isActivated;
	private RaycastHit2D[] hit2DArray = new RaycastHit2D[1];
	[System.NonSerialized]
	public bool isDragging; 
	
	
	void Start()
	{
		thisTransform = transform;
		originalPos = thisTransform.position;
		// Wait 2 seconds before doing anything
		Invoke("SetIsReady",2f);
	}

	void SetIsReady()
	{
		isReady = true;
	}

	void Update()
	{
		if(!isReady) return;
		
		// Cant use OnMouseDown() etc in flash. Would have been easier and avoided raycasting
		if (Input.GetMouseButtonDown(0) && !isActivated)
		{
			// Shoot a ray at mousePosition to see what 2d collider is hit
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
			
			// If it hit this gameObject, we will activate the dragging
			if(hit.collider != null && hit.collider.transform == thisTransform)
			{
				isActivated = true;
				screenPoint = Camera.main.WorldToScreenPoint(thisTransform.position);
			}
		}
		
		// Stop dragging when the mouse is unclicked
		if (Input.GetMouseButtonUp(0) && isActivated)
		{
			isActivated = false;
			isDragging = false;
		}
		
		// If if the mouse is still clicked, drag this gameObject to its location
		if (isActivated)
		{
			isDragging = true;
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
			thisTransform.position = curPosition;
		}
	}

}