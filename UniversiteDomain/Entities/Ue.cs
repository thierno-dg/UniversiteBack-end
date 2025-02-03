namespace UniversiteDomain.Entities;

public class Ue
{
    // Liste des notes associées à cette UE
    public List<Note> Notes { get; set; } = new();

    // Identifiant unique de l'UE
    public long Id { get; set; }

    // Numéro de l'UE
    public string NumeroUe { get; set; } = string.Empty;

    // Intitulé de l'UE
    public string Intitule { get; set; } = string.Empty;

    // Many-to-Many : une Ue est enseignée dans plusieurs parcours
    public List<Parcours>? EnseigneeDans { get; set; } = new();

    public override string ToString()
    {
        return "ID " + Id + " : " + NumeroUe + " - " + Intitule;
    }
}