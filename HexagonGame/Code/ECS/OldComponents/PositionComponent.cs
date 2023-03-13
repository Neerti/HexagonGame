using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Components;

public struct PositionComponent
{
	public Vector2 Position;

	public PositionComponent(Vector2 position)
	{
		Position = position;
	}

	public PositionComponent(int newX, int newY)
	{
		Position = new Vector2(newX, newY);
	}

	public PositionComponent(PositionComponent prototype)
	{
		Position = prototype.Position;
	}
}