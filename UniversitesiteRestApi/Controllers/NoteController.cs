using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Exceptions;
using System.Security.Claims;
using UniversiteDomain.UseCases;

namespace UniversiteRestApi.Controllers
{
    [Authorize(Roles = "Scolarite")]
    [Route("api/note")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly GenererCSVNotesUseCase _genererCSVNotesUseCase;
        private readonly UploadNotesUseCase _uploadNotesUseCase;

        public NoteController(
            GenererCSVNotesUseCase genererCSVNotesUseCase,
            UploadNotesUseCase uploadNotesUseCase)
        {
            _genererCSVNotesUseCase = genererCSVNotesUseCase;
            _uploadNotesUseCase = uploadNotesUseCase;
        }
        
        [HttpGet("csv/{ueId}")]
        public async Task<IActionResult> GenerateCsv(long ueId)
        {
            try
            {
                var csvContent = await _genererCSVNotesUseCase.ExecuteAsync(ueId);
                return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", $"UE_{ueId}_Notes.csv");
            }
            catch (CsvProcessingException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
        
        [HttpPost("upload/{ueId}")]
        public async Task<IActionResult> UploadCsv(long ueId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine(" Erreur: Le fichier est invalide ou vide.");
                return BadRequest(new { error = "Fichier invalide ou vide." });
            }

            try
            {
                Console.WriteLine($" Début du traitement du fichier CSV pour l'UE {ueId}");
                Console.WriteLine($" Nom du fichier : {file.FileName}, Taille : {file.Length} octets");

                using (var stream = file.OpenReadStream())
                {
                    await _uploadNotesUseCase.ExecuteAsync(stream, ueId);
                }

                Console.WriteLine(" Notes enregistrées avec succès !");
                return Ok(new { message = "Notes enregistrées avec succès." });
            }
            catch (CsvProcessingException e)
            {
                Console.WriteLine($" Erreur lors du traitement du fichier CSV: {e.Message}");
                return BadRequest(new { error = e.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erreur inconnue : {ex.Message}");
                return StatusCode(500, new { error = "Une erreur interne s'est produite." });
            }
        }

    }
}


