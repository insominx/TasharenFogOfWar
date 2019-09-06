//----------------------------------------------
//           Tasharen Fog of War
// Copyright © 2012-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Adding a Fog of War Renderer to any game object will hide that object's renderers if they are not visible according to the fog of war.
/// </summary>

public class FOWRenderers : MonoBehaviour
{
	Transform mTrans;
	Renderer[] mRenderers;
	float mNextUpdate = 0f;
	bool mIsVisible = true;
	bool mUpdate = true;

	/// <summary>
	/// Whether the renderers are currently visible or not.
	/// </summary>

	public bool isVisible { get { return mIsVisible; } }

	/// <summary>
	/// Rebuild the list of renderers and immediately update their visibility state.
	/// </summary>

	public void Rebuild () { mUpdate = true; }

	void Awake () { mTrans = transform; }

	void LateUpdate ()
	{
		if (mNextUpdate < Time.time)
		{
			mNextUpdate = Time.time + 0.075f + Random.value * 0.05f;

			if (FOWSystem.instance == null)
			{
				enabled = false;
				return;
			}

			if (mUpdate) mRenderers = GetComponentsInChildren<Renderer>();

			bool visible = FOWSystem.IsVisible(mTrans.position);

			if (mUpdate || mIsVisible != visible)
			{
				mUpdate = false;
				mIsVisible = visible;

				for (int i = 0, imax = mRenderers.Length; i < imax; ++i)
				{
					Renderer ren = mRenderers[i];

					if (ren)
					{
						ren.enabled = mIsVisible;
					}
					else
					{
						mUpdate = true;
						mNextUpdate = Time.time;
					}
				}
			}
		}
	}
}
