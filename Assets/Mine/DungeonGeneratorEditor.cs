using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonGenerator dg = (DungeonGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            dg.GenerateDungeon(dg.roomNumber);
        }
    }
}
