using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Systems;

public class CameraSystem
{
	public void MoveCamera(World world, Vector2 direction, float cameraSpeed, GameTime gameTime)
	{
		world.PositionComponents.Get(world.CameraEntity).Position +=
			direction * cameraSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
	}

	public void SetCamera(World world, Vector2 newPosition)
	{
		world.PositionComponents.Get(world.CameraEntity).Position = newPosition;
	}
}