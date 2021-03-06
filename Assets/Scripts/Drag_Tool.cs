﻿using UnityEngine;
using System.Collections;

public class Drag_Tool : MonoBehaviour
{
	public bool rotateInsteadOfDrag;
	public bool limitVerticalDrag;
	public bool limitHorizontalDrag;
	
	public float maxVerticalDrag;
	public float minVerticalDrag;
	public float maxHorizontalDrag;
	public float minHorizontalDrag;
	
	Transform thisTransform;
	Vector3 screenPoint;
	Vector3 offset;
	Vector3 originalPosition;
	Quaternion originalRotation;
	float angleOffset;
	bool isReady;
	bool isActivated;
	[System.NonSerialized]
	public bool isDragging; 

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
		thisTransform = transform;
		originalPosition = thisTransform.position;
		originalRotation = thisTransform.rotation;
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
				if (rotateInsteadOfDrag)
				{
					Vector3 v3 = Input.mousePosition - screenPoint;
					angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(v3.y, v3.x))  * Mathf.Rad2Deg;
				}
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
			
			Vector3 newPosition = thisTransform.position;			
			
			if (rotateInsteadOfDrag)
			{
				Vector3 v3 = Input.mousePosition - screenPoint;
				float angle = Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0,0,angle+angleOffset); 
			}
			
			if (limitHorizontalDrag)
				newPosition.x = Mathf.Clamp(curPosition.x,originalPosition.x - minHorizontalDrag, originalPosition.x + maxHorizontalDrag);
			else newPosition.x = curPosition.x;
			
			if (limitVerticalDrag)
				newPosition.y = Mathf.Clamp(curPosition.y,originalPosition.y - minVerticalDrag, originalPosition.y + maxVerticalDrag);
			else newPosition.y = curPosition.y;
			
			thisTransform.position = newPosition;
		}
	}

	void OnReset()
	{
		thisTransform.position = originalPosition;
		thisTransform.rotation = originalRotation;
	}

}