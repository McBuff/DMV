using UnityEngine;
using System.Collections;


namespace Player
{
    /// <summary>
    /// Base class for player conditions like, sleep, stun, frozen, staggered? , posison I DON'T KNOW DON'T JUDGE ME OK!?
    /// </summary>
    public class Condition : MonoBehaviour
    {

        #region internals

        // Length of condition ( if timed )
        protected float m_Duration;

        // Mutators for other scripts
        protected bool m_AllowPlayerMovement;
        protected bool m_AllowPLayerLook;

        protected ConditionController m_ConditionController;

        #endregion

        #region public data

        /// <summary>
        /// Duration of this condition
        /// </summary>
        public float Duration
        {
            get { return m_Duration; }
        }

        

        /// <summary>
        /// Allow player to move while under this condition?
        /// </summary>
        public bool AllowPlayerMovement
        {
            get { return m_AllowPlayerMovement; }
        }

        public bool AllowPlayerLook
        {
            get { return m_AllowPLayerLook; }
        }
        #endregion

        
        // Use this for initialization
        void Start()
        {
            m_AllowPlayerMovement = true;
            m_AllowPLayerLook = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetConditionController(ConditionController controller)
        {
            m_ConditionController = controller;
        }

    }
}
