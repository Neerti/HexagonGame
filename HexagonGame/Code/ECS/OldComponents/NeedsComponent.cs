using System.Collections.Generic;

namespace HexagonGame.ECS.Components;

public enum NeedsTag
{
	Base,
	Calories,
	Hydration
}

public struct NeedsComponent
{
	public Dictionary<NeedsTag, Need> Needs;

	public NeedsComponent()
	{
		Needs = new Dictionary<NeedsTag, Need>();
	}
}

public struct Need
{
	public float RequiredDaily;
	public float Deficiency;
	public float MaxDeficiency;
}