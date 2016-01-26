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
        protected float m_StartTime;
        

        // Mutators for other scripts
        protected bool m_AllowPlayerMovement;
        protected bool m_AllowPLayerLook;
        protected bool m_AllowPlayerAttack;

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

        public bool AllowPlayerAttack
        {
            get { return m_AllowPlayerAttack; }
        }
        #endregion

        
        // Use this for initialization
        void Start()
        {
            m_AllowPlayerMovement = true;
            m_AllowPLayerLook = true;
            m_AllowPlayerAttack = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetConditionController(ConditionController controller)
        {
            m_ConditionController = controller;
        }

        public void SetStartTime(double photontime)
        {
            m_StartTime = (float)photontime;
        }

        /// <summary>
        /// Set condition specific arguments ( if any )
        /// </summary>
        /// <param name="arguments"></param>
        public virtual void SetArguments( object[] arguments)
        {

        }


    }
}
