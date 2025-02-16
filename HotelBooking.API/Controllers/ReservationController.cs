using HotelBooking.Application.Interfaces;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.API.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [HttpPost]
        public async Task<IActionResult> AddReservation([FromBody] Reservation reservation, [FromServices] EmailService emailService)
        {
            //if (reservation == null) return BadRequest("Invalid reservation data");

            //// Buscar el cliente asociado a la reserva
            //var client = await _unitOfWork.Clients.GetByIdAsync(reservation.ClientId);
            //if (client == null) return BadRequest("Client not found");

            //// Guardar la reserva en la base de datos
            //await _unitOfWork.Reservations.AddAsync(reservation);
            //await _unitOfWork.CompleteAsync();

            //// Enviar correo de confirmación al cliente
            //string emailBody = $"Hola {client.FirstName}, tu reserva en el hotel con ID {reservation.HotelId} ha sido confirmada.";
            //await emailService.SendEmailAsync(client.Email, "Confirmación de Reserva", emailBody);

            //return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // ⬅️ Muestra errores de validación específicos
                }

                if (reservation == null) return BadRequest("Invalid reservation data");

                //Buscar el cliente asociado a la reserva
                var client = await _unitOfWork.Clients.GetByIdAsync(reservation.ClientId);
                if (client == null) return BadRequest("Client not found");

                //Buscar el hotle asociado a la reserva
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(reservation.HotelId);
                if (hotel == null) return BadRequest("Hotel not found");




                //Guardar la reserva en la base de datos
                await _unitOfWork.Reservations.AddAsync(reservation);
                await _unitOfWork.CompleteAsync();

                //Enviar correo de confirmación al cliente
                string emailBody = $@"
                                <html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <h2 style='color: #2E86C1;'>¡Reserva Confirmada! 🎉</h2>
                                    <p>Hola <strong>{client.FirstName} {client.LastName}</strong>,</p>
                                    <p>Tu reserva ha sido confirmada con éxito en <strong>Hotel Booking</strong>.</p>

                                    <h3>📌 Detalles de la Reserva:</h3>
                                    <ul>
                                        <li><strong>Hotel:</strong>  {hotel.Name}</li>
                                        <li><strong>Habitación:</strong># {reservation.RoomId} </li>
                                        <li><strong>Check-in:</strong> {reservation.CheckIn:dddd, dd MMMM yyyy} a las {reservation.CheckIn:HH:mm} hrs</li>
                                        <li><strong>Check-out:</strong> {reservation.CheckOut:dddd, dd MMMM yyyy} a las {reservation.CheckOut:HH:mm} hrs</li>
                                        <li><strong>Estado:</strong> {reservation.Status}</li>
                                    </ul>

                                    <p style='color: green;'><strong>📞 Si tienes alguna consulta, contáctanos al +123 456 789 o responde a este correo.</strong></p>
        
                                    <p>¡Gracias por confiar en <strong>Hotel Booking</strong>! 😊</p>
        
                                    <hr>
                                    <p style='font-size: 12px; color: #777;'>Este es un mensaje automático, por favor no respondas a este correo.</p>
                                </body>
                                </html>";

                await emailService.SendEmailAsync(client.Email, "🎉 Confirmación de tu Reserva en Hotel Booking", emailBody);

                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
                //return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

       


        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _unitOfWork.Reservations.GetAllAsync();
            return Ok(reservations);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
            return reservation == null ? NotFound() : Ok(reservation);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            reservation.Status = "Canceled";
            await _unitOfWork.Reservations.UpdateAsync(reservation);
            await _unitOfWork.CompleteAsync();

            return Ok(reservation);
        }
    }
}
