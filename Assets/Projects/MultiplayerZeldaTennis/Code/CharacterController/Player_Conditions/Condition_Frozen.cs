using UnityEngine;
using System.Collections;

namespace Player {


    public class Condition_Frozen : Condition  {

        // Use this for initialization
        void Start() {
            
            // Set mutations
            m_AllowPlayerMovement = false;
            m_AllowPLayerLook = false;

        }

        // Update is called once per frame
        void Update() {

        }
    }


}
