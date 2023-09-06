using System;
using System.Collections.Generic;

namespace MiParcial.Models;

public partial class Profesore
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}
