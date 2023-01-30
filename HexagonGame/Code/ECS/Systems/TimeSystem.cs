using System;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Systems;

public class TimeSystem
{
	public void Tick(GameRoot game, GameTime gameTime)
	{
		game.FractionalTick += gameTime.ElapsedGameTime;
		if (!(game.FractionalTick.TotalSeconds >= game.TickDelay))
			return;

		game.FractionalTick -= TimeSpan.FromSeconds(game.TickDelay);
		game.World.Calendar = game.World.Calendar.AddHours(1);
	}
}