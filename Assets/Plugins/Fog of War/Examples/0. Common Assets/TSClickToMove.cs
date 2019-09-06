using UnityEngine;

[AddComponentMenu("FOW Example/Click To Move")]
public class TSClickToMove : MonoBehaviour
{
	public LayerMask layerMask = 1;
	public KeyCode keyCode = KeyCode.Mouse0;

	Transform mTrans;
	Vector3 mTarget;
	Vector2 mMousePress;

	void Start ()
	{
		mTrans = transform;
		mTarget = mTrans.position;
	}

	void Update ()
	{
		if (Input.GetKeyDown(keyCode)) mMousePress = Input.mousePosition;

		if (Input.GetKeyUp(keyCode))
		{
			if (Vector2.Distance(Input.mousePosition, mMousePress) > 5f) return;

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, 300f, layerMask))
			{
				mTarget = hit.point;
			}
		}

		float dist = Vector3.Distance(mTrans.position, mTarget);

		if (dist > 0.001f)
		{
			Vector3 newPos = Vector3.MoveTowards(mTrans.position, mTarget, 10f * Time.deltaTime);
			newPos.y = Terrain.activeTerrain.SampleHeight(newPos);
			mTrans.position = newPos;
		}
	}
}