using UnityEngine;

[AddComponentMenu("FOW Example/Move With Mouse")]
public class TSMoveWithMouse : MonoBehaviour
{
#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_WP8
	Transform mTrans;
	Vector3 mMouse;
	Vector3 mTargetPos;
	Vector3 mTargetEuler;

	void Start ()
	{
		mTrans = transform;
		mMouse = Input.mousePosition;
		mTargetPos = mTrans.position;
		mTargetEuler = mTrans.rotation.eulerAngles;
	}

	void Update ()
	{
		Vector3 delta = Input.mousePosition - mMouse;
		mMouse = Input.mousePosition;

		if (Input.GetMouseButton(0))
		{
			mTargetEuler.y += Time.deltaTime * 10f * delta.x;
		}

		if (Input.GetMouseButton(1))
		{
			Vector3 dir = transform.rotation * Vector3.forward;
			dir.y = 0f;
			dir.Normalize();
			Quaternion rot = Quaternion.LookRotation(dir);
			mTargetPos += rot * new Vector3(delta.x * 0.1f, 0f, delta.y * 0.1f);
		}

		float deltaTime = Time.deltaTime * 8f;
		mTrans.position = Vector3.Lerp(mTrans.position, mTargetPos, deltaTime);
		mTrans.rotation = Quaternion.Slerp(mTrans.rotation, Quaternion.Euler(mTargetEuler), deltaTime);
	}
#endif
}
