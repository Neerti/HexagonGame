namespace HexagonGame.ECS.Components;

public struct TileAttributeComponent
{
	public float Height;
	public float Temperature;
	public float Humidity;

	public TileAttributeComponent(TileAttributeComponent prototype)
	{
		Height = prototype.Height;
		Temperature = prototype.Temperature;
		Humidity = prototype.Humidity;
	}
}