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

    //CommandBufferû�д�string�����Ĺ��캯������������д������ֻ��һ���������name�ĸ�ֵ�������Ǹ�CommandBuffer�Ŀղι��캯������name=bufferName���һ��
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