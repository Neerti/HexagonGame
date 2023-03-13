using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Components;

public struct AppearanceComponent
{
	public Color SpriteColor = Color.White;
	public string TextureName;

	public AppearanceComponent(string textureName, Color color)
	{
		SpriteColor = color;
		TextureName = textureName;
	}

	public AppearanceComponent(AppearanceComponent prototype)
	{
		SpriteColor = prototype.SpriteColor;
		TextureName = prototype.TextureName;
	}
	
}