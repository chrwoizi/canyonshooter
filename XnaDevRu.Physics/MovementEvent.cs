namespace XnaDevRu.Physics
{
    /// <summary>
    /// Stores details about movement event.
    /// </summary>
    public struct MovementEvent
    {
        public MovementEvent(Solid movedSolid)
        {
            Solid = movedSolid;
        }

        public Solid Solid;
    }

    public abstract class MovementEventProcessor : EventProcessor
    {
        public MovementEventProcessor() { }

        public abstract void HandleMovementEvent(MovementEvent move);

    }


}
