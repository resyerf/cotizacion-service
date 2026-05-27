namespace Cotizacion.API.Models;

/// <summary>Respuesta estándar para errores de la API.</summary>
/// <param name="Message">Mensaje descriptivo del error.</param>
/// <param name="Errors">Lista de errores de validación (vacía si no aplica).</param>
public record ErrorResponse(string Message, string[] Errors);
