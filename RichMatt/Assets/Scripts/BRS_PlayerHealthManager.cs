using UnityEngine;
using UnityEngine.UI;
//---------------------------------------------------------------------------------------------------
//This script is provided as a part of The Polygon Pilgrimage
//Subscribe to https://www.youtube.com/user/mentallic3d for more great tutorials and helpful scripts!
//---------------------------------------------------------------------------------------------------

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_PlayerHealthManager : MonoBehaviour
    {
        /// <summary>
        /// Amount of health this Manager currently has.
        /// </summary>
        [Header("---Player Health Parameters---")]
        [SerializeField] private float currentHealth = 100;

        /// <summary>
        /// Maximum amount of health this Manager is allowed to have at one time.
        /// </summary>
        [SerializeField] private int maxHealth = 100;

        /// <summary>
        /// Is this health manager current health above 0?
        /// </summary>
        [SerializeField] private bool isAlive = true;

        /// <summary>
        /// UI Component to visually show Players their health. Bots do not need one of these.
        /// </summary>
        [Header("---Health UI Slider---")]
        [SerializeField] private Slider healthSlider;
        
        // Use this for initialization
        void Start()
        {
            isAlive = true;//start the game alive

            if (healthSlider)
            {
                healthSlider.maxValue = maxHealth;//max
                healthSlider.minValue = 0;//health cannot go below 0
                
                //update UI
                healthSlider.value = currentHealth;
            }
        }

        /// <summary>
        /// Change the amount of health the HealthManager has by given amount.
        /// </summary>
        /// <param name="changeAmount">Amount of health to add / remove.</param>
        public void ChangeHealth(float changeAmount)
        {
            currentHealth += changeAmount;

            if (currentHealth <= 0)
            {
                isAlive = false;
                //do death stuff
            }

            //health cannot be less than 0 or greater than MaxHealth
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            //update UI
            if (healthSlider)
            {
                healthSlider.value = currentHealth;
            }
        }

        /// <summary>
        /// Sets this health manager to have maximum health. "Full heal".
        /// </summary>
        [ContextMenu("SetToMaxHealth()")]
        public void SetToMaxHealth()
        {
            currentHealth = maxHealth;

            //update UI
            if (healthSlider)
            {
                healthSlider.value = currentHealth;
            }
        }

        /// <summary>
        /// Get the Max Health of this health manager.
        /// </summary>
        /// <returns>A copy of the unit's current max health.</returns>
        public int GetMaxHealth()
        {
            return maxHealth;
        }

        /// <summary>
        /// Increase or decrease the unit's max health. Level-Up! or new equipment might change this amount.
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeMaxHealth(int amount)
        {
            maxHealth += amount;
        }
    }
}
