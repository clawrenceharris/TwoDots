public class InputGate
{
    public bool Enabled { get; private set; } = true;
    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }
}