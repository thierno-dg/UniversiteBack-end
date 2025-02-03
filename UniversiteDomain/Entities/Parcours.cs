using UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = string.Empty;
    public int AnneeFormation { get; set; } = 1;

    // Supprimez ou ignorez cette propriété si elle est redondante
    // [NotMapped]
    // public List<Ue> Ues { get; set; }

    public List<Etudiant>? Inscrits { get; set; } = new();
    public List<Ue>? UesEnseignees { get; set; } = new();
}