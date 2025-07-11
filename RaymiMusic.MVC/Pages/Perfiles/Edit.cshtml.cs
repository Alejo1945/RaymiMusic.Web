using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Pages.Perfiles
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Perfil Perfil { get; set; } = null!;

        [BindProperty]
        public IFormFile? Foto { get; set; }

        [BindProperty]
        public string? NombreArtistico { get; set; }

        [BindProperty]
        public string? Biografia { get; set; }

        public string Rol { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = HttpContext.Session.GetString("UsuarioId");
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
                return RedirectToPage("/Cuenta/Login");

            var usuario = await _context.Usuarios
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
                return NotFound();

            Rol = usuario.Rol;

            Perfil = usuario.Perfil ?? new Perfil { UsuarioId = userId };

            if (Rol == "Artista")
            {
                var artista = await _context.Artistas.FirstOrDefaultAsync(a => a.Id == userId);
                if (artista != null)
                {
                    NombreArtistico = artista.NombreArtistico;
                    Biografia = artista.Biografia;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdString = HttpContext.Session.GetString("UsuarioId");
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
                return RedirectToPage("/Cuenta/Login");

            var usuario = await _context.Usuarios
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
                return NotFound();

            Rol = usuario.Rol;
            ModelState.Remove("Perfil.Usuario");
            if (!ModelState.IsValid)
            {
                // Imprimir errores de ModelState en la consola (o en logs si usas ILogger)
                foreach (var entry in ModelState)
                {
                    var field = entry.Key;
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"[ModelState Error] Campo: {field} - Error: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            if (usuario.Perfil == null)
            {
                usuario.Perfil = new Perfil
                {
                    UsuarioId = userId
                };
                _context.Perfiles.Add(usuario.Perfil);
            }

            usuario.Perfil.NombreCompleto = Perfil.NombreCompleto;

            // ? Guardar la foto si fue cargada
            if (Foto != null)
            {
                var fileName = $"{Guid.NewGuid()}_{Foto.FileName}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Foto.CopyToAsync(stream);

                usuario.Perfil.UrlFoto = "/uploads/" + fileName;
            }

            _context.Usuarios.Update(usuario);

            // ? Solo para artistas
            if (Rol == "Artista")
            {
                var artista = await _context.Artistas.FirstOrDefaultAsync(a => a.Id == userId);
                if (artista != null)
                {
                    artista.NombreArtistico = NombreArtistico ?? "";
                    artista.Biografia = Biografia ?? "";
                    _context.Artistas.Update(artista);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Perfiles/Perfiles");
        }
    }
}