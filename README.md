# API Policy - Sistema de Gestión de Pólizas de Seguro

API REST desarrollada con **.NET 10** para la gestión de pólizas de seguro y siniestros.

## 🚀 Características

- **Gestión de Pólizas**: Creación, consulta y cotización de pólizas de seguro
- **Cálculo de Primas**: Sistema automático de cálculo de primas mensuales basado en cobertura
- **Base de Datos SQLite**: Almacenamiento ligero y portable
- **Documentación Swagger**: UI interactiva para pruebas de API
- **CORS Configurado**: Listo para integrarse con frontends Vue/Quasar

## 📋 Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Editor de código (VS Code, Visual Studio, Rider)

## 🔧 Instalación

1. **Clonar el repositorio**
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd API-POLICY
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   cd Api-Policy
   dotnet restore
   ```

3. **Aplicar migraciones de base de datos**
   ```bash
   dotnet ef database update
   ```
   
   Si no tienes Entity Framework CLI instalado:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

   La API estará disponible en: `http://localhost:5250`

## 📚 Documentación de API

### Swagger UI

Una vez que la aplicación esté ejecutándose, accede a la documentación interactiva en:

```
http://localhost:5250
```

### Endpoints Disponibles

#### **GET** `/api/policies`
Obtiene la lista de todas las pólizas.

**Respuesta:**
```json
[
  {
    "id": 1,
    "policyNumber": "POL-A1B2C3D4",
    "clientName": "Juan Pérez",
    "monthlyPremium": 60.00,
    "status": "Active"
  }
]
```

#### **POST** `/api/policies`
Crea una nueva póliza con cotización automática.

**Request Body:**
```json
{
  "clientName": "María García",
  "coverageAmount": 10000,
  "monthsDuration": 12
}
```

**Respuesta (201 Created):**
```json
{
  "id": 2,
  "policyNumber": "POL-E5F6G7H8",
  "clientName": "María García",
  "monthlyPremium": 60.00,
  "status": "Active"
}
```

**Validaciones:**
- Cobertura mínima: $1,000
- El número de póliza se genera automáticamente

## 🏗️ Arquitectura del Proyecto

```
Api-Policy/
├── Controllers/
│   └── PoliciesController.cs    # Endpoints de la API
├── Data/
│   └── AppDbContext.cs          # Configuración EF Core
├── Models/
│   ├── Policy.cs                # Entidad principal
│   ├── Claims.cs                # Entidad de siniestros
│   └── Dtos/
│       └── PolicyDto.cs         # DTOs de transferencia
├── Services/
│   └── IPricingService.cs       # Lógica de cálculo de primas
├── Migrations/                  # Migraciones de base de datos
├── Program.cs                   # Configuración principal
└── appsettings.json            # Configuración de la aplicación
```

## 💰 Lógica de Negocio

### Cálculo de Prima Mensual

La prima se calcula con la siguiente fórmula:

```
Prima Mensual = (Monto de Cobertura × 0.5%) + $10
```

**Ejemplo:**
- Cobertura: $10,000
- Prima: ($10,000 × 0.005) + $10 = **$60/mes**

### Estados de Póliza

- **Draft**: Borrador (pendiente de activación)
- **Active**: Activa y vigente
- **Expired**: Expirada
- **Cancelled**: Cancelada

## 🌐 Configuración CORS

La API está configurada para aceptar peticiones desde:

- `http://localhost:9000` - Quasar Framework

Para agregar más orígenes, edita la configuración en [Program.cs](Api-Policy/Program.cs):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(
            "http://localhost:PUERTO_AQUI"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
```

## 🗄️ Base de Datos

### SQLite

La aplicación usa **SQLite** como base de datos, almacenada en:
```
Api-Policy/api-policy.db
```

### Migraciones

**Crear una nueva migración:**
```bash
dotnet ef migrations add NombreMigracion
```

**Aplicar migraciones:**
```bash
dotnet ef database update
```

**Revertir última migración:**
```bash
dotnet ef migrations remove
```

## 🛠️ Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET | 10.0 | Framework principal |
| Entity Framework Core | 10.0.1 | ORM para base de datos |
| SQLite | 10.0.1 | Base de datos |
| Swashbuckle | 10.1.0 | Documentación Swagger/OpenAPI |
| ASP.NET Core | 10.0 | Framework web |

## 🔍 Pruebas

### Usando Swagger UI

1. Navega a `http://localhost:5250`
2. Expande el endpoint que deseas probar
3. Haz clic en "Try it out"
4. Completa los parámetros necesarios
5. Ejecuta la petición

### Usando cURL

**Obtener pólizas:**
```bash
curl -X GET http://localhost:5250/api/policies
```

**Crear póliza:**
```bash
curl -X POST http://localhost:5250/api/policies \
  -H "Content-Type: application/json" \
  -d '{
    "clientName": "Pedro Sánchez",
    "coverageAmount": 15000,
    "monthsDuration": 24
  }'
```

## 🚧 Desarrollo

### Modo de Desarrollo

Para ejecutar en modo de desarrollo con hot reload:

```bash
dotnet watch run
```

### Desactivar HTTPS (Desarrollo)

Por defecto, la redirección HTTPS está comentada en modo desarrollo. Para habilitarla, descomentar en [Program.cs](Api-Policy/Program.cs):

```csharp
app.UseHttpsRedirection();
```

## 📝 Próximas Funcionalidades

- [ ] Autenticación y autorización JWT
- [ ] Gestión completa de siniestros (Claims)
- [ ] Paginación en endpoints de listado
- [ ] Filtros y búsqueda avanzada
- [ ] Reportes y estadísticas
- [ ] Integración con pasarelas de pago
- [ ] Notificaciones por email
- [ ] Auditoría de cambios

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Haz fork del proyecto
2. Crea una rama para tu feature (`git checkout -b feature/NuevaCaracteristica`)
3. Commit tus cambios (`git commit -m 'Agregar nueva característica'`)
4. Push a la rama (`git push origin feature/NuevaCaracteristica`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 📧 Contacto

Para preguntas o sugerencias, por favor abre un issue en el repositorio.

---

⭐ Si te ha sido útil este proyecto, considera darle una estrella en GitHub!
