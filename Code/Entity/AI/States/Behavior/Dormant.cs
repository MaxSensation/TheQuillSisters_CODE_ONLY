// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Entity.AI.States
{
	/// <summary>
	///     Empty state that cannot transition anywhere unless externally acted upon.
	/// </summary>
	[CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Dormant")]
    public class Dormant : AIBaseState
    {
    }
}