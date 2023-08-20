using ByTheSword.Scripts.Utilities;
using Godot;

namespace ByTheSword.Scripts.Controllers
{
    public class MovementController
    {
        public Vector2 StepRight(Vector2 position)
        {
            position.X += Constants.MOVEMENT_STEP;
            return position;
        }

        public Vector2 StepLeft(Vector2 position)
        {
            position.X -= Constants.MOVEMENT_STEP;
            return position;
        }

        public Vector2 StepUp(Vector2 position)
        {
            position.Y -= Constants.MOVEMENT_STEP;
            return position;
        }

        public Vector2 StepDown(Vector2 position)
        {
            position.Y += Constants.MOVEMENT_STEP;
            return position;
        }
    }
}
