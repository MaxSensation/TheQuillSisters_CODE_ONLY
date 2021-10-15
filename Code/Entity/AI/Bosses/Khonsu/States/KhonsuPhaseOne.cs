// Primary Author :  Viktor Dahlberg - vida6631

using System;
using System.Threading;
using System.Threading.Tasks;
using Combat.ConditionSystem;
using Framework;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity.AI.Bosses.Khonsu.States
{
	[CreateAssetMenu(menuName = "States/EnemyStates/Bosses/Khonsu/KhonsuPhaseOne")]
	public class KhonsuPhaseOne : KhonsuPhase
	{
		[SerializeField]
		private SerializableTuple<float, float> attackDelaySpan = default;
		[SerializeField]
		private SerializableTuple<float, float> enragedDelaySpan = default;
		[SerializeField]
		private int spawnedMummiesCount = default;
		[SerializeField]
		private int enragedSpawnedMummiesCount = default;
		[SerializeField]
		private AudioClip beginSummonAudioClip = default;
		[SerializeField]
		private AudioClip completeSummonAudioClip = default;
		[SerializeField]
		private ScriptObjVar<Component> audioSource = default;

		public override void Enter()
		{
			var span = enraged ? enragedDelaySpan : attackDelaySpan;
			FirstAttack(span, ((Khonsu) AI).Cancel);
			AI.AnimationHandler.OnAttackAnimationEnded += () => SecondAttack(span, ((Khonsu) AI).Cancel);
		}

		public override void Exit()
		{
			AI.AnimationHandler.OnAttackAnimationEnded = null;
		}

		private async void FirstAttack(SerializableTuple<float, float> span, CancellationTokenSource tokenSource)
		{
			try
			{
				await Task.Delay(TimeSpan.FromSeconds(Random.Range(span.item1, span.item2)), tokenSource.Token);
				SummonEnemyWave();
			} catch (TaskCanceledException) { }
		}

		private void SummonEnemyWave()
		{
			AI.AnimationHandler.OnAttackClimaxed += Trigger;
			AI.AnimationHandler.OnAttackCompleted += UnTrigger;
			AI.AnimationHandler.SetBool("Null", true);
			(audioSource.value as AudioSource)?.PlayOneShot(beginSummonAudioClip);
		}
		
		private void Trigger()
		{
			AI.AnimationHandler.OnAttackClimaxed -= Trigger;
			((Khonsu) AI).groundSpawner.SpawnEnemies(Environment.RoomManager.Enemy.MummyMelee, enraged ? enragedSpawnedMummiesCount : spawnedMummiesCount);
			((Khonsu) AI).platformSpawner.SpawnEnemies(Environment.RoomManager.Enemy.MummyRanged, ((Khonsu) AI).HealthGateThresholds);
			((Khonsu) AI).HealthGateThresholds = 0;
			(audioSource.value as AudioSource)?.PlayOneShot(completeSummonAudioClip);
		}
		
		private void UnTrigger()
		{
			AI.AnimationHandler.OnAttackCompleted -= UnTrigger;
			AI.AnimationHandler.SetBool("Null", false);
		}

		private async void SecondAttack(SerializableTuple<float, float> span, CancellationTokenSource tokenSource)
		{
			AI.AnimationHandler.OnAttackAnimationEnded = null;
			try
			{
				await Task.Delay(
					TimeSpan.FromSeconds(Random.Range(span.item1, span.item2)),
					((Khonsu) AI).Cancel.Token);
				BeginNova();
				AI.AnimationHandler.OnAttackClimaxed += MakeVulnerable;
				AI.AnimationHandler.OnAttackAnimationEnded += () => StateMachine.TransitionTo<KhonsuPhaseTwo>();
			} catch (TaskCanceledException) { }
		}

		

		private void MakeVulnerable()
		{
			AI.AnimationHandler.OnAttackClimaxed -= MakeVulnerable;
			ConditionManager.RemoveCondition(invincibilityCondition, AI);
			((Khonsu) AI).FadeOut(overlayFadeSpeed);
		}

		
	}
}