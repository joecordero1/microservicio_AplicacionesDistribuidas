using Microservice_VolunTrack.Domain;
using Microservice_VolunTrack.Domain.Models;
using Microservice_VolunTrack.Infraestructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Microservice_VolunTrack.Application
{
    public class MessageProcessor
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IFormHandlerFactory _formHandlerFactory;
        private readonly ApiGatewayClient _apiGatewayClient;

        public MessageProcessor(
            IDatabaseManager databaseManager,
            IMessagePublisher messagePublisher,
            IFormHandlerFactory formHandlerFactory,
            ApiGatewayClient apiGatewayClient) // Cambio aquí, inyecta la instancia ya creada de ApiGatewayClient
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
            _formHandlerFactory = formHandlerFactory ?? throw new ArgumentNullException(nameof(formHandlerFactory));
            _apiGatewayClient = apiGatewayClient ?? throw new ArgumentNullException(nameof(apiGatewayClient));
        }

        public async Task ProcessMessage(string message)
        {
            try
            {
                var formType = message.Trim();
                // Comprobar si el tipo de formulario es "PrimerFormulario" y el ID de usuario es 2
                if (formType == "PrimerFormulario")
                {
                    var ano = 2024; // Suponiendo que el año a analizar es 2024

                    var inicio = new DateTime(ano, 1, 1);
                    var fin = new DateTime(ano, 12, 31);

                    Console.WriteLine($"Comparando mejoras anuales desde {inicio} hasta {fin}");

                    // Obtener intervenciones de la base de datos
                    var intervenciones = await _apiGatewayClient.GetAsync<List<Intervencion>>("intervenciones");

                    Console.WriteLine($"Intervenciones encontradas: {intervenciones.Count}");

                    var totalMejora = intervenciones.Sum(intervencion => intervencion.ResultadoDespues - intervencion.ResultadoAntes);

                    Console.WriteLine($"Total mejora calculada: {totalMejora}");

                    // Datos ficticios de la ciudad vecina
                    var datosCiudadVecina = new System.Collections.Generic.Dictionary<int, int>
                {
                    { 2020, 15 },
                    { 2021, 20 },
                    { 2022, 30 },
                    { 2023, 49 },
                    { 2024, 55 }
                };

                    var mejoraVecina = datosCiudadVecina.ContainsKey(ano) ? datosCiudadVecina[ano] : 100;

                    var porcentajeAlcanzado = (totalMejora / (double)mejoraVecina) * 100;
                    var faltanteParaAlcanzar = mejoraVecina - totalMejora;

                    Console.WriteLine($"Mejora vecina: {mejoraVecina}, Porcentaje alcanzado: {porcentajeAlcanzado}, Faltante para alcanzar: {faltanteParaAlcanzar}");

                    // Crear el mensaje a enviar a la cola
                    string messageToSend = $"{mejoraVecina},{porcentajeAlcanzado},{faltanteParaAlcanzar}";

                    // Enviar el mensaje a la cola
                    _messagePublisher.PublishMessage(messageToSend);
                }
                else
                {
                    // Manejar otros tipos de formularios o IDs de usuario
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el mensaje: {ex.Message}");
                // Manejo de errores adecuado
            }
        }
    }
}
