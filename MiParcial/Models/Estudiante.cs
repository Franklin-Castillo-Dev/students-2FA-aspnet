using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiParcial.Models;

public partial class Estudiante
{
    public int EstudianteId { get; set; }

    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    public string? Nombre { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();

    public virtual User? User { get; set; }
}
