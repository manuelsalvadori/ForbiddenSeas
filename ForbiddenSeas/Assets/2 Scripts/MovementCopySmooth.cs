﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCopySmooth : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	private float velocity=0f;
	public float smooth=1f;

	void Start () {
		transform.position = player.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = player.transform.position;
		Vector3 EulerAngles = transform.rotation.eulerAngles;
		EulerAngles.y = Mathf.SmoothDampAngle (EulerAngles.y, player.transform.eulerAngles.y, ref velocity, smooth);
		transform.rotation = Quaternion.Euler (EulerAngles);
	}
}
