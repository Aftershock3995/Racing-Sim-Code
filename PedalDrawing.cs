using RacingSimPedals;

namespace RacingSimPedals
{
    public class PedalDrawingForm
    {
        public static float desiredX1 = 1f;
        public static float desiredX2 = 1f;
        public static float desiredX3 = 1f;

        public static void IncrementDesiredX(float newPosition)
        {
            desiredX1 = newPosition;
            desiredX2 = newPosition;
            desiredX3 = newPosition;

            if (desiredX1 > 200f)
            {
                desiredX1 = 1f;
            }

            else if (desiredX2 > 200f)
            {
                desiredX2 = 1f;
            }

            else if (desiredX3 > 200f)
            {
                desiredX3 = 1f;
            }
        }
    }
}
