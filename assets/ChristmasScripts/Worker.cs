using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour
{
	ChoppingWoodState woodChopping = null;
	GameObject ConstructionSite = null;
	
	WorkerTask currentTask = WorkerTask.Idle;
	Vector3 moveToTarget = Vector3.zero;
	
	// This guy moves to a specified location when Idle
	// He'll also move when he has stuff to do!
	bool move = false;
	
	// Use this for initialization
	void Start()
	{
		moveToTarget = this.transform.position;
	}
	
	public void SetTask(GameObject selectedTaskObject)
	{
		var tree = selectedTaskObject.GetComponent<ChristmasTree>();
		var wareHouse = selectedTaskObject.GetComponent<Warehouse>();

		if(tree != null)
		{

			//Debug.Log("ChristmasTree!");

			currentTask = WorkerTask.Lumberjack;

			// Official statement: Please note that this function is very slow. It is not recommended to use this function every frame.
			// In most cases you can use the singleton pattern instead. So what its saying is that I should be manually registering objects. F that!
			//Warehouse[] ob = FindObjectsOfType(typeof(Warehouse)) as Warehouse[];
			//Warehouse nearestWareHouse = null;
			//foreach(var o in ob)
			//{
			//	if(nearestWareHouse == null)
			//	{
			//		nearestWareHouse = o;
			//	}
			//	else if(Vector3.Distance(o.transform.position, this.transform.position) < 
			//		Vector3.Distance(nearestWareHouse.transform.position, this.transform.position))
			//	{
			//		nearestWareHouse = o;
			//	}
			//}

			Warehouse nearestWareHouse = RTSUnitQueryCollection.FindNearestObjectOfType<Warehouse>(this.transform.gameObject);

			// find nearest warehouse

			woodChopping = null; // hopefully this will dispose of the object? It doesn't have any unmanaged res.
			woodChopping = new ChoppingWoodState(tree, nearestWareHouse, this.gameObject);	
		}

		if(wareHouse != null)
		{
			currentTask = WorkerTask.Lumberjack;
			ChristmasTree nearestTree = RTSUnitQueryCollection.FindNearestObjectOfType<ChristmasTree>(this.transform.gameObject);

			woodChopping = null;
			woodChopping = new ChoppingWoodState(nearestTree, wareHouse, this.gameObject);
		}
	}

	//set base condition to null, so that we may move into idle state.
	public void SetTask(Vector3 moveTo, int unitsCount, int i, WorkerTask taskContext = WorkerTask.Idle)
	{
		currentTask = taskContext;
		MoveTo(moveTo, unitsCount, i);
	}

	// Update is called once per frame
	void Update ()
	{
		
		// This guy is doing nothing!
		if(currentTask == WorkerTask.Idle)
		{
			var navAgent = GetComponent<NavMeshAgent>();

			navAgent.updateRotation = true;
			return;
		}

		// Chopping wood!
		if (currentTask == WorkerTask.Lumberjack && woodChopping != null)
		{
			//Debug.Log("Chopping in session." + woodChopping);
			woodChopping.ReachedResource();
		}
		
		// Building shit!
		if(currentTask == WorkerTask.Builder)
		{
			if(ConstructionSite != null)
			{
				
			}
		}
	}
	
	void MoveTo(Vector3 targetMove, int unitsCount, int i)
	{
		move = true;
		moveToTarget = targetMove;
		var sm = GetComponent<SimpleCharacterMove>();
		
		if(sm != null)
		{
			sm.Move(targetMove,unitsCount, i);

		}
	}
}


/// <summary>
/// This class represents the state of the worker. In this stateobject, he is chopping wood
/// and delivering it to a specified station.
/// </summary>
public class ChoppingWoodState
{
	ResourceManager resourceManger = null;
	ChristmasTree tree = null;
	Warehouse warehouse = null;
	
	int quantityHeld = 0;
	bool gathering = false;
	
	float gatherSpeed = 2;
	float gathered = 0;
	
	Vector3 goToTarget = Vector3.zero;

	GameObject currentGameObject;
	
	public Vector3 GoToTarget
	{
		get
		{
			return goToTarget;
		}
	}

	public ChoppingWoodState(ChristmasTree _resource, Warehouse _warehouse, GameObject transform)
	{
		tree = _resource;
		warehouse = _warehouse;
		resourceManger = ResourceManager.GetManager();
		currentGameObject = transform;
		gathering = true;
	}
	
	public void ReachedResource()
	{
		var animator = currentGameObject.GetComponent<Animator>();
		var navAgent = currentGameObject.GetComponent<NavMeshAgent>();
		//Debug.Log("Chopping in session.");

		if (tree.WoodQuantity <= 0)
		{
			gathering = false;
			navAgent.updateRotation = true;
		}

		//else if(Vector3.Distance(currentGameObject.transform.position, 
		//	tree.transform.position) < 2f)
		//{
		//	gathering = true;
		//}

		if(!gathering)
		{
			//this is actually attack. which is actually mine  or chop wood :)
			animator.SetBool("Crouch", false);
			goToTarget = warehouse.transform.position;

			if(Vector3.Distance(goToTarget, currentGameObject.transform.position) < 10f)
			{
				//do some complex collision shit to get this. Will do tomorrow. Hopefully I can show rebecca wood chopping sim.
				//Debug.Log("Dumping stuff!!");
				if (quantityHeld > 0)
				{
					warehouse.DropOffResources(ResourceType.Wood, quantityHeld);
					quantityHeld = 0;

					//then we find a new tree :)
					var nearestTree = RTSUnitQueryCollection.FindNearestObjectOfType<ChristmasTree>(currentGameObject);

					if(nearestTree != null)
					{
						gathering = true;
						tree = nearestTree;
					}
				}
			}
		}
		else
		{
			if(tree == null)
			{
				return;
			}

			goToTarget = tree.transform.position;
		
			if (gathered < gatherSpeed)	
			{
				gathered += Time.deltaTime;
				navAgent.updateRotation = false;
				animator.SetBool("Crouch", false);
				Quaternion rotation = Quaternion.LookRotation(goToTarget - currentGameObject.transform.position);
				currentGameObject.transform.rotation = Quaternion.Slerp(currentGameObject.transform.rotation, rotation, Time.deltaTime * 2);
			}
			else if(Vector3.Distance(goToTarget, currentGameObject.transform.position) < 3f)
			{
				gathered = 0;
				// this tries to take a certain amount, if successful then we're done.
				quantityHeld += tree.TakeWood(5);
				animator.SetBool("Crouch", true);
				//once the tree is ded, find a new tree!
			}
		}

		//switch states when we get to the spot.
		var worker = currentGameObject.GetComponent<Worker>();
		worker.SetTask(goToTarget, 1, 0, WorkerTask.Lumberjack);
	}
}
	
public enum WorkerTask
{
	Idle,
	Lumberjack,
	GoldMiner,
	OreMiner,
	Farmer,
	Builder
}