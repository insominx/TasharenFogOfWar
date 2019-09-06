using UnityEngine;

/// <summary>
/// Very basic game map script that takes a camera and renderes it into a texture using the specified replacement shader.
/// </summary>

public class MinimapPlain : MonoBehaviour
{
	/// <summary>
	/// Renderer used to draw the map.
	/// </summary>

	public Camera mapRenderer;

	/// <summary>
	/// Shader used to draw the map with.
	/// </summary>

	public Shader mapRendererShader;

	/// <summary>
	/// Material to update.
	/// </summary>

	public Material material;

	/// <summary>
	/// Width of the minimap's texture.
	/// </summary>

	public int width = 128;

	/// <summary>
	/// Height of the minimap's texture.
	/// </summary>

	public int height = 128;

	/// <summary>
	/// How often the map will be checked for updates.
	/// </summary>

	public float updateFrequency = 0.5f;

	[System.NonSerialized] protected Transform mTrans;
	[System.NonSerialized] protected RenderTexture mRT;
	[System.NonSerialized] protected float mNextUpdate = 0f;
	[System.NonSerialized] protected Vector3 mLastPos = Vector3.zero;
	[System.NonSerialized] protected float mLastSize = 0f;
	[System.NonSerialized] protected int mWidth = 0;
	[System.NonSerialized] protected int mHeight = 0;
	[System.NonSerialized] protected bool mRefresh = true;
	[System.NonSerialized] protected bool mSizeChanged = false;

	/// <summary>
	/// Return 'false' if you don't want to render the map (such as while the world is loading).
	/// </summary>

	protected virtual bool canRender { get { return (Terrain.activeTerrain != null); } }

	/// <summary>
	/// Cache the transform and register callbacks.
	/// </summary>

	protected virtual void Awake ()
	{
		mRefresh = true;
		mTrans = transform;

		if (mapRenderer == null)
		{
			Debug.LogError("Expected to find a map renderer to work with", this);
			enabled = false;
			return;
		}
	}

	protected void Start () { OnStart(); Update(); }

	/// <summary>
	/// Mark the map as changed.
	/// </summary>

	protected void Invalidate () { mRefresh = true; mSizeChanged = true; }
	protected void OnApplicationFocus (bool focus) { mRefresh = true; mSizeChanged = true; }

	/// <summary>
	/// Update what's necessary.
	/// </summary>

	protected virtual void Update ()
	{
		if (mLastPos != mapRenderer.transform.position || mLastSize != mapRenderer.orthographicSize)
		{
			mLastPos = mapRenderer.transform.position;
			mLastSize = mapRenderer.orthographicSize;
			mRefresh = true;
		}

		if (mWidth != width || mHeight != height)
		{
			mSizeChanged = true;
			mWidth = width;
			mHeight = height;
			mRefresh = true;
		}

		if (canRender && (mRefresh || mNextUpdate < Time.time))
		{
			mNextUpdate = Time.time + updateFrequency;

			if (mRefresh)
			{
				Terrain ter = Terrain.activeTerrain;

				if (ter != null)
				{
					Shader.SetGlobalFloat("terrainOffset", ter.transform.position.y);
					Shader.SetGlobalFloat("terrainScale", ter.terrainData.heightmapScale.y);
				}

				if (mSizeChanged && mRT != null)
				{
					Destroy(mRT);
					mRT = null;
				}

				if (mRT == null)
				{
					mRT = new RenderTexture(mWidth, mHeight, 24, RenderTextureFormat.ARGB32);
					mRT.name = name;
					mRT.autoGenerateMips = false;
				}

				// Render the map into the render texture
				mapRenderer.targetTexture = mRT;
				mapRenderer.RenderWithShader(mapRendererShader, "");
				mapRenderer.targetTexture = null;
			}

			OnUpdate(mRefresh);
			mRefresh = false;
			mSizeChanged = false;
		}
	}

	/// <summary>
	/// Invalidate the render texture's dimensions when returning to the app.
	/// </summary>

	protected void OnApplicationPause (bool isPaused)
	{
		if (!isPaused)
		{
			mRefresh = true;
			mSizeChanged = true;
			Update();
		}
	}

	/// <summary>
	/// Clear the refresh flag at the end.
	/// </summary>

	protected void LateUpdate () { OnLateUpdate(); }

	/// <summary>
	/// Anything you need to do in Start.
	/// </summary>

	protected virtual void OnStart () { }

	/// <summary>
	/// Anything else you might want to update (target indicators and such).
	/// The 'rebuild' parameter will be 'true' if the map texture was rebuilt.
	/// </summary>

	protected virtual void OnUpdate (bool rebuild) { }

	/// <summary>
	/// Anything you need to do in Late Update.
	/// </summary>

	protected virtual void OnLateUpdate () { if (material != null) material.mainTexture = mRT; }

	/// <summary>
	/// Anything you need to do in OnDestroy.
	/// </summary>

	protected virtual void OnDestroy ()
	{
		if (mRT != null)
		{
			Destroy(mRT);
			mRT = null;
		}
	}
}
