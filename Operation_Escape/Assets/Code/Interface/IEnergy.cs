
public interface IEnergy
{
    public int ultimateEnergy { get; set; }
    public int maxUltimateEnergy { get; set; }

    public int energy { get; set; }
    public int maxEnergt { get; set; }

    public void UseEnergy(int amount)
    {
        if (energy >= amount)
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
            GetultimateEnergy(amount);
        }
    }

    public void UseUltimateEnergy(int amount)
    {
        if (ultimateEnergy >= amount)
        {
            ultimateEnergy -= amount;
        }
    }

    public void GetultimateEnergy(int amount)
    {
        ultimateEnergy += amount;
        if (ultimateEnergy > maxUltimateEnergy)
        {
            ultimateEnergy = maxUltimateEnergy;
        }
    }
}

