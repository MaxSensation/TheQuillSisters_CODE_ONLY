//Author: Erik Pilström - erpi3245
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.HealthSystem {
	public class PlayerHealth : Health
	{
		public static System.Action PlayerHealed;
		[SerializeField]
		private ScriptObjVar<float> currentHealth = default;

		//Modifiers for altering the player's max health, for example when upgrading skill tree
		public enum MaxHealthModifiers {
			ADD,
			SUBTRACT,
			MULTIPLY,
			DIVIDE
		};

		private void Awake()
		{
			currentHealth.value = maxHealth.Value;
		}

		/// <summary>
		/// Heal the player, but do not exceed player's maximum health.
		/// </summary>
		/// <param name="healAmount">The amount to heal.</param>
		public void Heal(float healAmount)
		{
			if (currentHealth.value + healAmount >= maxHealth.Value)
			{
				currentHealth.value = maxHealth.Value;
			}
			else
			{
				currentHealth.value += healAmount;
			}
			PlayerHealed?.Invoke();
		}

		public override void TakeDamage(float damage)
		{
			if (IsInvincible)
			{
				return;
			}
			currentHealth.value -= damage;
			TookDamage?.Invoke(gameObject, damage);
			if (currentHealth.value <= 0)
			{
				Die();
			}
		}

		public override void Die()
		{
			EntityDied?.Invoke(gameObject);
		}

		/// <summary>
		/// Changes the player's max health by a given factor/amount, where the type of arithmetic operation is given as the modifier param.
		/// </summary>
		/// <param name="amount">Factor or amount to change the max health value by.</param>
		/// <param name="modifier">The type of arithmetic operation to perform.</param>
		public void AlterMaxHealth(float amount, MaxHealthModifiers modifier)
		{ 
			switch (modifier)
			{
				default:
				case MaxHealthModifiers.ADD:
					currentHealth.value += amount;
					break;
				case MaxHealthModifiers.SUBTRACT:
					currentHealth.value -= amount;
					break;
				case MaxHealthModifiers.MULTIPLY:
					currentHealth.value *= amount;
					break;
				case MaxHealthModifiers.DIVIDE:
					currentHealth.value /= amount;
					break;
			}
		}

		public override float GetCurrentHealth()
		{
			return currentHealth.value;
		}
	}
}
