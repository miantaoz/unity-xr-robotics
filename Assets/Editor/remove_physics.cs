using Unity.Robotics.UrdfImporter;
using UnityEngine;
using UnityEditor;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine.PlayerLoop;

public class ComponentStripper
{
    // This creates a clickable menu item at the top of the Unity Editor
    [MenuItem("Tools/Remove Specific Component From Selected Prefabs")]
    public static void RemoveComponents()
    {
        // Get all currently selected objects in the Project window
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Please select at least one prefab in the Project window.");
            return;
        }

        foreach (GameObject obj in selectedObjects)
        {
            // Get the actual file path on your file system (e.g., Assets/Prefabs/MyPrefab.prefab)
            string assetPath = AssetDatabase.GetAssetPath(obj);

            // Guard clause: Ensure we are only touching actual saved prefab assets, not scene instances
            if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".prefab"))
            {
                Debug.LogWarning($"Skipping {obj.name}: Not a valid prefab asset.");
                continue;
            }

            // EditPrefabContentsScope safely opens the prefab in the background
            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
            {
                // This is the root GameObject of the prefab being edited
                GameObject prefabRoot = editingScope.prefabContentsRoot;

                // CHANGE THIS TYPE: Replace 'Controller' with your target component.
                // The 'true' parameter ensures it searches inactive GameObjects as well.
                var componentsToRemove = prefabRoot.GetComponentsInChildren<UrdfRobot>(true);

                int removedCount = 0;
                
                // We must iterate backwards or capture them in an array because 
                // modifying a collection while iterating over it causes errors.
                for (int i = componentsToRemove.Length - 1; i >= 0; i--)
                {
                    // DestroyImmediate is required in Editor scripts (Destroy() is for runtime)
                    Object.DestroyImmediate(componentsToRemove[i], true);
                    removedCount++;
                }

                Debug.Log($"Successfully removed {removedCount} components from {prefabRoot.name}.");
                
                // Once the 'using' block ends, Unity automatically saves the prefab to disk.
            }
        }
    }
}
