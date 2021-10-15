// Primary Author : Andreas Berzelius - anbe5918

using Entity.AI.Scarabs;
using Entity.AI.States;

public abstract class ScarabBaseState : AIBaseState
{
    protected Scarab S => AI as Scarab;
}