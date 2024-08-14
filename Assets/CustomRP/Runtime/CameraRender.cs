using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
public partial class CameraRender
{
    ScriptableRenderContext context;
    CullingResults cullingResults;
    //�����Always�ǲ�һ���ģ�������ʲô����أ�
    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    static Material errorMaterial;
    Camera camera;

    const string bufferName = "Render Camera";

    //CommandBufferû�д�string�����Ĺ��캯������������д������ֻ��һ���������name�ĸ�ֵ�������Ǹ�CommandBuffer�Ŀղι��캯������name=bufferName���һ��
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;

        if (!Cull())
        {
            return;
        }

        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        Submit();
    }

    void DrawVisibleGeometry()
    {
        var sorttingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSetting = new DrawingSettings(unlitShaderTagId,sorttingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        context.DrawRenderers(cullingResults, ref drawingSetting, ref filteringSettings);
        context.DrawSkybox(camera);
        sorttingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSetting.sortingSettings = sorttingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults,ref drawingSetting,ref filteringSettings);
    }
    void Submit()
    {
        buffer.EndSample(bufferName);
        ExecuteBuffer();
        context.Submit();
    }

    void Setup()
    {
        context.SetupCameraProperties(camera);
        buffer.ClearRenderTarget(true, true, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    bool Cull()
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResults=context.Cull(ref p);
            return true;
        }
        return false;
    }
}