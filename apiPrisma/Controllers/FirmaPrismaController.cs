using apiPrisma.Clases;
using apiPrisma.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiPrisma.Controllers
{
    //[Authorize]
    //[Route("api/v1/[controller]")]
    [Route("api/v1/firma_prisma")]
    [ApiController]

    public class FirmaPrismaController : ControllerBase
    {
        private readonly FirmaPrismaService _firmaPrismaService;


        public FirmaPrismaController(FirmaPrismaService firmaPrismaService)
        {
            _firmaPrismaService = firmaPrismaService;
        }

        [Authorize]
        [HttpPost]
        [Route("firmar_documentos")]
        public async Task<ActionResult<Respuesta>> Firmar_documento(Peticion parPeticion)
        {
            Respuesta resp = await _firmaPrismaService.FirmarDocumento(parPeticion);
            return Ok(resp);
        }

        [HttpPost]
        [Route("solicitud_tokens")]
        public async Task<ActionResult<RetToken>> Sol_token(UserLogin userLogin)
        {
            RetToken ret = new RetToken();
            try
            {                
                ret.token = await _firmaPrismaService.GetToken(userLogin);
                ret.generado = 1;
                ret.mensaje = "Token generado correctamente.";
                return ret;
            }
            catch (Exception ex) 
            {
                ret.generado = 0;
                ret.mensaje = ex.Message;
                return ret;
            }            
        }

        [HttpGet]
        [Route("hola_mundo")]
        public async Task<ActionResult<string>> Hola_mundo()
        {
            return "Hola, estoy vivo";
        }

    }
}
