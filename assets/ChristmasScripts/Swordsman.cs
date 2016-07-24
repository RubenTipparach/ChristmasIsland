using UnityEngine;
using System.Collections;

public class Swordsman : MonoBehaviour {

	SwordsmanAttackingState swordsAttacking = null;

	MillitarTask currentTask = MillitarTask.Idle;
	Vector3 moveToTarget = Vector3.zero;

	bool move = false;

	[SerializeField]
	float enemySeekingRange = 100;

	public SwordsmanAttackingState CurrentAttckState
	{
		get
		{
			return swordsAttacking;
		}
	}

	// Use this for initialization
	void Start ()
	{
		moveToTarget = this.transform.position;
	}
	
	public void SetTask(GameObject selectedTaskObject)
	{
		// Space objects are attackable objects such as People and Buildings
		var enemyTarget = selectedTaskObject.GetComponent<SpaceObject>();

		if(enemyTarget != null)
		{
			currentTask = MillitarTask.Attack;
		
			swordsAttacking = null;
			swordsAttacking = new SwordsmanAttackingState(enemyTarget, this.gameObject, enemySeekingRange);
		}
	}

	public void SetTask(Vector3 moveTo, int unitsCount, int i, MillitarTask task = MillitarTask.Idle )
	{
		currentTask = task;
		MoveTo(moveTo, unitsCount, i);
	}

	// Update is called once per frame
	void Update ()
	{
		if(currentTask == MillitarTask.Idle)
		{
			var agent = GetComponent<NavMeshAgent>();
			agent.updateRotation = true;

			return;
		}

		if(currentTask == MillitarTask.Attack && swordsAttacking != null)
		{
			swordsAttacking.AttackingMelee();
		}

		// TODO: Gaurd stance
		// TODO: Passive stance
	}

	void MoveTo(Vector3 targetMove, int unitsCount, int i)
	{
		move = true;
		moveToTarget = targetMove;
		var sm = GetComponent<SimpleCharacterMove>();

		if (sm != null)
		{
			sm.Move(targetMove, unitsCount, i);

		}
	}
}

public class SwordsmanAttackingState
{
	//Only if Transpoder == enemy, later we'll use ints for teams.
	SpaceObject enemyObject = null;

	bool isAttacking = false;

	float attackSpeed = 2;
	float attackRecharge = 0; // this recharges to attack speed amount

	Vector3 goToTarget = Vector3.zero;

	GameObject currentGameObject = null;

	float nextEnemyRange;

	Vector3 offSet = Vector3.zero;

	/// <summary>
	/// This result will be used to help the ai query how it will attack the enemy. It should check if another character is approaching the enemy,
	/// of any type. Then arrange itself so that it is not colliding with a friendly unit.
	/// </summary>
	public SpaceObject EnemyBeingAttacked
	{
		get
		{
			return enemyObject;
		}
	}

	public Vector3 GoToTarget
	{
		get
		{
			return goToTarget;
		}
	}

	// All space objects have health
	public SwordsmanAttackingState(SpaceObject _enemyObject, GameObject _currentGameObject, float _nextEnemyRange)
	{
		enemyObject = _enemyObject;
		currentGameObject = _currentGameObject;
		nextEnemyRange = _nextEnemyRange;

		// Begin attack approach
		isAttacking = true;

		//since queries cost so much, save some time and do one here.
		//offSet = DelegateAttackPosition(currentGameObject, enemyObject);
	}

	public void AttackingMelee()
	{
		if (!isAttacking)
		{
			return;
		}

		var animator = currentGameObject.GetComponent<Animator>();
		var navAgent = currentGameObject.GetComponent<NavMeshAgent>();
		var swordsman = currentGameObject.GetComponent<Swordsman>();

		if(enemyObject != null)
		{
			//Debug.Log("hit1");
			goToTarget = enemyObject.transform.position - offSet;
				
			if (attackRecharge < attackSpeed)
			{
				attackRecharge += Time.deltaTime;
				animator.SetBool("Crouch", false);

				// Rotate to face enemy.
				Vector3 projectedToDefaultPlane = Vector3.ProjectOnPlane((enemyObject.transform.position - currentGameObject.transform.position), Vector3.up).normalized;
				Quaternion rotation = Quaternion.LookRotation(projectedToDefaultPlane);

				navAgent.updateRotation = false;
				currentGameObject.transform.rotation = Quaternion.Slerp(currentGameObject.transform.rotation, rotation, Time.deltaTime * 2);

				//if (Vector3.Distance(goToTarget, currentGameObject.transform.position) < 3f)
				//{
				//	navAgent.updatePosition = false;
				//	currentGameObject.transform.position = Vector3.Lerp(currentGameObject.transform.position, goToTarget, Time.deltaTime);
				//}
				//else if(!navAgent.updatePosition)
				//{
				//	//Debug.Log("Lerp update position." + goToTarget);
				//	goToTarget = navAgent.nextPosition;
				//	currentGameObject.transform.position = Vector3.Lerp(currentGameObject.transform.position, goToTarget, Time.deltaTime * 5);
					
				//	// update the position if  we are close enough to the target.
				//	if (Vector3.Distance(goToTarget, currentGameObject.transform.position) < .5f)
				//	{
				//		//Debug.Log("Nav update position.");
				//		navAgent.updatePosition = true;
				//	}
				//}
			}
			else if (Vector3.Distance(goToTarget, currentGameObject.transform.position) < 3f)
			{
				attackRecharge = 0;
				navAgent.stoppingDistance = 2f;
				animator.SetBool("Crouch", true);
				navAgent.updateRotation = true;
				enemyObject.TakeHealth(2);

				// needs to return to normal spot before updating the position.
				if(enemyObject.Health <= 0)
				{
					//Debug.Log("Lerp back to nav mesh Position: " + navAgent.nextPosition);
					goToTarget = navAgent.nextPosition;
				}
			}
		}
		else
		{
			//if (!navAgent.updatePosition)
			//{
			//	//Debug.Log("Lerp update position." + goToTarget);
			//	goToTarget = navAgent.nextPosition;
			//	currentGameObject.transform.position = Vector3.Lerp(currentGameObject.transform.position, goToTarget, Time.deltaTime * 5);

			//	// update the position if  we are close enough to the target.
			//	if (Vector3.Distance(goToTarget, currentGameObject.transform.position) < .5f)
			//	{
			//		//Debug.Log("Nav update position.");
			//		navAgent.updatePosition = true;
			//	}
			//}
		}

		if(enemyObject == null)
		{
			// Set isAttacking to false if enemy is out of range.
			// That is the end of this script in that case.
			var nearestEnemy = RTSUnitQueryCollection.FindNearestIdentifiedType<SpaceObject>(currentGameObject, TransponderType.Enemy, 100);
			
			//make sure the agent will update position before moving to a new thing. This also includes decide whether to attack or not.
			//if (navAgent.updatePosition)
			//{
				if (nearestEnemy != null)
				{
					enemyObject = nearestEnemy;
					//offSet = DelegateAttackPosition(currentGameObject, enemyObject);
				}
				else
				{
					isAttacking = false;
				}
			//}
		}

		//approach the designated target.
		if (isAttacking)
		{
			swordsman.SetTask(goToTarget, 1, 0, MillitarTask.Attack);
		}
		else
		{
			// Set this to its default state.
			navAgent.updateRotation = true;
			navAgent.updatePosition = true;
			animator.SetBool("Crouch", false);

			swordsman.SetTask(goToTarget, 1, 0);
			goToTarget = currentGameObject.transform.position;
		}
	}

	/// <summary>
	///  Angles are at unit 1, multiply accordingly.
	/// </summary>
	/// <param name="self"></param>
	/// <param name="enemy"></param>
	/// <returns>A round Robin if there is 1 or more attackers already attacking.</returns>
	public static Vector3 DelegateAttackPosition(GameObject self, SpaceObject enemy)
	{
		Swordsman[] nearbyAttackers = RTSUnitQueryCollection.FindAndOrganizeAttackVectorForSwordsman(enemy, TransponderType.Friendly);

		//defalt case
		Vector3 attackAngle = Vector3.ProjectOnPlane((enemy.transform.position - self.transform.position), Vector3.up).normalized;

		if(nearbyAttackers.Length == 0)
		{
			//attack at the closest angle.
			return attackAngle;
		}
		else if(nearbyAttackers.Length >= 0)
		{
			// move clock wise by 1/8 * 360
			Quaternion rotationOfAttack = Quaternion.Euler(0, nearbyAttackers.Length / 8 * 360, 0);
			return rotationOfAttack * attackAngle;
		}

		// shouldn't get here.
		return enemy.transform.position;
	}
}

public enum MillitarTask
{
	Idle,
	Guard,
	Attack
}
