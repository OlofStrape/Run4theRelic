using UnityEditor;
using UnityEngine;
using Run4theRelic.Puzzles.LaserRedirect;

namespace Run4theRelic.EditorTools
{
    public static class LaserRedirectBuilder
    {
        [MenuItem("Relic/Build Laser Redirect (demo)")]
        public static void BuildDemo()
        {
            // Root parent
            GameObject root = new GameObject("LaserRedirect_Demo");

            // Controller
            var controllerGO = new GameObject("LaserRedirectController");
            controllerGO.transform.SetParent(root.transform);
            var controller = controllerGO.AddComponent<LaserRedirectController>();

            // Emitter
            var emitterGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            emitterGO.name = "LaserEmitter";
            emitterGO.transform.SetParent(root.transform);
            emitterGO.transform.position = new Vector3(-2f, 1f, 0f);
            emitterGO.transform.forward = Vector3.right;
            Object.DestroyImmediate(emitterGO.GetComponent<Collider>()); // keep clean unless wanted
            var emitter = emitterGO.AddComponent<LaserEmitter>();
            emitter.maxDistance = 30f;
            emitter.maxBounces = 5;
            emitter.hitMask = LayerMask.GetMask("Default");
            var lr = emitterGO.AddComponent<LineRenderer>();
            lr.widthMultiplier = 0.02f;
            lr.positionCount = 0;
            lr.useWorldSpace = true;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            emitter.line = lr;

            // Mirrors (as cubes)
            for (int i = 0; i < 2; i++)
            {
                var mirrorGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mirrorGO.name = $"LaserMirror_{i + 1}";
                mirrorGO.transform.SetParent(root.transform);
                mirrorGO.transform.localScale = new Vector3(0.1f, 1.0f, 1.0f);
                mirrorGO.transform.position = new Vector3(-0.25f + i * 1.5f, 1f, 0f);
                mirrorGO.AddComponent<LaserMirror>();
                // collider remains for ray hits; rotation can be done by XRI in scene
            }

            // Receiver (as cube)
            var receiverGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            receiverGO.name = "LaserReceiver";
            receiverGO.transform.SetParent(root.transform);
            receiverGO.transform.position = new Vector3(3f, 1f, 0f);
            receiverGO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            var receiver = receiverGO.AddComponent<LaserReceiver>();
            receiver.holdSeconds = 1.5f;

            // Link controller to receiver list via SerializedObject for undo/serialization
            var so = new SerializedObject(controller);
            var receiversProp = so.FindProperty("receivers");
            receiversProp.arraySize = 1;
            receiversProp.GetArrayElementAtIndex(0).objectReferenceValue = receiver;
            so.ApplyModifiedPropertiesWithoutUndo();

            Selection.activeGameObject = root;
            Undo.RegisterCreatedObjectUndo(root, "Build Laser Redirect Demo");
        }
    }
}

