namespace Zen.Game.Model.Object
{
    public abstract class GroundObjectListener
    {
        public abstract void OnAdded(GroundObject groundObject);

        public abstract void OnRemoved(GroundObject groundObject);
    }
}