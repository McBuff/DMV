using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(BouncingProjectile))]
public class BouncingProjectileEditor : Editor
{

    static bool visualizeProjectileStates = false;
    static float visualizationTime;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        visualizeProjectileStates = EditorGUILayout.Toggle("Visualize path", visualizeProjectileStates);
        visualizationTime = EditorGUILayout.FloatField("Visualisation Time", visualizationTime);
    }


    [DrawGizmo(GizmoType.Selected)]
    static void DrawProjectileStatePoints(BouncingProjectile projectile , GizmoType gizmoType)
    {
        if (projectile == null)
            return;

        if (projectile.HasPackages() == false)
            return;

        List<Projectile2DState> points = projectile.GetAllPackages();

        DrawSegment(projectile.previousPosition + Vector3.up * .99f, projectile.transform.position + Vector3.up * .99f, Color.blue, visualizationTime);
        Vector3 ballMoveDir = projectile.transform.position - projectile.previousPosition;

        DrawArrow(projectile.transform.position + Vector3.up * (.99f - (projectile.previousPosition - projectile.transform.position).magnitude * .1f) ,
            ballMoveDir,
            (projectile.previousPosition - projectile.transform.position).magnitude,
            Color.cyan, 
            visualizationTime);

        for (int i = 0; i < points.Count; i++)
        {
            // draw little + for points
            DrawCross(points[i].Position3() + Vector3.up, .2f, Color.white, visualizationTime);

            // draw little + for points
            DrawArrow(points[i].Position3() + Vector3.up, points[i].Direction3(), .5f, Color.yellow, visualizationTime);
        }

        

    }

    /// <summary>
    /// Draws a + in the editor
    /// </summary>
    /// <param name="position"></param>
    /// <param name="col"></param>
    static void DrawCross(Vector3 position, float size, Color color, float duration)
    {
        Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color, duration);

        Debug.DrawLine(position - Vector3.left * size, position + Vector3.left * size, color, duration);

    }

    /// <summary>
    /// Draws an arrow in a direction
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    static void DrawArrow(Vector3 position,Vector3 direction, float size, Color color, float duration)
    {
        Vector3 crossVec = Vector3.Cross(direction, Vector3.up);

        Debug.DrawLine(position, position + direction  * size, color, duration);

        Debug.DrawLine(position + direction * size, position + direction * size * .8f + crossVec * size * .2f, color, duration);
        Debug.DrawLine(position + direction * size, position + direction * size * .8f - crossVec * size * .2f, color, duration);

    }

    static void DrawSegment(Vector3 startpos, Vector3 endPos, Color color, float duration) {
        
        // draw update segmnets
        Debug.DrawLine(startpos, endPos  , color, visualizationTime);

        Vector3 crossPos = Vector3.Cross(startpos - endPos, Vector3.up);
        Debug.DrawLine(endPos + crossPos * .05f,
                        endPos - crossPos * .05f, Color.white, visualizationTime);
        Debug.DrawLine(endPos, endPos - Vector3.up, Color.white , visualizationTime);



    }
}
#endif