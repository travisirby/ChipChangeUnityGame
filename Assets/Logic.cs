﻿using UnityEngine;
using System.Collections;

public class Logic : MonoBehaviour {

	public GameObject testSprite;

	void Start () {
	
	}
	
	void Update () {
	}

	void OnMouseUp()
	{
		RaycastHit2D ray = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (ray.collider != null && ray.collider.transform == this.gameObject.transform)
			Instantiate(testSprite, transform.position, transform.rotation);
	}
}