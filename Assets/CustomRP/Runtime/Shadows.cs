using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Shadows
{
    const string bufferName="Shadows";

    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    static int dirShadowAtlasId = Shader.PropertyToID("_DirShadowAtlas");

    private const int maxShadowDirectionalLightCount = 1;

    private int shadowDirectionalLightCount;
    
    ScriptableRenderContext context;
    CullingResults cullingResults;
    ShadowSetting setting;

    struct ShadowDirectionalLight
    {
        public int visibleLightIndex;
    }
    ShadowDirectionalLight[] shadowDirectionalLights = new ShadowDirectionalLight[maxShadowDirectionalLightCount];
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSetting setting)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.setting = setting;
        shadowDirectionalLightCount = 0;
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        if (shadowDirectionalLightCount < maxShadowDirectionalLightCount && 
            light.shadows!=LightShadows.None && light.shadowStrength > 0f &&
            cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds bounds))
        {
            shadowDirectionalLights[shadowDirectionalLightCount++] = new ShadowDirectionalLight
            {
                visibleLightIndex = visibleLightIndex,
            };
        }
    }

    public void Render()
    {
        if (maxShadowDirectionalLightCount > 0)
        {
            RenderDirectionalShadows();
        }
        else
        {
            buffer.GetTemporaryRT(dirShadowAtlasId, 1, 1, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
        }
    }

    void RenderDirectionalShadows()
    {
        int atlasSize=(int)setting.directinoal.atlasSize;
        buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize,32,FilterMode.Bilinear,RenderTextureFormat.Shadowmap);
        buffer.SetRenderTarget(dirShadowAtlasId,RenderBufferLoadAction.DontCare,RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);
        ExecuteBuffer();
    }

    public void Cleanup()
    {
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }
}
