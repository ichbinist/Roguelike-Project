#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

// Odin Drawer to customize TableMatrix for boolean arrays
public class BooleanTableDrawer : OdinValueDrawer<bool[,]>
{
    private const float CellSize = 40f; // Adjust cell size as needed
    private const float Spacing = 5f; // Adjust spacing between cells as needed

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var valueEntry = this.ValueEntry;

        // Cast the parentObject to MonoBehaviour or ScriptableObject depending on your case
        InterfaceCache parentObject = valueEntry.Property.ParentValues[0] as InterfaceCache;
        if (parentObject == null)
        {
            Debug.LogError("Parent object is not a MonoBehaviour.");
            return;
        }

        // Create a SerializedObject for the parentObject
        var serializedParentObject = new SerializedObject(parentObject);

        bool[,] itemGrid = (bool[,])valueEntry.SmartValue;

        int rows = itemGrid.GetLength(0);
        int cols = itemGrid.GetLength(1);

        float totalWidth = rows * (CellSize + Spacing) - Spacing;
        float totalHeight = cols * (CellSize + Spacing) - Spacing;

        Rect cellRect = GUILayoutUtility.GetRect(totalWidth, totalHeight);

        // Calculate offset to center the grid
        float xOffset = (cellRect.width - totalWidth) / 2;
        float yOffset = (cellRect.height - totalHeight) / 2;

        EditorGUI.BeginChangeCheck();

        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                Rect rect = new Rect(cellRect.x + xOffset + x * (CellSize + Spacing), cellRect.y + yOffset + y * (CellSize + Spacing), CellSize, CellSize);
                bool newValue = EditorGUI.Toggle(rect, itemGrid[x, y]);
                if (EditorGUI.EndChangeCheck())
                {
                    itemGrid[x, y] = newValue;
                    MarkDirty(parentObject);
                }

                Color color = itemGrid[x, y] ? Color.green : Color.red;
                EditorGUI.DrawRect(rect, color);
            }
        }

        // Ensure to apply modifications at the end
        serializedParentObject.ApplyModifiedProperties();
    }

    // Mark the InterfaceCache object as dirty
    private void MarkDirty(InterfaceCache serializedParentObject)
    {
        if (serializedParentObject != null)
        {
            EditorUtility.SetDirty(serializedParentObject);
        }
    }
}
#endif
