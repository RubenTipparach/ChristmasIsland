using UnityEngine;
using System.Collections;

public class Warehouse : MonoBehaviour {

	public int wood;

  public void DropOffResources(ResourceType resourceType, int quantity)
  {
	var rManager = ResourceManager.GetManager();
	rManager.DumpResource(resourceType, quantity);
	wood = rManager.GetWood;
  } 
}

public enum ResourceType
{
  Wood,
  Food,
  Gold,
  Ore
}