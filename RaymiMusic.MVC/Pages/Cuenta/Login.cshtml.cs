using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Pages.Cuenta
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Correo { get; set; } = string.Empty;

        [BindProperty]
        public string Contrasena { get; set; } = string.Empty;

        public string? ErrorMensaje { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Buscar usuario por correo
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == Correo);

            if (usuario == null)
            {
                ErrorMensaje = "Correo o contraseña incorrectos.";
                return Page();
            }

            // Verificar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(Contrasena, usuario.HashContrasena))
            {
                ErrorMensaje = "Correo o contraseña incorrectos.";
                return Page();
            }

            // Guardar datos en sesión
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("Correo", usuario.Correo);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            // Redirigir a la página de perfil
            return RedirectToPage("/Perfiles/Perfiles");
        }
    }
}