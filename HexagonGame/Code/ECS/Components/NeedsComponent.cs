using System.Collections.Generic;

namespace HexagonGame.ECS.Components;

public enum NeedsTag
{
	Calories,
	Hydration
}

public struct NeedsComponent
{
	public Dictionary<NeedsTag, float> Needs;

	public NeedsComponent()
	{
		Needs = new Dictionary<NeedsTag, float>();
	}
}