using UnityEngine;
using System.Collections;

namespace Player
{


    public class Condition_Knockback : Condition
    {
        // custom Variables
        // -----------
        protected Vector3 m_Direction;
        protected Player_Movement m_MovementController;

        protected float m_Speed;
        protected float m_SpeedFallof;

        protected const string m_particleEffectName = "Effects/FX_KnockbackSmoke";
        protected GameObject m_particleEffectObject;

        // Unity's Methods
        //---------
        // Use this for initialization
        void Start()
        {
            m_MovementController = GetComponent<Player_Movement>();

            // Set mutations
            m_AllowPlayerMovement = false;
            m_AllowPLayerLook = true;
            m_AllowPlayerAttack = false;

            m_Duration = 1f; // debug value, load this using a settings mechanic later (.4 seems about right, maybe even .3 )
            m_Speed = 20f;
            m_SpeedFallof = 1f;

            // add the knockback particle effect
            m_particleEffectObject = (GameObject) Instantiate( Resources.Load(m_particleEffectName) , transform.position +  Vector3.up * .3f , transform.rotation );
            m_particleEffectObject.transform.parent = this.transform;
        }

        // Update is called once per frame
        void Update()
        {
            // Debug, slightly move player in given direciton
            PlayerController owner = GetComponentInParent<PlayerController>();
            if (owner == null)
                Debug.LogError("Player was given a knockback... but there is no player?");





            // End state calculation
            double targetTime = (m_StartTime + m_Duration);
            if (targetTime  <= GameTime.Instance.Time)
            {
                // destroy particle effect
                GameObject.Destroy(m_particleEffectObject);
                Debug.Log(string.Format("Knockback ended at time {0} , was given {1} , startime: {2} , duration {3}", new object[] { GameTime.Instance.Time, targetTime, m_StartTime, m_Duration }));
                m_ConditionController.RemoveCondition( this.GetType());
            }


            // degrade movement speed;
            float frameSpeed =Mathf.Lerp(0, m_Speed, (((float)targetTime - (float)GameTime.Instance.Time) / (float)m_Duration )/ m_SpeedFallof);


            // movement code
            Vector3 allowedMovement = m_MovementController.GetMaxMovementDirection(m_Direction);
            owner.transform.position += (allowedMovement * Time.deltaTime * frameSpeed);




        }


        // Initialisation
        //---------

        /// <summary>
        /// First argument = Direction
        /// </summary>
        /// <param name="args"></param>
        public override void SetArguments( object[] args)
        {
            if (args == null)
                Debug.LogError("Knockback requires some arguments which were not given.");

            m_Direction = (Vector3)args[0];
        }

    }


}