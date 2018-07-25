using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spanner {
    public class Mortal : MonoBehaviour {

        public MortalSettings settings;

        #region events
        public delegate void OnInitializedHandler(Mortal mortal);
        public delegate void OnDamageHandler(Mortal mortal, int amount, int currentHealth);
        public delegate void OnHealHandler(Mortal mortal, int amount, int currentHealth);
        public delegate void OnDeathHandler(Mortal mortal);
        public delegate void OnReviveHandler(Mortal mortal, int currentHealth);

        public event OnInitializedHandler OnInitialized;
        public event OnDamageHandler OnDamage;
        public event OnHealHandler OnHeal;
        public event OnDeathHandler OnDeath;
        public event OnReviveHandler OnRevive;
        #endregion

        #region status
        private int _maxHealth;
        private int _currentHealth;
        private bool _dead;
        #endregion

        #region properties
        public int MaxHealth {get{return _maxHealth;}}
        public int CurrentHealth {get{return _currentHealth;}}
        public bool Dead {get{return _dead;}}
        #endregion

        private void Start() {
            if(settings.initializeOnStart) {
                Initialize();
            }
        }

        /// <summary>
        /// Applies the public settings to the Mortal
        /// </summary>
        public void Initialize() {
            Initialize(settings);
        } 

        /// <summary>
        /// Applies the given settings to the Mortal
        /// </summary>
        /// <param name="newSettings">The settings to apply</param>
        public void Initialize(MortalSettings newSettings) {
            // Clean the user's horrible input
            SanitizeSettings(newSettings);
            // Compute the maximum and current health from the sanitized settings value
            if (newSettings.randomizeHealth) {
                _maxHealth = Random.Range(newSettings.minHealthCap, newSettings.maxHealthCap + 1);
                _currentHealth = Mathf.Clamp(Random.Range(newSettings.minStartingHealth, newSettings.maxStartingHealth), 1, _maxHealth);
            } else {
                _maxHealth = newSettings.maxHealth;
                _currentHealth = newSettings.startingHealth;
            }

            _dead = false;

            // Let listeners know that the settings have been applied
            if (OnInitialized != null) {
                OnInitialized(this);
            }
        } 

        /// <summary>
        /// Checks whether or not the public settings are valid and sanitizes the user's input.
        /// </summary>
        private void SanitizeSettings() {
            settings = SanitizeSettings(settings);
        }

        /// <summary>
        /// Checks whether or not the given settings are valid and sanitizes the user's input.
        /// </summary>
        /// <param name="newSettings">The settings to sanitize</param>
        private MortalSettings SanitizeSettings(MortalSettings newSettings) {
            if (newSettings.randomizeHealth) {
                if (newSettings.minHealthCap <= 0) {
                    Debug.LogWarning("Minimum Health Cap must be greater than 0");
                    newSettings.minHealthCap = 1;
                }
                if (newSettings.maxHealthCap < newSettings.minHealthCap) {
                    Debug.LogWarning("Maximum Health Cap must be greater than or equal to " + newSettings.minHealthCap);
                    newSettings.maxHealthCap = newSettings.minHealthCap;
                }
                if (newSettings.minStartingHealth <= 0 || newSettings.minStartingHealth > newSettings.maxHealthCap) {
                    Debug.LogWarning("Minimum Starting Health must be greater than 0 and less than or equal to " + newSettings.maxHealthCap);
                    newSettings.minStartingHealth = Mathf.Clamp(newSettings.minStartingHealth, 1, newSettings.maxHealthCap);
                }
                if (newSettings.maxStartingHealth < newSettings.minStartingHealth || newSettings.maxStartingHealth > newSettings.maxHealthCap) {
                    Debug.LogWarning("Maximum Starting Health must be greater than or equal to " + newSettings.minStartingHealth +
                        " and less than or equal to " + newSettings.maxHealthCap);
                    newSettings.maxStartingHealth = Mathf.Clamp(newSettings.maxStartingHealth, newSettings.minStartingHealth, newSettings.maxHealthCap);
                }
            } else {
                if (newSettings.maxHealth <= 0) {
                    Debug.LogWarning("Maximum health must be greater than 0");
                    newSettings.maxHealth = 1;
                }
                if (newSettings.startingHealth <= 0 || newSettings.startingHealth > newSettings.maxHealth) {
                    Debug.LogWarning("Starting health must be greater than 0 and less than or equal to " + newSettings.maxHealth);
                    newSettings.startingHealth = Mathf.Clamp(newSettings.startingHealth, 1, newSettings.maxHealth);
                }
            }

            return newSettings;
        }

        /// <summary>
        /// Subtracts the given damage amount from the current health and raises the OnDamage event.
        /// Afterwards, checks if the minimum health has dropped to zero or lower. If so, calls the Die method.
        /// </summary>
        /// <param name="amount">The amount of damage to subtract from the current health</param>
        public void Damage(int amount) {
            if (!_dead) {
                if (amount < 0) {
                    Debug.LogError("Damage amount must be greater than or equal to zero");
                } else {
                    _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
                    if (OnDamage != null) {
                        OnDamage(this, amount, _currentHealth);
                    }
                    if (_currentHealth <= 0) {
                        Die();
                    }
                }
            }
        }

        /// <summary>
        /// Adds the given heal amount to the current health and raises the OnHeal event.
        /// Healing may not bring the current health value above the maximum health value.
        /// </summary>
        /// <param name="amount">The amount of health points to add to the current health</param>
        public void Heal(int amount) {
            if (!_dead) {
                if (amount < 0) {
                    Debug.LogError("Heal amount must be greater than or equal to zero");
                } else {
                    _currentHealth = Mathf.Clamp(_currentHealth + amount, 1, _maxHealth);
                    if (OnHeal != null) {
                        OnHeal(this, amount, _currentHealth);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the OnDeath event and sets the dead flag to true.
        /// </summary>
        public void Die() {
            _dead = true;
            if (OnDeath != null) {
                OnDeath(this);
            }
        }

        /// <summary>
        /// Sets the dead flag to false, raises the OnRevive event, and calculates the current
        /// health value from the settings class. Use this method if you don't want to specify
        /// a health value this mortal will have when revived.
        /// </summary>
        public void Revive() {
            _dead = false;

            if (settings.randomizeHealth) {
                _currentHealth = Mathf.Clamp(Random.Range(settings.minStartingHealth, settings.maxStartingHealth), 1, _maxHealth);
            } else {
                _currentHealth = settings.startingHealth;
            }

            if (OnRevive != null) {
                OnRevive(this, _currentHealth);
            }
        }

        /// <summary>
        /// Sets the dead flag to false, raises the OnRevive event, and calculates the current
        /// health value from the given value. The given value is clamped between 1 and the
        /// maximum health specified on the MortalSettings for this Mortal.
        /// </summary>
        /// <param name="currentHealth">The health to give to this mortal when it is revived</param>
        public void Revive(int currentHealth) {
            _dead = false;
            _currentHealth = Mathf.Clamp(currentHealth, 1, settings.maxHealth);
            if (OnRevive != null) {
                OnRevive(this, _currentHealth);
            }
        }
    }
}
