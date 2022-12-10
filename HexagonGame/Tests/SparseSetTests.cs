using HexagonGame.ECS.SparseSets;
using Xunit;

namespace HexagonGame.Tests;

public class SparseSetTests
{
	private SparseSet<string> _sparseSet = new SparseSet<string>(10);

	[Fact]
	public void Add_AddingElement_ShouldBeAdded()
	{
		_sparseSet.Add(0, "test");
		Assert.Equal(1, _sparseSet.Count);
	}
	
	[Fact]
	public void Add_AddingDuplicateIndex_ShouldBeIgnored()
	{
		_sparseSet.Add(0, "test");
		_sparseSet.Add(0, "another");
		Assert.Equal("test", _sparseSet.Elements[0]);
	}
}