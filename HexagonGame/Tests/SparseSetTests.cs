using System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.SparseSets;
using Xunit;

namespace HexagonGame.Tests;

public class SparseSetTests
{
	private const int MaxIndex = 10;
	private SparseSet<TestComponent> _sparseSet = new(MaxIndex, MaxIndex);


	[Fact]
	public void Add_Normal_ShouldBeAdded()
	{
		_sparseSet.Add(0, new TestComponent {Number = 0});
		Assert.Equal(1, _sparseSet.Count);
	}

	[Fact]
	public void Add_DuplicateIndex_ShouldBeIgnored()
	{
		var foo = new TestComponent {Number = 0};
		var bar = new TestComponent {Number = 1};
		_sparseSet.Add(0, foo);
		_sparseSet.Add(0, bar);
		Assert.Equal(foo, _sparseSet.Get(0));
	}

	[Fact]
	public void Add_OutOfBoundsOver_ShouldThrowException()
	{
		Assert.Throws<ArgumentOutOfRangeException>(
			() => _sparseSet.Add(MaxIndex + 1, new TestComponent())
		);
	}

	[Fact]
	public void Add_OutOfBoundsUnder_ShouldThrowException()
	{
		Assert.Throws<ArgumentOutOfRangeException>(
			() => _sparseSet.Add(-1, new TestComponent()));
	}

	[Fact]
	public void Get_Normal_ShouldWork()
	{
		var foo = new TestComponent {Number = 0};
		var bar = new TestComponent {Number = 1};
		_sparseSet.Add(0, foo);
		_sparseSet.Add(1, bar);
		Assert.Equal(foo, _sparseSet.Get(0));
		Assert.Equal(bar, _sparseSet.Get(1));
	}

	[Fact]
	public void Get_SparseHoles_ShouldThrowException()
	{
		_sparseSet.Add(0, new TestComponent());
		Assert.Throws<ArgumentException>(
			() => _sparseSet.Get(1));
	}

	[Fact]
	public void Remove_Normal_ShouldRemove()
	{
		_sparseSet.Add(0, new TestComponent());
		Assert.True(_sparseSet.Contains(0));
		_sparseSet.Remove(0);
		Assert.False(_sparseSet.Contains(0));
	}

	[Fact]
	public void Remove_NotInSet_DoNothing()
	{
		_sparseSet.Add(0, new TestComponent());
		_sparseSet.Remove(1);
		Assert.True(_sparseSet.Contains(0));
	}

	[Fact]
	public void Enumerable_IterateDenseSet_OnlyIterateFilledIndices()
	{
		for (var i = 0; i < MaxIndex / 2; i++)
		{
			_sparseSet.Add(i, new TestComponent());
		}

		var total = 0;
		foreach (var component in _sparseSet)
		{
			total++;
		}

		Assert.True(total == MaxIndex / 2);
	}
}