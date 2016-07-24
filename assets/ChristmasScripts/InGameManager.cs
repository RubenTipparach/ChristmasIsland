using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour {
	
	[SerializeField]
	Text woodAmount = null;

	[SerializeField]
	Text goldAmount = null;

	float refreshRate = 1;

	float refreshing = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI()
	{
		var resourceManager = ResourceManager.GetManager();

		if (refreshing >= refreshRate)
		{
			woodAmount.text = resourceManager.GetWood.ToString();
			goldAmount.text = 0 + "";

			refreshing = 0;
		}
		else
		{
			refreshing += Time.deltaTime;
		}
	}
}
