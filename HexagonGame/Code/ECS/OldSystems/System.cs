using HexagonGame.ECS.Worlds;

namespace HexagonGame.ECS.Systems;

public abstract class System
{
	public virtual void Initialize(GameRoot game) { return; }

	public virtual void Process(GameRoot root, OldWorld oldWorld) { return; }
}