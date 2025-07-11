using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Modelos;
using RaymiMusic.Api.Data;

namespace RaymiMusic.MVC.Pages.Perfiles
{
    public class PerfilModel : PageModel
    {
        private readonly AppDbContext _context;

        public PerfilModel(AppDbContext context)
        {
            _context = context;
        }

        public Usuario? Usuario { get; set; }
        public Artista? Artista { get; set; }
        public string? Error { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Leer desde la sesión
            var userIdStr = HttpContext.Session.GetString("UsuarioId");

            // DEBUG opcional: imprimir claves de sesión
            Console.WriteLine("Sesión:");
            foreach (var key in HttpContext.Session.Keys)
            {
                Console.WriteLine($"- {key} = {HttpContext.Session.GetString(key)}");
            }

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                Error = "No estás autenticado.";
                return Page();
            }

            // Buscar usuario con su perfil
            Usuario = await _context.Usuarios
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (Usuario == null)
            {
                Error = "Usuario no encontrado.";
                return Page();
            }

            // Si es artista, cargar datos adicionales
            if (Usuario.Rol == "Artista")
            {
                Artista = await _context.Artistas.FirstOrDefaultAsync(a => a.Id == userId);
            }

            return Page();
        }
    }
}