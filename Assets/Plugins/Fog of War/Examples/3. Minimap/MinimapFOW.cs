using UnityEngine;

public class MinimapFOW : MinimapPlain
{
	[System.NonSerialized] protected float mFogScale = 1f;
	[System.NonSerialized] protected float mFogOffset = 0f;

	protected override void OnLateUpdate ()
	{
		base.OnLateUpdate();

		if (FOWSystem.instance != null)
		{
			float camRange = mapRenderer.GetComponent<Camera>().orthographicSize * 2f;
			mFogScale = camRange / FOWSystem.instance.worldSize;
			mFogOffset = (1f - mFogScale) * 0.5f;

			material.SetFloat("_Blend", FOWSystem.instance.blendFactor);
			material.SetTexture("_FogTex0", FOWSystem.instance.texture0);
			material.SetTexture("_FogTex1", FOWSystem.instance.texture1);
			material.SetTextureScale("_FogTex0", new Vector2(mFogScale, mFogScale));
			material.SetTextureOffset("_FogTex0", new Vector2(mFogOffset, mFogOffset));
		}
	}
}
