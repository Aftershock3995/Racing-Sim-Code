namespace RacingSimPedals
{
    public class PedalDrawingForm
    {
        public static float desiredX = 1f;

        public static void IncrementDesiredX()
        {
            desiredX += .5f;

            if (desiredX > 200f)
            {
                desiredX = 1f;
            }
        }
    }
}
