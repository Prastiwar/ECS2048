using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class Utils
{
    public static MeshInstanceRenderer GetLookFromPrototype(string prototypeName)
    {
        var prototype = GameObject.Find(prototypeName);
        if (prototype != null)
        {
            var result = prototype.GetComponent<MeshInstanceRendererComponent>().Value;
            Object.Destroy(prototype);
            return result;
        }
        Debug.LogWarning($"{prototypeName} was not found, creating new renderer");
        return NewMeshInstanceRenderer();
    }

    private static MeshInstanceRenderer NewMeshInstanceRenderer()
    {
        Material newMaterial = new Material(Shader.Find("Standard")) { enableInstancing = true };
        MeshInstanceRenderer renderer = new MeshInstanceRenderer {
            castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
            receiveShadows = false,
            mesh = new Mesh(),
            material = newMaterial
        };
        return renderer;
    }
}
