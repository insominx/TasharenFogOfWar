using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("FOW Example/Click To Move")]
[RequireComponent(typeof(NavMeshAgent))]
public class TSClickToMoveAgent : MonoBehaviour
{
	public LayerMask layerMask = 1;
	public KeyCode keyCode = KeyCode.Mouse0;

	Vector2 mMousePress;
  NavMeshAgent agent;

	void Start ()
	{
    agent = GetComponent<NavMeshAgent>();
	}

	void Update ()
	{
		if (Input.GetKeyDown(keyCode))
    {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, 300f, layerMask))
			{
        agent.SetDestination(hit.point);
			}
    }
	}
}