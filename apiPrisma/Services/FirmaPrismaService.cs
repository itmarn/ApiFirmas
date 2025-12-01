using apiPrisma.Clases;
using apiPrisma.Entities;
using apiPrisma.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
namespace apiPrisma.Services
{
    public class FirmaPrismaService
    {
        private readonly string? _urlApi;
        private readonly string? _secretKey;
        private readonly string? _Issuer;
        private readonly string? _Audience;

        private readonly ParametrosService _parametrosServices;
        private readonly UsuariosService _usuarioService;

        public FirmaPrismaService(ParametrosService parametrosService, UsuariosService usuariosService, IConfiguration configuration)
        {
            _parametrosServices = parametrosService;
            _usuarioService = usuariosService;
            _urlApi = configuration["MiAplicacion:APIURL"];
            _secretKey = configuration["MiAplicacion:SecretKey"];
            _Issuer = configuration["MiAplicacion:Issuer"];
            _Audience = configuration["MiAplicacion:Audience"];
        }

        public async Task<Respuesta> FirmarDocumento(Peticion pet) 
        {
            Respuesta resp = new Respuesta();
            try 
            {
                TB_PARAMETROS_SISTEMA pars = await _parametrosServices.buscar(pet.sistema);

                var client = new HttpClient();
                var url = _urlApi;

                using (var formData = new MultipartFormDataContent())
                {                
                    formData.Add(new StringContent(pars.LLAVE_ACCESO), "llave_acceso");
                    formData.Add(new StringContent(pars.EMAIL), "email");
                    formData.Add(new StringContent(pars.PASSWORD_FIRMA), "password_firma");
                    formData.Add(new StringContent("true"), "solicitud_firma[es_sincrono]");
                    formData.Add(new StringContent(pet.concepto), "solicitud_firma[concepto]");
                    formData.Add(new StringContent(pet.lugar), "solicitud_firma[lugar]");
                    formData.Add(new StringContent("PDF"), "solicitud_firma[tipo_solicitud]");
                    formData.Add(new StringContent("GRAPHIC_AND_DESCRIPTION"), "solicitud_firma[apariencia_firma]");
                    formData.Add(new StringContent(pet.rubrica64), "solicitud_firma[rubrica]");
                    //Archivo pdf
                    //byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    byte[] fileBytes = Convert.FromBase64String(pet.archivo64);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    var nombre_archivo = "archivo_firmado.pdf";
                    if (pet.path_firmado != null && pet.path_firmado != "")
                    {
                        if (Path.Exists(Path.GetDirectoryName(pet.path_firmado))) 
                        {
                            nombre_archivo = Path.GetFileName(pet.path_firmado);
                        }
                    }
                    formData.Add(fileContent, "solicitud_firma[archivo]",nombre_archivo);
                    //--Archivo pdf
                    formData.Add(new StringContent(""), "solicitud_firma[url_respuesta]");
                    formData.Add(new StringContent("false"), "solicitud_firma[cerrar]");
                    formData.Add(new StringContent(pet.pagina.ToString()), "solicitud_firma[coordenadas][][paginas]");
                    if (pet.coordenadas == null || pet.coordenadas == "")
                    {
                        formData.Add(new StringContent(pars.COORDENADAS), "solicitud_firma[coordenadas][][coordenadas]");
                    
                    }
                    else
                    {
                        formData.Add(new StringContent(pet.coordenadas), "solicitud_firma[coordenadas][][coordenadas]");
                    }                        
                    formData.Add(new StringContent(pars.FONT_SIZE), "solicitud_firma[font_size]");
                    formData.Add(new StringContent("false"), "solicitud_firma[show_dn]");
                    var response = await client.PostAsync(url, formData);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) {
                        string strResp = await response.Content.ReadAsStringAsync();
                        OutputJsonPris? outPris = JsonSerializer.Deserialize<OutputJsonPris>(strResp);
                        if (outPris.respuesta)
                        {
                            if (pet.path_firmado == null || pet.path_firmado == "")
                            {
                                resp.archivo64 = outPris.archivo;
                                resp.mensaje = "OK - El Path para devolver el archivo no se encontro, se devuelve en base 64";
                            }
                            else
                            {
                                if (Path.Exists(Path.GetDirectoryName(pet.path_firmado)))
                                {
                                    File.WriteAllBytes(pet.path_firmado, Convert.FromBase64String(outPris.archivo));
                                    resp.mensaje = "OK";
                                } 
                                else
                                {
                                    resp.archivo64 = outPris.archivo;
                                    resp.mensaje = "OK - El Path para devolver el archivo no se encontro, se devuelve en base 64";
                                }
                            }
                            resp.correcto = 1;
                            //LogWriter.Log(resp.mensaje + " Archivo Firmado Correctamente: " + pet.path_firmado);
                            LogWriter.Log(resp.mensaje + " Archivo Firmado Correctamente! ");
                        }
                        else
                        {
                            resp.correcto = 0;
                            resp.mensaje = "Error: " + outPris.descripcion;
                            LogWriter.Log(resp.mensaje);
                        }
                    }
                    else {                        
                        resp.correcto = 0;
                        resp.mensaje = "Error: " + await response.Content.ReadAsStringAsync(); ;
                        LogWriter.Log(resp.mensaje);
                    }                
                }
            }
            catch (Exception ex)
            {                
                resp.correcto = 0;
                resp.mensaje = ex.Message;
                LogWriter.Log("Error: " + resp.mensaje);
            }
            return resp;
        }

        public async Task<string> GetToken(UserLogin userLogin)
        {

            USUARIOS user = await _usuarioService.buscar(userLogin.usuario);

            if (user == null)
            {
                throw new Exception("ERROR DE AUTENTICACIÓN USUARIO INVALIDO!");
            }

            if (!SecretHasher.Verify(userLogin.clave, user.Password))
            {
                throw new Exception("ERROR DE AUTENTICACIÓN PASSWORD INVALIDO!");
            }

            string token = GenerateToken(user);
            return token;
        }

        private string GenerateToken(USUARIOS user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);
            var claims = new[]
            {
                new Claim("UserId", user.ID.ToString().Trim()),
                new Claim("UserName", user.NombreUsuario ?? ""),
                new Claim("Email",user.Email.Trim()),
                new Claim("Nit", user.Nit.Trim()),
                new Claim("Nombres", user.Nombres.ToUpper().Trim()),
                new Claim("Apellidos", user.Apellidos.ToUpper().Trim()),
                new Claim(ClaimTypes.Name, user.NombreUsuario ?? ""),

            };
            var payload = new JwtPayload(
              _Issuer,
              _Audience,
              claims,
              DateTime.Now,
              DateTime.UtcNow.AddHours(24));
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

