//----------------------------------------------
//           Tasharen Fog of War
// Copyright © 2012-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Fog of War requires 3 things in order to work:
/// 1. Fog of War system (FOWSystem) that will create a height map of your scene and perform all the updates.
/// 2. Fog of War Revealer on one or more game objects in the world (this class).
/// 3. Either a FOWImageEffect on your camera, or have your game objects use FOW-sampling shaders such as "Fog of War/Diffuse".
/// </summary>

public class FOWRevealer : MonoBehaviour
{
	Transform mTrans;

  public float fieldOfView = 45f;

	/// <summary>
	/// Radius of the area being revealed. Everything below X is always revealed. Everything up to Y may or may not be revealed.
	/// </summary>

	public Vector2 range = new Vector2(2f, 30f);

	/// <summary>
	/// What kind of line of sight checks will be performed.
	/// - "None" means no line of sight checks, and the entire area covered by radius.y will be revealed.
	/// - "OnlyOnce" means the line of sight check will be executed only once, and the result will be cached.
	/// - "EveryUpdate" means the line of sight check will be performed every update. Good for moving objects.
	/// </summary>

	public FOWSystem.LOSChecks lineOfSightCheck = FOWSystem.LOSChecks.None;

	/// <summary>
	/// Whether the revealer is actually active or not. If you wanted additional checks such as "is the unit dead?",
	/// then simply derive from this class and change the "isActive" value accordingly.
	/// </summary>

	public bool isActive = true;

	protected FOWSystem.Revealer mRevealer;

	protected virtual void Start ()
	{
		mTrans = transform;
		mRevealer = FOWSystem.CreateRevealer();
	}

	protected virtual void OnDisable ()
	{
		if (mRevealer != null)
			mRevealer.isActive = false;
	}

	protected virtual void OnDestroy ()
	{
		if (mRevealer != null)
		{
			FOWSystem.DeleteRevealer(mRevealer);
			mRevealer = null;
		}
	}

	protected virtual void LateUpdate ()
	{
		if (isActive)
		{
			if (lineOfSightCheck != FOWSystem.LOSChecks.OnlyOnce)
				mRevealer.isValid = false;

			mRevealer.pos = mTrans.position;
      mRevealer.rot = mTrans.rotation;
			mRevealer.inner = range.x;
			mRevealer.outer = range.y;
			mRevealer.los = lineOfSightCheck;
			mRevealer.isActive = true;

      if (fieldOfView != mRevealer.fovDegrees)
      {
        mRevealer.fovDegrees = fieldOfView;
        mRevealer.fovCosine = Mathf.Cos(Mathf.Deg2Rad * fieldOfView);
      }
		}
		else if (mRevealer.isActive)
		{
			mRevealer.isActive = false;
			mRevealer.isValid = false;
		}
	}

	void OnDrawGizmosSelected ()
	{
		if (lineOfSightCheck != FOWSystem.LOSChecks.None && range.x > 0f)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(transform.position, range.x);
		}
		Gizmos.color = Color.grey;
		Gizmos.DrawWireSphere(transform.position, range.y);
	}

	/// <summary>
	/// Want to force-rebuild the cached buffer? Just call this function.
	/// </summary>

	public void Rebuild () { mRevealer.isValid = false; }
}
