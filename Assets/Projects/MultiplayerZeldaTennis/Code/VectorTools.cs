using UnityEngine;
using System.Collections;


namespace Toolbox
{
    public class VectorTools
    {
        /// <summary>
        /// Calculates the point at which 2 vectors intersect. Note that both vectors need to lie in the same plane for this to work.
        /// </summary>
        /// <param name="Xpos">POINT X</param>
        /// <param name="Xdir">POINT X's Direction</param>
        /// <param name="Ypos">POINT Y</param>
        /// <param name="Ydir">POINT Y's Direction</param>
        /// <returns>A point at whihc these vectors intersect</returns>
        public static Vector3 IntersectionPoint(Vector3 Xpos, Vector3 Xdir,
                                                Vector3 Ypos, Vector3 Ydir)
        {
            // rule:
            // http://www.cimt.plymouth.ac.uk/projects/mepres/step-up/sect4/index.htm
            // B = AB * c
            // c = sin(c) * |AC| / sin(b) 
            Vector3 contactPoint = Vector3.zero;

            Vector3 AC = Ypos - Xpos;
            Vector3 CA = Xpos - Ypos;

            Vector3 CB = Ydir * -1f;
            Vector3 AB = Xdir;


            float angleA = Mathf.Abs(Vector3.Angle(AB, AC));
            float angleC = Mathf.Abs(Vector3.Angle(CA, CB));
            float angleB = 180f - angleA - angleC;

            
            // if they are NOT on the same line
            if (angleB != 0)
            {
                float AB_Length = Mathf.Sin(Mathf.Deg2Rad * angleC) * ((AC).magnitude / Mathf.Sin(Mathf.Deg2Rad * angleB));
                contactPoint = AB.normalized * AB_Length;
            }
            else contactPoint = (Xpos - Ypos) * 0.5f; // pick point halfway between TODO: ACCOUNT FOR VELOCITY

            return contactPoint + Xpos;
        }
    }
}


