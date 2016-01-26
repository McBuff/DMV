#define DEBUG
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Player
{
    
public class ConditionController {

        private PlayerController m_playerController;

        private List<Condition> m_ConditionsList;

        /// <summary>
        /// Is the player allowed to move? Checks all conditions
        /// </summary>
        /// <returns></returns>
        public bool AllowPlayerMovement()
        {
            // if ANY condition prohibits the player from moving, returns false
            bool result = true;


            for (int i = 0; i < m_ConditionsList.Count; i++)
            {
                if (m_ConditionsList[i].AllowPlayerMovement == false)
                    result = false;
            }

            return result;
        }

        /// <summary>
        /// Is the player allowed to look? Checks all conditions
        /// </summary>
        public bool AllowPlayerLook()
        {
            // if ANY condition prohibits the player from moving, returns false
            bool result = true;


            for (int i = 0; i < m_ConditionsList.Count; i++)
            {
                if (m_ConditionsList[i].AllowPlayerLook == false)
                    result = false;
            }

            return result;
        }

        //c-tor
        public ConditionController(PlayerController playerController)
        {
            m_playerController = playerController;
            m_ConditionsList = new List<Condition>();
        }

        /// <summary>
        /// Adds a condition to this controller owner ( Player )
        /// </summary>
        /// <param name="type"></param>
        /// <param name="netstarttime">Optional: starttime of this event over the network</param>
        /// <param name="args">Group of arguments</param>
        /// <returns>New condition Component or Null if this condition is already on the player</returns>
        public Condition AddCondition(Type type, double netstarttime = 0d, object[] args = null)
        {
#if DEBUG
            Debug.Log("Adding new condition");
#endif
            if (HasCondition(type))
                return null;

            Condition newCondition = (Condition) m_playerController.gameObject.AddComponent( type );
            m_ConditionsList.Add(newCondition);

            newCondition.SetConditionController(this);

            // init additional data
            newCondition.SetStartTime(netstarttime);
            newCondition.SetArguments(args);

            return newCondition;
        }


        /// <summary>
        /// Removes and destroys condition to this player
        /// </summary>
        /// <param name="type"></param>
        public void RemoveCondition(Type type)
        {
            Condition condition = GetCondition(type);
            if (condition == null)
            {
                Debug.LogWarning("ConditionController: Instructed to remove condition that is not afflicting me.");
                return; // not removed, but it's not there anyway
            }

            m_ConditionsList.Remove(condition);
            GameObject.Destroy(condition);
        }

        public void RemoveAllConditions()
        {
            for (int i = m_ConditionsList.Count -1 ; i >= 0 ; i--)
            {
                RemoveCondition( m_ConditionsList[i].GetType() );
            }
        }

        /// <summary>
        /// Is this condition already applied tot he player
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool HasCondition(Type type)
        {
            Condition cond = GetCondition(type);
            if (cond == null)
                return false;
            return true;
        }

        /// <summary>
        /// Returns a condition of given type IF the player has it. otherwise returns null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Null or a Condition applied to the player</returns>
        public Condition GetCondition(Type type)
        {
            for (int i = 0; i < m_ConditionsList.Count; i++)
            {
                if (m_ConditionsList[i].GetType() == type)
                    return m_ConditionsList[i];
            }
            return null;
        }
	
    }   

}
