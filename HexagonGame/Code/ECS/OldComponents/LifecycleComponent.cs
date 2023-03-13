using System;

namespace HexagonGame.ECS.Components;

public struct LifecycleComponent
{

	public DateTime Birthday;
	public Ages CurrentAge; 

	public LifecycleComponent(DateTime newBirthday)
	{
		Birthday = newBirthday;
		CurrentAge = Ages.Undetermined;
	}
	
	public enum Ages
	{
		Undetermined,
		Baby,
		Child,
		Adolescent,
		Adult,
		Elderly
	}
}