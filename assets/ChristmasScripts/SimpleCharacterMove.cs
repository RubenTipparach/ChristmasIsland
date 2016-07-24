using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using System;

public class SimpleCharacterMove : MonoBehaviour {

	private Vector3 target = Vector3.zero;
	bool inMove = false;

	public float moveSpeed = 10;

	public int GetFormationPos
	{
		get
		{
			return formationPos;
		}
	}

	int formationPos;

	// Use this for initialization
	void Start () {
		target = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		var nAgent = GetComponent<NavMeshAgent>();

		var tpsChar = GetComponent<ThirdPersonCharacter> ();
		Vector3 direction = (target - transform.position);

		nAgent.SetDestination(target);
		var m_Animator = GetComponent<Animator>();
		m_Animator.SetFloat("Forward", nAgent.velocity.magnitude, 0.01f, Time.deltaTime);

		//if (inMove) {
		//	var m_Animator = GetComponent<Animator>();

		//	nAgent.SetDestination(target);
		//	m_Animator.SetFloat("Forward", nAgent.speed, 0.1f, Time.deltaTime);
		//	//m_Animator.SetFloat("Turn", nAgent., 0.1f, Time.deltaTime);
		//	//tpsChar.Move (direction.normalized * Time.deltaTime * moveSpeed, false, false);
		//}

		//if (direction.magnitude < .5f) {
		//	//Debug.Log ("Stop!!");
		//	//tpsChar.Move (Vector3.zero, false, false);
		//	inMove = false;
		//	//var m_Animator = GetComponent<Animator>();

		//	//m_Animator.SetFloat("Forward", nAgent.speed, 0.1f, Time.deltaTime);
		//}
	}

	public void Move(Vector3 _target)
	{
		inMove = true;
		this.target = _target;
	}

	//write formation code later. Organize unit types into flanks.
	public void Move(Vector3 _target, int unitsCount, int ithPosition)
	{
		inMove = true;
		float spacing = 3;

		formationPos = ithPosition;

		if (unitsCount > 0)
		{
			int sqrRootRounded = Convert.ToInt32(Mathf.Ceil(Mathf.Sqrt(unitsCount)));
			int row = ithPosition/sqrRootRounded;
			// Calculate each unit's distance from center, consider that the above is width/height of formation
			Vector3 offSet = new Vector3(
				ithPosition % sqrRootRounded * spacing - (sqrRootRounded * spacing)/2,
				0,
				row * spacing - (sqrRootRounded * spacing) / 2);
			this.target = _target + offSet;
		}
		else
		{
			this.target = _target;
		}
	}
}
