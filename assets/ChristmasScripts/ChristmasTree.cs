using UnityEngine;
using System.Collections;

public class ChristmasTree : MonoBehaviour
{
	[SerializeField]
	int woodQuantity;

	public int WoodQuantity
	{
		get
		{
			return woodQuantity;
		}
	}

	public int TakeWood(int amount)
	{
		if (woodQuantity - amount >= 0)
		{
			woodQuantity -= amount;
			return amount;
		}
		else
		{
			int remaining = amount - woodQuantity;
			woodQuantity = 0;
			return remaining;
		}
	}

	//tree dies once its out of wood! :)
	void Update()
	{
		if (woodQuantity == 0)
		{
			// maybe we'll play some fancy animations later!
			Destroy(this.gameObject);
		}
	}
}