using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class CustomRenderPipeline : RenderPipeline
{
    CameraRender render=new CameraRender();
    bool useDynamicBatching,useGPUInstancing;
    ShadowSetting shadowSetting;
    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing,bool useSRPBatcher, ShadowSetting shadowSetting)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        this.shadowSetting = shadowSetting;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
    }
    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            render.Render(context, cameras[i],useDynamicBatching,useGPUInstancing,shadowSetting);
        }
    }
}
