using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RaymiMusic.Modelos;

namespace RaymiMusic.MVC.Services
{
    public class UsuarioApiService : IUsuarioApiService
    {
        private readonly HttpClient _httpClient;

        public UsuarioApiService(IHttpClientFactory httpFactory)
        {
            _httpClient = httpFactory.CreateClient("RaymiMusicApi");
        }

        public async Task<Usuario?> AutenticarUsuarioAsync(string correo, string contrasena)
        {
            var loginData = new { Correo = correo, Contrasena = contrasena };
            var response = await _httpClient.PostAsJsonAsync("api/usuarios/autenticar", loginData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Usuario>();
            }

            return null;  // Si la autenticación falla, retorna null
        }
        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Usuario>>("api/Usuarios")
                   ?? Array.Empty<Usuario>();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Usuario>($"api/Usuarios/{id}");
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            var resp = await _httpClient.PostAsJsonAsync("api/Usuarios", usuario);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<Usuario>()
                   ?? throw new ApplicationException("Error al crear usuario");
        }

        public async Task ActualizarAsync(Guid id, Usuario usuario)
        {
            var resp = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", usuario);
            resp.EnsureSuccessStatusCode();
        }

        public async Task EliminarAsync(Guid id)
        {
            var resp = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
            resp.EnsureSuccessStatusCode();
        }

    }
}
