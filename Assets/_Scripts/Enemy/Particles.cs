using UnityEngine;
using System.Collections;
using Spanner;

public class Particles : MonoBehaviour {

    public ParticleSystem minusSystem;
    public ParticleSystem plusSystem;
    public Mortal targetMortal;

    public float length;

    private float _minusTimer;
    private float _plusTimer;

    public void Start() {
        minusSystem.Stop();
        plusSystem.Stop();
    }

    private void OnEnable() {
        targetMortal.OnDamage += OnDamageHandler;
        targetMortal.OnHeal += OnHealHandler;
    }

    private void OnDisable() {
        targetMortal.OnDamage -= OnDamageHandler;
        targetMortal.OnHeal -= OnHealHandler;
    }

    private void Update() {
        if (Time.time - _minusTimer >= length) {
            minusSystem.Stop();
        }

        if (Time.time - _plusTimer >= length) {
            plusSystem.Stop();
        }
    }

    public void OnDamageHandler(Mortal mortal, int amount, int currentHealth) {
        minusSystem.Play();
        _minusTimer = Time.time;
    }

    public void OnHealHandler(Mortal mortal, int amount, int currentHealth) {
        plusSystem.Play();
        _plusTimer = Time.time;
    }
}
