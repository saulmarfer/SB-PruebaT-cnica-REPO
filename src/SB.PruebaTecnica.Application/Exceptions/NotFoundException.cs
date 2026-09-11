namespace SB.PruebaTecnica.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string entidad, object clave)
            : base($"{entidad} con identificador '{clave}' no fue encontrado(a).") { }
    }
}
