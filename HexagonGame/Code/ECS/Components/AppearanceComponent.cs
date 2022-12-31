using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Components;

public struct AppearanceComponent
{
	public Texture2D SpriteTexture;
	public Color SpriteColor = Color.White;

	public AppearanceComponent(Texture2D texture, Color color)
	{
		SpriteTexture = texture;
		SpriteColor = color;
	}

	public AppearanceComponent(Texture2D texture)
	{
		SpriteTexture = texture;
	}
}