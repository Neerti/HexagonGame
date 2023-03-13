using System;
using Arch.Core;
using Arch.System;
using HexagonGame.ECS.Components;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Systems;

public class CameraSystem : BaseSystem<World, float>
{
	private readonly QueryDescription _desc = new QueryDescription().WithExclusive<Position, Camera>();
	
	public CameraSystem(GameRoot root, World world) : base(world)
	{
		
	}

	public override void Initialize()
	{
		World.Query(in _desc, (ref Position pos, ref Camera camera) =>
			{
				pos.WorldPosition = new Vector3(0, 0, 0);
				camera.Radius = 10;
				camera.PolarAngle = MathHelper.ToRadians(45);
				camera.AzimuthAngle = MathHelper.ToRadians(90);
			}
		);
	}

	public override void Update(in float deltaTime)
	{
		World.Query(in _desc, (ref Position pos, ref Camera cam) =>
			{
				var newX = (float) (pos.WorldPosition.X +
				                    cam.Radius * Math.Cos(cam.PolarAngle) *
				                    Math.Cos(cam.AzimuthAngle));
				var newY = (float) (pos.WorldPosition.Y + cam.Radius * Math.Sin(cam.PolarAngle));
				var newZ = (float) (pos.WorldPosition.Z +
				                    cam.Radius * Math.Cos(cam.PolarAngle) *
				                    Math.Sin(cam.AzimuthAngle));
				cam.ViewPosition = new Vector3(newX, newY, newZ);
			}
		);
	}
}

/*

    x = c.x + r*cos(p)*cos(a)
    y = c.y + r*sin(p)
    z = c.z + r*cos(p)*sin(a) 
*/