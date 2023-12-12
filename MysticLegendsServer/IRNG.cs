namespace MysticLegendsServer;

public interface IRNG
{
    public int RandomNumber(int min, int max);
    public int RandomNumber(int max);

    public double RandomDecimal(double min, double max);
    public double RandomDecimal(double max);
}
