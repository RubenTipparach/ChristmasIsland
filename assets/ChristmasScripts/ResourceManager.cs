using UnityEngine;
using System.Collections;

public class ResourceManager
{
	private int WoodQuantity;

	private static ResourceManager instance;

	public static ResourceManager GetManager()
	{
		if (instance == null)
		{
			instance = new ResourceManager();
		}

		return instance;
	}

	private ResourceManager()
	{
		//do shit.
	}

	public void DumpResource(ResourceType resourceType, int Quantity)
	{
		if (resourceType == ResourceType.Wood)
		{
			WoodQuantity += Quantity;
		}
	}

	public int GetWood
	{
		get
		{
			return WoodQuantity;
		}
	}
}

public class Player
{
	private PlayerType playerType;

	private Resources resources;

	private int unitSupply; // Unit capacity.

	public PlayerType PlayerType
	{
		get
		{
			return playerType;
		}
	}

}

public class GameResources
{
	private int wood;
	private int gold;
	private int food;

	public GameResources()
	{

	}
}

public enum PlayerType
{
	Human,
	AI,
	NPC
}