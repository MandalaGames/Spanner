using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spanner {
    public class Mortal : MonoBehaviour {

        public MortalSettings settings;

        #region events
        public delegate void OnSettingsSanitizedHandler(Mortal mortal);
        public delegate void OnDamageHandler(Mortal mortal, int amount, int currentHealth);
        public delegate void OnHealHandler(Mortal mortal, int amount, int currentHealth);
        public delegate void OnDeathHandler(Mortal mortal);
        public delegate void OnReviveHandler(Mortal mortal, int currentHealth);

        public event OnSettingsSanitizedHandler OnSettingsSanitized;
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
            // Clean the user's horrible input
            SanitizeSettings();
            // Compute the maximum and current health from the sanitized settings value
            if (settings.randomizeHealth) {
                _maxHealth = Random.Range(settings.minHealthCap, settings.maxHealthCap + 1);
                _currentHealth = Mathf.Clamp(Random.Range(settings.minStartingHealth, settings.maxStartingHealth), 1, _maxHealth);
            } else {
                _maxHealth = settings.maxHealth;
                _currentHealth = settings.startingHealth;
            }

            // Let listeners know that the settings are finalized
            if (OnSettingsSanitized != null) {
                OnSettingsSanitized(this);
            }

            _dead = false;
        }

        /// <summary>
        /// Checks whether or not the given settings are valid and sanitizes the user's input.
        /// </summary>
        private void SanitizeSettings() {
            if (settings.randomizeHealth) {
                if (settings.minHealthCap <= 0) {
                    Debug.LogWarning("Minimum Health Cap must be greater than 0");
                    settings.minHealthCap = 1;
                }
                if (settings.maxHealthCap < settings.minHealthCap) {
                    Debug.LogWarning("Maximum Health Cap must be greater than or equal to " + settings.minHealthCap);
                    settings.maxHealthCap = settings.minHealthCap;
                }
                if (settings.minStartingHealth <= 0 || settings.minStartingHealth > settings.maxHealthCap) {
                    Debug.LogWarning("Minimum Starting Health must be greater than 0 and less than or equal to " + settings.maxHealthCap);
                    settings.minStartingHealth = Mathf.Clamp(settings.minStartingHealth, 1, settings.maxHealthCap);
                }
                if (settings.maxStartingHealth < settings.minStartingHealth || settings.maxStartingHealth > settings.maxHealthCap) {
                    Debug.LogWarning("Maximum Starting Health must be greater than or equal to " + settings.minStartingHealth +
                        " and less than or equal to " + settings.maxHealthCap);
                    settings.maxStartingHealth = Mathf.Clamp(settings.maxStartingHealth, settings.minStartingHealth, settings.maxHealthCap);
                }
            } else {
                if (settings.maxHealth <= 0) {
                    Debug.LogWarning("Maximum health must be greater than 0");
                    settings.maxHealth = 1;
                }
                if (settings.startingHealth <= 0 || settings.startingHealth > settings.maxHealth) {
                    Debug.LogWarning("Starting health must be greater than 0 and less than or equal to " + settings.maxHealth);
                    settings.startingHealth = Mathf.Clamp(settings.startingHealth, 1, settings.maxHealth);
                }
            }
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
                    _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, settings.maxHealth);
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
                    _currentHealth = Mathf.Clamp(_currentHealth + amount, 1, settings.maxHealth);
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
