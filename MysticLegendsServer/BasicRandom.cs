namespace MysticLegendsServer;

public class BasicRandom : IRNG
{
    private readonly Random random;
    public BasicRandom()
    {
        random = new Random();
    }

    public int RandomNumber(int min, int max) => random.Next(min, max);

    public int RandomNumber(int max) => random.Next(max);

    public double RandomDecimal(double min, double max) => random.NextDouble() * (max - min) + min;

    public double RandomDecimal(double max) => random.NextDouble() * max;
}
