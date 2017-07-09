using Newtonsoft.Json;

namespace Zen.Game.Model
{
    public abstract class Mob : Entity
    {
        protected Mob()
        {
            WalkingQueue = new WalkingQueue(this);
        }

        public int Id { get; set; }
        public bool Active => Id != 0;
        public Animation Animation { get; private set; }
        public SpotAnimation SpotAnimation { get; private set; }
        public Direction FirstDirection { get; private set; } = Direction.None;
        public Direction SecondDirection { get; private set; } = Direction.None;
        public Direction MostRecentDirection { get; private set; } = Direction.South;
        public bool Teleporting { get; private set; }
        public WalkingQueue WalkingQueue { get; }

        public void SetDirections(Direction firstDirection, Direction secondDirection)
        {
            FirstDirection = firstDirection;
            SecondDirection = secondDirection;

            if (secondDirection != Direction.None)
                MostRecentDirection = secondDirection;
            else if (firstDirection != Direction.None)
                MostRecentDirection = firstDirection;
        }

        public void PlayAnimation(Animation animation)
        {
            Animation = animation;
        }

        public void PlaySpotAnimation(SpotAnimation spotAnimation)
        {
            SpotAnimation = spotAnimation;
        }

        public void ResetId() => Id = 0;
        public bool IsAnimationUpdated() => Animation != null;
        public bool IsSpotAnimationUpdated() => SpotAnimation != null;

        public void Teleport(Position position)
        {
            Position = position;
            Teleporting = true;
            WalkingQueue.Reset();
        }

        public void Reset()
        {
            Animation = null;
            SpotAnimation = null;
            Teleporting = false;
            WalkingQueue.MinimapFlagReset = false;
        }
    }
}