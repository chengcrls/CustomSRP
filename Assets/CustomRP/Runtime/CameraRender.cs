using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
public class CameraRender
{
    ScriptableRenderContext context;

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

        Setup();
        DrawVisibleGeometry();
        Submit();
    }

    void DrawVisibleGeometry()
    {
        context.DrawSkybox(camera);
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
}