using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour {

	[SerializeField]
	private string Name = "";

	[SerializeField]
	private TransponderType transponderType = TransponderType.Nuetral;

	[SerializeField]
	private float health = 10;

	public float Health
	{
		get
		{
			return health;
		}
	}

	public float TakeHealth(float val)
	{
		health -= val;
		return -val;
	}

	public string ObjName
	{
		get
		{
			return Name;
		}
	}

	public TransponderType Transponder
	{
		get
		{
			return transponderType;
		}
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{

		//Kill if you are dead, or die if you are killed!!!! Ahahahahaha
		if(health <= 0)
		{
			Destroy(this.gameObject);
		}
	}
}

public enum TransponderType
{
	Enemy,
	Nuetral,
	Friendly,
	Own
}
