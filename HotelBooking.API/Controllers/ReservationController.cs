using HotelBooking.Application.Interfaces;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
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


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReservation([FromBody] Reservation reservation, [FromServices] EmailService emailService)
        {
          

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // ⬅️ Muestra errores de validación específicos
                }

                if (reservation == null) return BadRequest("Invalid reservation data");

                if (reservation.CheckOut <= reservation.CheckIn)
                {
                    return BadRequest("Check-out date must be after check-in date.");
                }



                var hotelExists = await _unitOfWork.Hotels.GetByIdAsync(reservation.HotelId);
                if (hotelExists == null) return NotFound("Hotel not found.");

                var roomExists = await _unitOfWork.Rooms.GetByIdAsync(reservation.RoomId);
                if (roomExists == null) return NotFound("Room not found.");

                var client = await _unitOfWork.Clients.GetByIdAsync(reservation.ClientId);
                if (client == null) return BadRequest("Client not found");

                if (string.IsNullOrEmpty(client.EmergencyContactName))
                {
                    return BadRequest("An emergency contact is required for reservations.");
                }

                var roomOccupied = await _unitOfWork.Reservations.AnyAsync(r =>
                                        r.RoomId == reservation.RoomId &&
                                        ((reservation.CheckIn >= r.CheckIn && reservation.CheckIn < r.CheckOut) ||
                                         (reservation.CheckOut > r.CheckIn && reservation.CheckOut <= r.CheckOut)));

                if (roomOccupied)
                {
                    return BadRequest("The selected room is already booked for the given dates.");
                }

                if (string.IsNullOrEmpty(client.FirstName) || string.IsNullOrEmpty(client.LastName) ||
                     string.IsNullOrEmpty(client.Email) || string.IsNullOrEmpty(client.DocumentNumber))
                {
                    return BadRequest("Guest information is incomplete. Please provide First Name, Last Name, Email, and Document Number.");
                }

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



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _unitOfWork.Reservations.GetAllAsync();
            return Ok(reservations);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
            return reservation == null ? NotFound() : Ok(reservation);
        }

        [Authorize]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id, [FromServices] EmailService emailService)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
            if (reservation == null) return NotFound("❌ No se encontró la reserva especificada.");

            reservation.Status = "Canceled";
            await _unitOfWork.Reservations.UpdateAsync(reservation);
            await _unitOfWork.CompleteAsync();

            // Obtener detalles del cliente y hotel
            var client = await _unitOfWork.Clients.GetByIdAsync(reservation.ClientId);
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(reservation.HotelId);

            if (client != null && hotel != null)
            {
                // 📩 **Mensaje de cancelación de reserva**
                string emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #C0392B;'>❌ Tu Reserva ha sido Cancelada</h2>
                    <p>Hola <strong>{client.FirstName} {client.LastName}</strong>,</p>
                    <p>Lamentamos informarte que tu reserva en <strong>{hotel.Name}</strong> ha sido <strong style='color: red;'>CANCELADA</strong>.</p>

                    <h3>📌 Detalles de la Reserva Cancelada:</h3>
                    <ul>
                        <li><strong>Hotel:</strong> {hotel.Name}</li>
                        <li><strong>Habitación:</strong> # {reservation.RoomId}</li>
                        <li><strong>Check-in Original:</strong> {reservation.CheckIn:dddd, dd MMMM yyyy} a las {reservation.CheckIn:HH:mm} hrs</li>
                        <li><strong>Check-out Original:</strong> {reservation.CheckOut:dddd, dd MMMM yyyy} a las {reservation.CheckOut:HH:mm} hrs</li>
                        <li><strong>Estado:</strong> <span style='color: red;'>CANCELADO</span></li>
                    </ul>

                    <p style='color: #D35400;'><strong>⚠ Si esta cancelación fue un error, por favor contáctanos lo antes posible al +123 456 789.</strong></p>

                    <p>Esperamos verte en otra ocasión. ¡Gracias por considerar <strong>Hotel Booking</strong>! 😊</p>

                    <hr>
                    <p style='font-size: 12px; color: #777;'>Este es un mensaje automático, por favor no respondas a este correo.</p>
                </body>
                </html>";

                await emailService.SendEmailAsync(client.Email, "❌ Cancelación de tu Reserva en Hotel Booking", emailBody);
            }

            return Ok(new { message = "✅ La reserva ha sido cancelada con éxito.", reservation });
        }

    }
}
