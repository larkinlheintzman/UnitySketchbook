using UnityEditor;
using UnityEngine;

namespace LevelGenerator.Scripts.Editor
{
    [CustomEditor(typeof(LevelGenerator))]
    public class GeneratorEditor : UnityEditor.Editor
    {

      LevelGenerator levelGenerator;

        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();



            using (var check = new EditorGUI.ChangeCheckScope())
            {
              base.OnInspectorGUI();
              if (check.changed)
              {
                // Debug.Log("requesting update of level from GUI");
                if (levelGenerator.isEnabled)
                {
                  levelGenerator.Cleanup();
                  levelGenerator.GenerateLevel();
                }

              }
            }
            if (GUILayout.Button("Add Section Template"))
            {
              levelGenerator.AddSectionTemplate();
            }

            if (GUILayout.Button("Add Dead End Template"))
            {
              levelGenerator.AddDeadEndTemplate();
            }
        }

        private void OnEnable()
        {
          levelGenerator = (LevelGenerator)target;
        }
    }
}
