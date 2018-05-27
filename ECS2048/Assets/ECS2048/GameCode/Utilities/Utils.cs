using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace TP.ECS2048
{
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
            return NewMeshInstanceRenderer(PrimitiveType.Cube);
        }

        public static MeshInstanceRenderer NewMeshInstanceRenderer(PrimitiveType primitiveType)
        {
            var cube = GameObject.CreatePrimitive(primitiveType);
            Material newMaterial = new Material(Bootstrap.GameSettings.MaterialToCopy) { enableInstancing = true };
            MeshInstanceRenderer renderer = new MeshInstanceRenderer {
                castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows = false,
                mesh = cube.GetComponent<MeshFilter>().mesh,
                material = newMaterial
            };
            Object.Destroy(cube);
            return renderer;
        }
    }
}
