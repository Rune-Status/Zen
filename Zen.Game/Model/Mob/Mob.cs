using Zen.Game.Model.Mob.Action;

namespace Zen.Game.Model.Mob
{
    public abstract class Mob : IEntity
    {
        protected Mob(Position position)
        {
            Position = position;
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
        public int LastRegionId { get; set; }
        public Position Position { get; set; }
        public World World { get; set; }
        public Action<Mob> Action { get; set; }

        public void SetDirections(Direction firstDirection, Direction secondDirection)
        {
            FirstDirection = firstDirection;
            SecondDirection = secondDirection;

            if (secondDirection != Direction.None)
                MostRecentDirection = secondDirection;
            else if (firstDirection != Direction.None)
                MostRecentDirection = firstDirection;
        }

        public void PlayAnimation(Animation animation) => Animation = animation;
        public void PlaySpotAnimation(SpotAnimation spotAnimation) => SpotAnimation = spotAnimation;
        public void ResetId() => Id = 0;
        public bool IsAnimationUpdated() => Animation != null;
        public bool IsSpotAnimationUpdated() => SpotAnimation != null;

        public void StartAction(Action<Mob> action)
        {
            if (Action != null)
            {
                if (Action.Equals(action))
                    return;
                StopAction(true);
            }
            Action = action;
            World.Schedule(action);
        }

        public void StopAction(bool cancelMoving = false)
        {
            if (Action != null)
            {
                Action.Stop();
                Action = null;
            }
            PlayAnimation(new Animation(-1));
            PlaySpotAnimation(new SpotAnimation(-1));
            if (cancelMoving)
                WalkingQueue.Reset();
        }

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