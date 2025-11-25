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
        public async Task<ActionResult<string>> Sol_token(UserLogin userLogin)
        {
            string token = "";
            token = await _firmaPrismaService.GetToken(userLogin);
            return token;
        }

    }
}
