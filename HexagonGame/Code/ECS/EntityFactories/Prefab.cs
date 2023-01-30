using System;
using HexagonGame.ECS.Components;

namespace HexagonGame.ECS.EntityFactories;

public struct Prefab
{
	public PositionComponent? PositionComponent;
	public AppearanceComponent? AppearanceComponent;
	public TileAttributeComponent? TileAttributeComponent;
}