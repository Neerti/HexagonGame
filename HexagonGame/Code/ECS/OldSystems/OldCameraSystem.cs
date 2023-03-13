using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Systems;

public class OldCameraSystem
{
	public void MoveCamera(OldWorld oldWorld, Vector2 direction, float cameraSpeed, GameTime gameTime)
	{
		oldWorld.PositionComponents.Get(oldWorld.CameraEntity).Position +=
			direction * cameraSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
	}

	public void SetCamera(OldWorld oldWorld, Vector2 newPosition)
	{
		oldWorld.PositionComponents.Get(oldWorld.CameraEntity).Position = newPosition;
	}
}