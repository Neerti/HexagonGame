using System;
using System.Collections.Generic;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.Worlds;

namespace HexagonGame.ECS.Systems;

public class LifecycleSystem : System
{
	// Proof of concept code.
	private readonly Dictionary<LifecycleComponent.Ages, int> _ages = new()
	{
		{LifecycleComponent.Ages.Baby, 0},
		{LifecycleComponent.Ages.Child, 6},
		{LifecycleComponent.Ages.Adolescent, 13},
		{LifecycleComponent.Ages.Adult, 18},
		{LifecycleComponent.Ages.Elderly, 70}
	};

	public new void Process(GameRoot root, OldWorld oldWorld)
	{
		for (var i = 0; i < oldWorld.LifecycleComponents.Count; i++)
		{
			var component = oldWorld.LifecycleComponents.Elements[i];
			
			// Proof of concept code.
			var age = (oldWorld.Calendar - component.Birthday).TotalDays / 365.25f; // This is pretty dumb.
			var currentAge = LifecycleComponent.Ages.Baby;
			foreach (var pair in _ages)
			{
				if (age > pair.Value)
				{
					currentAge = pair.Key;
				}
				else
				{
					break;
				}
			}

			component.CurrentAge = currentAge;
		}
	}
}