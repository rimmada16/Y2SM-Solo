/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pathfinding))]
public class PathfindingEditor : Editor
{
    private string[] _options = new string[] { "Roam bounds", "Follow Waypoints" };
    private int _selectedOptionIndex = 0;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Select Option:", EditorStyles.boldLabel);
        _selectedOptionIndex = EditorGUILayout.Popup(_selectedOptionIndex, _options);

        // Apply the selected option to all selected target objects
        foreach (var targetObject in targets)
        {
            var pathfinding = (Pathfinding)targetObject;
            if (_selectedOptionIndex == 1)
            {
                // Follow waypoints
                pathfinding.isTargetingWaypoints = true;
            }
            else
            {
                // Roam bounds
                pathfinding.isTargetingWaypoints = false;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}*/