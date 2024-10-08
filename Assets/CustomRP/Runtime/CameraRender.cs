using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
public partial class CameraRender
{
    ScriptableRenderContext context;
    CullingResults cullingResults;
    //这个和Always是不一样的，它们有什么差别呢？
    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    static Material errorMaterial;
    Camera camera;

    const string bufferName = "Render Camera";

    //CommandBuffer没有带string参数的构造函数，因此下面的写法可以只有一个语句就完成name的赋值，就像是给CommandBuffer的空参构造函数加了name=bufferName语句一样
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;
        PrepareBuffer();
        //为什么要写在Cull之前。放在后面就会看不到，被剔除掉了吗？这里的剔除逻辑是什么？
        PrepareForSceneWindow();
        if (!Cull())
        {
            return;
        }

        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos();
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
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
    }

    void Setup()
    {
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        buffer.ClearRenderTarget(flags<=CameraClearFlags.Depth, flags==CameraClearFlags.Color, flags==CameraClearFlags.Color?camera.backgroundColor.linear:Color.clear);
        buffer.BeginSample(SampleName);
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