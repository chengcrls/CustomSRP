using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class CustomRenderPipeline : RenderPipeline
{
    CameraRender render=new CameraRender();
    public CustomRenderPipeline()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
    }
    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            render.Render(context, cameras[i]);
        }
    }
}
