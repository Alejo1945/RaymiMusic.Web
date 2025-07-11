using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Pages.Cuenta
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Correo { get; set; } = null!;

        [BindProperty]
        public string Contrasena { get; set; } = null!;

        [BindProperty]
        public string TipoCuenta { get; set; } = "Usuario";  // "Usuario" o "Artista"

        public string? ErrorMensaje { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Verifica si ya existe el correo
            if (await _context.Usuarios.AnyAsync(u => u.Correo == Correo))
            {
                ErrorMensaje = "Ya existe una cuenta con este correo.";
                return Page();
            }

            // 2. Define el rol
            string rol = TipoCuenta == "Artista" ? "Artista" : "Free";

            // 3. Crea el nuevo usuario con ID nuevo
            var nuevoUsuarioId = Guid.NewGuid();
            var nuevoUsuario = new Usuario
            {
                Id = nuevoUsuarioId,
                Correo = Correo,
                HashContrasena = BCrypt.Net.BCrypt.HashPassword(Contrasena),
                Rol = rol,
                PlanSuscripcionId = await _context.Planes
                    .Where(p => p.Nombre == "Free")
                    .Select(p => p.Id)
                    .FirstAsync()
            };

            // 4. Agrega usuario a la base de datos
            _context.Usuarios.Add(nuevoUsuario);

            // ? 5. Crear un perfil vacío relacionado con el usuario
            var nuevoPerfil = new Perfil
            {
                UsuarioId = nuevoUsuarioId,
                NombreCompleto = "",  // Se podrá editar después
                FechaNacimiento = null,
                UrlFoto = null
            };
            _context.Perfiles.Add(nuevoPerfil);

            // 6. Si es artista, agrega también a la tabla Artistas
            if (rol == "Artista")
            {
                _context.Artistas.Add(new Artista
                {
                    Id = Guid.NewGuid(),
                    NombreArtistico = Correo // o puedes dejarlo editable luego
                });
            }

            // 7. Guarda todos los cambios
            await _context.SaveChangesAsync();

            // 8. Redirige al login
            return RedirectToPage("/Cuenta/Login");
        }
    }
}
