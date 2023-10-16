namespace TeamGPT.Models;

public struct Persona
{
    public string? Background { get; set; } // e.g., "Engineer", "Artist", "Doctor"
    public List<string>? Skills { get; set; } // e.g., "Programming", "Drawing", "Surgery"
    public List<string>? KnowledgeDomains { get; set; } // e.g., "Machine Learning", "Renaissance Art", "Cardiology"
    public List<string>? Proclivities { get; set; } // e.g., "Analytical", "Creative", "Patient"
}