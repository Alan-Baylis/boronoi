using System;

[Flags]
public enum ObjectProp
{
    Water = 1,
    Land = 2,
    ShallowWater = 4,
    Shore = 8,
    River = 16
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
