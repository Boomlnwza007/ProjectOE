
public interface IEnergy
{
    public int energy { get; set; }
    public int maxEnergt { get; set; }
    public void UseEnergy(int amount)
    {
        if (energy > amount)
        {
            energy -= amount;
        }
    }

    public void GetEnergy(int amount)
    {
        energy += amount;
        if (energy > maxEnergt)
        {
            energy = maxEnergt;
        }
    }
}

