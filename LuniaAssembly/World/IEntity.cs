using System;

namespace LuniaAssembly
{
    public interface IEntity
    {
        Guid Guid { get; set; }
        Position Position { get; set; } 
    }
}