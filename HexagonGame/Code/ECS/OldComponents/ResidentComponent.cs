using System.Collections.Generic;

namespace HexagonGame.ECS.Components;

public struct ResidentComponent
{
	public List<int> Residents;

	public ResidentComponent()
	{
		Residents = new List<int>();
	}
}