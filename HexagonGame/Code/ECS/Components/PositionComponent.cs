using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Components;

public struct PositionComponent
{
	public Vector2 Position;
	public float Rotation;

	public PositionComponent(Vector2 position)
	{
		Position = position;
		Rotation = 0f;
	}

	public PositionComponent(int newX, int newY)
	{
		Position = new Vector2(newX, newY);
		Rotation = 0f;
	}
}