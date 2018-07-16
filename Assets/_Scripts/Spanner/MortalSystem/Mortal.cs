using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spanner {
    public class Mortal : MonoBehaviour {

        public MortalSettings settings;

        #region events
        public delegate void OnDamageHandler(Mortal mortal, int amount, int currentHealth);
        public delegate void OnHealHandler(Mortal mortal, int amount, int currentHealth);
        public delegate void OnDeathHandler(Mortal mortal);

        public event OnDamageHandler OnDamage;
        public event OnHealHandler OnHeal;
        public event OnDeathHandler OnDeath;
        #endregion

        #region status
        private int _currentHealth;
        private bool _dead;
        #endregion

        private void Start() {
            if (SettingsVerified()) {
                _currentHealth = settings.startingHealth;
                _dead = false;
            }
        }

        private bool SettingsVerified() {
            if (settings.maxHealth <= 0) {
                Debug.LogError("Maximum health must be greater than 0");
                return false;
            }
            if (settings.startingHealth <= 0 || settings.startingHealth > settings.maxHealth) {
                Debug.LogError("Starting health must be greater than 0 and less than " + settings.maxHealth);
                return false;
            }

            return true;
        }

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

        public void Die() {
            _dead = true;
            if (OnDeath != null) {
                OnDeath(this);
            }
        }
    }
}
