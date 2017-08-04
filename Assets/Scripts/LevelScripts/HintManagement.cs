using UnityEngine;

public class HintManagement : MonoBehaviour
{
	public string message = "";
	
	private GameObject player;
	private bool used = false;

	private ControlsTutorial manager;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		manager = this.transform.parent.GetComponent<ControlsTutorial> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if((other.gameObject == player) && !used)
		{
			manager.setShowMsg(true);
			manager.setMessage(message);
			used = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject == player)
		{
			manager.setShowMsg(false);
			Destroy(gameObject);
		}
	}
}
