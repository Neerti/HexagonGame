using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Components;

public struct Camera
{
	public Vector3 FuturePosition;
	public float Radius;
	public float AzimuthAngle;
	public float PolarAngle;
	public Vector3 ViewPosition;
}