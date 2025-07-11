namespace RaymiMusic.MVC.Models
{
    public class UsuarioViewModel
    {
        public Guid Id { get; set; }
        public string Correo { get; set; }

        public string? NombreCompleto { get; set; }
        public string? UrlFoto { get; set; }

        public string? Rol { get; set; }
        public string? NombrePlan { get; set; }
        public decimal? PrecioPlan { get; set; }

    }
}
