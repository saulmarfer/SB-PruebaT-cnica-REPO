namespace SB.PruebaTecnica.Application.Exceptions
{
    public class UnauthorizedAppException : Exception
    {
        public UnauthorizedAppException(string message = "Credenciales inválidas.") : base(message) { }
    }
}
