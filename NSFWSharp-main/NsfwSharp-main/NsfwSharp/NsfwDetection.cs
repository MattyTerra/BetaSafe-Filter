public class NsfwDetection
{
    public string Name { get; private set; }
    public double Confidence { get; private set; }

    public float RotAngle { get; private set; }

    public NsfwDetection(string name, double confidence, float rotangle=0)
    {
        Name = name;
        Confidence = confidence;
        RotAngle= rotangle;
    }
}