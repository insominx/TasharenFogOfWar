//----------------------------------------------
//           Tasharen Fog of War
// Copyright © 2012-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Fog of War requires 3 things in order to work:
/// 1. Fog of War system (FOWSystem) that will create a height map of your scene and perform all the updates.
/// 2. Fog of War Revealer on one or more game objects in the world.
/// 3. Either a FOWImageEffect on your camera, or have your game objects use FOW-sampling shaders such as "Fog of War/Diffuse".
/// </summary>

[RequireComponent(typeof(Camera))]
public class FOWImageEffect : MonoBehaviour
{
	static public FOWImageEffect instance;

	/// <summary>
	/// Shader used to create the effect. Should reference "Image Effects/Fog of War".
	/// </summary>

	public Shader shader;

	Camera mCam;
	FOWSystem mFog;
	Matrix4x4 mInverseMVP;
	Material mMat;

	void Awake () { instance = this; }
	void OnDestroy () { if (instance == this) instance = null; }

	/// <summary>
	/// The camera we're working with needs depth.
	/// </summary>

	void OnEnable ()
	{
		mCam = GetComponent<Camera>();
		mCam.depthTextureMode = DepthTextureMode.Depth;
		if (shader == null) shader = Shader.Find("Image Effects/Fog of War");
	}

	/// <summary>
	/// Destroy the material when disabled.
	/// </summary>

	void OnDisable () { if (mMat) DestroyImmediate(mMat); }

	/// <summary>
	/// Automatically disable the effect if the shaders don't support it.
	/// </summary>

	void Start ()
	{
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth) || !shader || !shader.isSupported)
			enabled = false;
	}

	/// <summary>
	/// Called by camera to apply image effect.
	/// </summary>

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (mFog == null)
		{
			mFog = FOWSystem.instance;
			if (mFog == null) mFog = FindObjectOfType(typeof(FOWSystem)) as FOWSystem;
		}

		if (mFog == null)
		{
			enabled = false;
			return;
		}

		// Calculate the inverse modelview-projection matrix to convert screen coordinates to world coordinates
		mInverseMVP = (mCam.projectionMatrix * mCam.worldToCameraMatrix).inverse;

		float invScale = 1f / mFog.worldSize;
		Transform t = mFog.transform;
		float x = t.position.x - mFog.worldSize * 0.5f;
		float y = t.position.z - mFog.worldSize * 0.5f;

		if (mMat == null)
		{
			mMat = new Material(shader);
			mMat.hideFlags = HideFlags.HideAndDontSave;
		}

		Vector4 camPos = mCam.transform.position;

		// This accounts for Anti-aliasing on Windows flipping the depth UV coordinates.
		// Despite the official documentation, the following approach simply doesn't work:
		// http://docs.unity3d.com/Documentation/Components/SL-PlatformDifferences.html

		if (QualitySettings.antiAliasing > 0)
		{
			RuntimePlatform pl = Application.platform;

			if (pl == RuntimePlatform.WindowsEditor ||
				pl == RuntimePlatform.WindowsPlayer ||
				pl == RuntimePlatform.WebGLPlayer)
			{
				camPos.w = 1f;
			}
		}

		Vector4 p = new Vector4(-x * invScale, -y * invScale, invScale, mFog.blendFactor);
		mMat.SetColor("_Unexplored", mFog.unexploredColor);
		mMat.SetColor("_Explored", mFog.exploredColor);
		mMat.SetTexture("_FogTex0", mFog.texture0);
		mMat.SetTexture("_FogTex1", mFog.texture1);
		mMat.SetMatrix("_InverseMVP", mInverseMVP);
		mMat.SetVector("_CamPos", camPos);
		mMat.SetVector("_Params", p);

		Graphics.Blit(source, destination, mMat);
	}
}
