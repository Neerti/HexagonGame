using System;

namespace HexagonGame.ECS.Components;

public class LifecycleComponent
{

	public DateTime Birthday;
	public Ages CurrentAge; 

	public LifecycleComponent(DateTime newBirthday)
	{
		Birthday = newBirthday;
	}
	
	public enum Ages
	{
		Baby,
		Child,
		Adolescent,
		Adult,
		Elder
	}
}