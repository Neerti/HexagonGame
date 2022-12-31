using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class TextureSystem
{
	public Dictionary<string, Texture2D> Textures;
	
	public void LoadContent(Game1 game)
	{
		Textures = new Dictionary<string, Texture2D>
		{
			["hexagon"] = game.Content.Load<Texture2D>("Images/generic_hexagon_64"),
			["hexagon_outline"] = game.Content.Load<Texture2D>("Images/hexagon_outline64"),
			["pine_tree"] = game.Content.Load<Texture2D>("Images/pine_tree"),
			["box_128x64"] = game.Content.Load<Texture2D>("Images/box_128x64")
		};
	}
}