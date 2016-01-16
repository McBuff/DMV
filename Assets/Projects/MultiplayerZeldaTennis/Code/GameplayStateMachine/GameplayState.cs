using UnityEngine;
using System.Collections;
using Toolbox;


namespace DMV.GameplaystateManager
{
    /// <summary>
    /// base state for all gameplaystates
    /// </summary>
    public abstract class GameplayState
    {
        
        protected GameplayManager m_Owner;
        protected double m_StateStartTime;

       public GameplayState(GameplayManager owner , double stateStart)
        {
            m_Owner = owner;
            m_StateStartTime = stateStart;
        }

        protected bool m_IsInitialised = false;

        /// <summary>
        /// Initializes data in object
        /// </summary>
        public virtual void Init() {

            m_IsInitialised = true;
        }

        public abstract void Update();
        public abstract GameplayStateType GetCurrentStateType();

        /// <summary>
        /// Returns true if next state should be announced. Base handles debug overrides
        /// </summary>
        /// <returns></returns>
        public virtual bool isStateFinished()
        {
            /// Debug
            bool doManualProgression = DebugGUI.GetInstance().GameState_DoManualProgression();

            if (doManualProgression == false)
                return true;

            return doManualProgression && Input.GetKeyDown(KeyCode.F1);            
        }

        /// <summary>
        /// Tells owner to change states on next update cycle
        /// </summary>
        protected void AnnounceNextState(GameplayStateType stateType) {
            m_Owner.SetNextGameplayState(stateType);
        }

    }

    public enum GameplayStateType
    {
        waiting,
        countdown,
        battle,
        ending
    }
}
