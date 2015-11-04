using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

[CustomEditor(typeof(BouncingProjectile))]
public class BouncingProjectileEditor : Editor
{

    static bool visualizeProjectileStates = false;
    static float visualizationTime = .5f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        visualizeProjectileStates = EditorGUILayout.Toggle("Visualize path", visualizeProjectileStates);
        visualizationTime = EditorGUILayout.FloatField("Visualisation Time", visualizationTime);
    }

    //[DrawGizmo(GizmoType.Selected)]
    //static void DrawLinesForProjectileStates(BouncingProjectile projectile, GizmoType gizmoType)
    //{
    //    if (projectile == null || visualizeProjectileStates == false)
    //        return;

    //    if (projectile.HasPackages() == false)
    //        return;

    //    Gizmos.color = Color.red;

    //    List<Projectile2DState> points = projectile.GetAllPackages();

    //    for (int i = 0; i < points.Count - 1; i++)
    //    {
    //        Debug.DrawLine(points[i].Position3(), points[i + 1].Position3(), Color.yellow, 1f);
    //        //Gizmos.DrawLine(points[i].Position3(), points[i + 1].Position3());            
    //    }
    //}


    [DrawGizmo(GizmoType.Selected)]
    static void DrawProjectileStatePoints(BouncingProjectile projectile , GizmoType gizmoType)
    {
        if (projectile == null)
            return;

        if (projectile.HasPackages() == false)
            return;

        List<Projectile2DState> points = projectile.GetAllPackages();

        for (int i = 0; i < points.Count; i++)
        {
            // draw little + for points
            DrawCross(points[i].Position3() + Vector3.up, .2f, Color.white, visualizationTime);
        }

    }

    [DrawGizmo(GizmoType.Selected)]
    static void DrawProjectileStateDirections(BouncingProjectile projectile, GizmoType gizmoType)
    {
        if (projectile == null)
            return;

        if (projectile.HasPackages() == false)
            return;

        List<Projectile2DState> points = projectile.GetAllPackages();

        for (int i = 0; i < points.Count; i++)
        {
            // draw little + for points
            DrawArrow(points[i].Position3() + Vector3.up , points[i].Direction3() , .5f, Color.yellow, visualizationTime);
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
        Debug.DrawLine(position, position + direction  * size, color, duration);

    }
}