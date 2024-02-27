using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

// Custom attribute to define a box group with a specified title color
public class BoxGroupWithTitleColorAttribute : PropertyGroupAttribute
{
    public Color TitleColor { get; private set; }

    public BoxGroupWithTitleColorAttribute(string group, float r, float g, float b, float a = 1f) : base(group)
    {
        TitleColor = new Color(r, g, b, a);
    }
}

// Custom drawer to draw BoxGroupWithTitleColorAttribute
public class BoxGroupWithTitleColorAttributeDrawer : OdinGroupDrawer<BoxGroupWithTitleColorAttribute>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        // Set the background color of the box group title
        GUIHelper.PushColor(Attribute.TitleColor);

        SirenixEditorGUI.BeginBox();

        GUIHelper.PopColor();

        for (int i = 0; i < Property.Children.Count; i++)
        {
            Property.Children[i].Draw();
        }

        SirenixEditorGUI.EndBox();
    }
}
