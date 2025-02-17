HotelBooking API 🚀
Prueba técnica para la gestión de reservas de hotel en .NET 6 con Entity Framework Core, JWT Authentication y Clean Architecture.

📌 Tecnologías Utilizadas
C# .NET 6
Entity Framework Core
SQL Server
Swagger para documentación
JWT para autenticación
DDD + Clean Architecture
SendGrid para envío de correos
📂 Estructura del Proyecto
plaintext
Copiar
Editar
📦 HotelBooking
 ┣ 📂 HotelBooking.API             # Capa de presentación (Controllers, Middlewares)
 ┣ 📂 HotelBooking.Application     # Casos de uso, servicios, lógica de negocio
 ┣ 📂 HotelBooking.Domain          # Entidades, interfaces, excepciones
 ┣ 📂 HotelBooking.Infrastructure  # Repositorios, contexto de base de datos
 ┣ 📜 README.md                    # Documentación
 ┗ 📜 HotelBooking.sln             # Archivo de solución de Visual Studio
🛠️ Instalación y Configuración
1️⃣ Clonar el repositorio
bash
Copiar
Editar
git clone [URL_DEL_REPO]
cd HotelBooking
2️⃣ Configurar Base de Datos
Asegúrate de tener SQL Server en ejecución.
Configura la cadena de conexión en appsettings.json:
json
Copiar
Editar
"ConnectionStrings": {
   "DefaultConnection": "Server=localhost;Database=HotelBookingDB;User Id=sa;Password=your_password;"
}
3️⃣ Aplicar Migraciones
bash
Copiar
Editar
dotnet ef database update
4️⃣ Iniciar el Proyecto
bash
Copiar
Editar
dotnet run
La API estará disponible en http://localhost:7174/swagger/index.html.

🛡️ Autenticación con JWT
Para acceder a los endpoints protegidos:

Inicia sesión en /api/auth/login con:
json
Copiar
Editar
{
  "username": "admin",
  "password": "admin123"
}
Copia el token JWT y agrégalo en Swagger con el botón “Authorize”.
📖 Endpoints Principales
🔑 Autenticación
POST /api/auth/login → Devuelve un JWT válido.
🧑 Clientes
GET /api/clients → Obtiene todos los clientes.
POST /api/clients → Crea un nuevo cliente.
PUT /api/clients/{id} → Modifica un cliente existente.
DELETE /api/clients/{id} → Elimina un cliente.
🏨 Hoteles
GET /api/hotels/search → Buscar hoteles disponibles.
GET /api/hotels/{id} → Obtener detalles de un hotel.
POST /api/hotels → Crear un nuevo hotel.
🛏️ Reservas
POST /api/reservations → Crear una reserva.
GET /api/reservations/{id} → Obtener detalles de una reserva.
PATCH /api/reservations/{id}/cancel → Cancelar una reserva.
📬 Notificaciones
La aplicación envía correos de confirmación de reserva usando SendGrid.
Para configurarlo, actualiza el appsettings.json con tus credenciales:

json
Copiar
Editar
"SendGrid": {
   "ApiKey": "your_sendgrid_api_key",
   "FromEmail": "noreply@hotelbooking.com"
}
📌 Notas Finales
DDD & Clean Architecture: Se ha seguido una estructura modular para mantener un código limpio y escalable.
Entity Framework Core: Se utiliza como ORM para interactuar con SQL Server.
JWT: Seguridad mediante autenticación basada en tokens.
📩 Contacto
Si tienes preguntas o sugerencias, contáctame en:
✉️ rdfajardos@gmail.com
