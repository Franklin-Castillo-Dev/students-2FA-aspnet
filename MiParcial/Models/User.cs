using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiParcial.Models;

public partial class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo Correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo Correo debe tener un formato de dirección de correo electrónico válido.")]
    public string? Email { get; set; }

    public int? Edad { get; set; }

    public string? Permiso { get; set; }

    [Required(ErrorMessage = "El campo Password es obligatorio.")]
    public string? Password { get; set; }

    public string? TokenVerificacionCorreo { get; set; }

    public bool CorreoVerificado { get; set; }

    public string? Codigo2Fa { get; set; }
    
    public string? CodigoVerificacion { get; set; }

    public DateTime? CreacionCodigoVerificacion { get; set; }

    public bool Habilitar2Fa { get; set; }

    public DateTime? CreacionCodigo2Fa { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}
