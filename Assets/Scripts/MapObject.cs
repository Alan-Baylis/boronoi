using System;

[Flags]
public enum ObjectProp
{
    Water = 1,
    Ocean = 2,
    Land = 4,
    ShallowWater = 8,
    Shore = 16,
    River = 32
}

public class MapObjectState
{
    private ObjectProp _state;

    public bool Has(ObjectProp value)
    {
        return (_state & value) == value;
    }

    public bool Is(ObjectProp value)
    {
        return _state == value;
    }

    public void Add(ObjectProp value)
    {
        _state |= value;
    }

    public void Remove(ObjectProp value)
    {
        _state &= ~value;
    }
}

public class MapObject
{
    public MapObjectState Props { get; private set; }

    public MapObject()
    {
        Props = new MapObjectState();
    }
}
