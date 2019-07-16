using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveToPosition : MonoBehaviour {

    // Use this for initialization
    public Transform Target;
    public bool IsMoving = false;
    public float Speed;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(IsMoving)
        {
            // The step size is equal to speed times frame time.
            float step = Speed * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, Target.position, step);
        }
	}
}
