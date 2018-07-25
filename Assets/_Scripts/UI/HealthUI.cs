using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Spanner;

[RequireComponent(typeof(Text))]
public class HealthUI : MonoBehaviour {

    public Mortal targetMortal;

    private Text _text;

    private void Start() {
        _text = GetComponent<Text>();
        if (targetMortal != null) {
            _text.text = "Health: " + targetMortal.settings.startingHealth + "/" + targetMortal.settings.maxHealth;
        }
    }

    private void OnEnable() {
        if (targetMortal != null) {
            targetMortal.OnDamage += OnDamageHandler;
            targetMortal.OnHeal += OnHealHandler;
            targetMortal.OnDeath += OnDeathHandler;
            targetMortal.OnInitialized += OnInitializedHandler;
        }
    }

    private void OnDisable() {
        if (targetMortal != null) {
            targetMortal.OnDamage -= OnDamageHandler;
            targetMortal.OnHeal -= OnHealHandler;
            targetMortal.OnDeath -= OnDeathHandler;
            targetMortal.OnInitialized -= OnInitializedHandler;
        }
    }

    public void OnInitializedHandler(Mortal mortal) {
        _text = GetComponent<Text>();
        if (targetMortal != null) {
            _text.text = "Health: " + targetMortal.CurrentHealth + "/" + targetMortal.MaxHealth;
        }
    }

    public void OnDamageHandler(Mortal mortal, int amount, int currenthealth) {
        _text.text = "Health: " + mortal.CurrentHealth + "/" + mortal.MaxHealth;
    }

    public void OnHealHandler(Mortal mortal, int amount, int currenthealth) {
        _text.text = "Health: " + mortal.CurrentHealth + "/" + mortal.MaxHealth;
    }

    public void OnDeathHandler(Mortal mortal) {
        _text.text = "Dead";
    }
}
