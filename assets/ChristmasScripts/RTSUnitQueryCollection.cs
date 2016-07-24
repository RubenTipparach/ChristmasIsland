using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RTSUnitQueryCollection
{
	/// <summary>
	/// This one is a more generalized version than the SpaceObject finder. This algorithm finds any object in the world
	/// of Monobehavior type that is closest.
	/// </summary>
	/// <typeparam name="T">The object we are searching for.</typeparam>
	/// <param name="gameObject">The game object that we are using to search in relative position.</param>
	/// <returns>The nearest object to our relative body.</returns>
	public static T FindNearestObjectOfType<T>(GameObject gameObject) where T : MonoBehaviour
	{
		T[] ob = Object.FindObjectsOfType(typeof(T)) as T[];
		T nearestObj = null;
		foreach (var o in ob)
		{
			if (nearestObj == null)
			{
				nearestObj = o;
			}
			else if (Vector3.Distance(o.transform.position, gameObject.transform.position) <
				Vector3.Distance(nearestObj.transform.position, gameObject.transform.position))
			{
				nearestObj = o;
			}
		}

		return nearestObj;
	}

	/// <summary>
	/// Find the closes object of type T. This is usedul for finding an enemy or friendly to interact with.
	/// </summary>
	/// <typeparam name="T">Any type of object that is of SpaceObject and has the specified alignment type.</typeparam>
	/// <param name="gameObject">Our object that we want to query for in relative position.</param>
	/// <param name="alignment">Who this character belongs to?</param>
	/// <param name="range">The range or distance of the search.</param>
	/// <returns>The object that is nearest to us according to the specified parameter.</returns>
	public static T FindNearestIdentifiedType<T>(GameObject gameObject, TransponderType alignment, float range) where T : SpaceObject
	{
		T[] ob = Object.FindObjectsOfType(typeof(T)) as T[];
		T nearestObj = null;
		foreach (var o in ob)
		{
			if (nearestObj == null)
			{
				if (Vector3.Distance(o.transform.position, gameObject.transform.position) < range && alignment == o.Transponder)
				{
					nearestObj = o;
				}
			}
			else if (Vector3.Distance(o.transform.position, gameObject.transform.position) <
				Vector3.Distance(nearestObj.transform.position, gameObject.transform.position)
				&& Vector3.Distance(o.transform.position, gameObject.transform.position) < range
				&& alignment == o.Transponder)
			{
				nearestObj = o;
			}
		}

		return nearestObj;
	}

	/// <summary>
	/// Here we ask the question, who is attacking this guy? If there are multiple people, then we must consider our position relative to them.
	/// </summary>
	/// <param name="consideredTarget">The target our current swordsman is consdiering attacking</param>
	/// <param name="alignment">His alignment, although not currently used, we may find a use for it later.</param>
	/// <returns>The array of characters commencing the attack.</returns>
	public static Swordsman[] FindAndOrganizeAttackVectorForSwordsman(SpaceObject consideredTarget, TransponderType alignment)
	{
		Swordsman[] swordsmans = Object.FindObjectsOfType<Swordsman>();
		List<Swordsman> attackingSameGuy = new List<Swordsman>(8);

		foreach (var attacker in attackingSameGuy)
		{
			if(attacker.CurrentAttckState != null)
			{
				if(attacker.CurrentAttckState.EnemyBeingAttacked != null)
				{
					if(attacker.CurrentAttckState.EnemyBeingAttacked == consideredTarget)
					{
						attackingSameGuy.Add(attacker);
					}
				}
			}
		}

		return attackingSameGuy.ToArray();
	}
}
