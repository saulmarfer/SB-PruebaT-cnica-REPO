namespace SB.PruebaTecnica.Application.Exceptions
{
    public class ValidationAppException : Exception
    {
        public IReadOnlyList<string> Errores { get; }

        public ValidationAppException(string mensaje) : base(mensaje)
        {
            Errores = new List<string> { mensaje };
        }

        public ValidationAppException(IEnumerable<string> errores)
            : base(string.Join(" | ", errores))
        {
            Errores = errores.ToList();
        }
    }
}
