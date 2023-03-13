using System;
using Arch.Core;
using Arch.System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.Systems;

namespace HexagonGame.Universes;
/// <summary>
/// The Universe object holds all mutable state for a particular save game, and is the starting
/// point for serialization and deserialization.
/// Multiple Universes can exist at once, however that is intended for unit testing only, to improve isolation.
/// One Universe is used in normal gameplay.
/// </summary>
public class Universe
{
	public World World;

	public Group<float> UpdateSystems;
	public Group<float> DrawSystems;

	public DateTime Calendar;

	public Entity CameraEntity;

	public void Initialize(GameRoot root)
	{
		CameraEntity = World.Create<Position, Camera>();
		
		UpdateSystems = new Group<float>(
			new InputSystem(root, World),
			new CameraSystem(root, World));
		DrawSystems = new Group<float>(
			new RenderingSystem(root, World),
			new UISystem(root, World));
		
		UpdateSystems.Initialize();
		DrawSystems.Initialize();
	}

	public void Cleanup(GameRoot root)
	{
		UpdateSystems.Dispose();
		DrawSystems.Dispose();
	}
}