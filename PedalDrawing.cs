using RacingSimPedals;

namespace RacingSimPedals
{
    public class PedalDrawingForm
    {
        public static float desiredX = 1f;

        public static void IncrementDesiredX(float newPosition)
        {
            desiredX = newPosition;

            if (desiredX > 200f)
            {
                desiredX = 1f;
            }
        }
    }
}
