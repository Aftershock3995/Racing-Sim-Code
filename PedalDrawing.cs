public static class PedalDrawingForm
{
    public static float desiredX1 = 1f;
    public static float desiredX2 = 1f;
    public static float desiredX3 = 1f;

    public static void IncrementDesiredX(float newPosition1, float newPosition2, float newPosition3)
    {
        desiredX1 = newPosition1;
        desiredX2 = newPosition2;
        desiredX3 = newPosition3;

        if (desiredX1 > 200f)
        {
            desiredX1 = 1f;
        }

        if (desiredX2 > 200f)
        {
            desiredX2 = 1f;
        }

        if (desiredX3 > 200f)
        {
            desiredX3 = 1f;
        }
    }
}
