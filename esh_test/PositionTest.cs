using NUnit.Framework;

using esh;

namespace esh_test;

public class DeltaTest
{
    [Test]
    public void Negation()
    {
        Assert.AreEqual(-new Position.Delta(1, -1), new Position.Delta(-1, 1));
    }
}

public class PositionTest
{
    [Test]
    public void Equality()
    {
        Assert.AreEqual(new Position(1, 2), new Position(1, 2));
        Assert.AreNotEqual(new Position(1, 2), new Position(2, 1));
    }

    [Test]
    public void CanAdd()
    {
        Assert.IsTrue(new Position(0, 0).CanAdd(new Position.Delta(1, 0)));
        Assert.IsFalse(new Position(0, 0).CanAdd(new Position.Delta(-1, 0)));
    }

    [Test]
    public void Add()
    {
        Assert.AreEqual(new Position(1, 2) + new Position.Delta(3, 4), new Position(4, 6));
    }

    [Test]
    public void Subtract()
    {
        Assert.AreEqual(new Position(3, 4) - new Position.Delta(2, 1), new Position(1, 3));
    }
}
