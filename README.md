# Proyecto ASP.NET Core + Blazor con Patrón Unit of Work

Este proyecto es una solución modular desarrollada con **ASP.NET Core** para el backend y **Blazor Server** para el frontend. Se implementa el patrón **Unit of Work** para manejar las operaciones de acceso a datos en **SQLite**.

## 🚀 Instrucciones para ejecutar localmente

### Backend (`taskmanager.api`)

1. Abre **Visual Studio Code**.
2. Abre la carpeta `Absence.api` que contiene el archivo de solución (`.sln`).
3. Presiona `F5` o usa el botón **Run** para ejecutar la API.
4. La API estará disponible en:  
   👉 `https://localhost:44328/` (Dependerá del local)

> El backend utiliza **almacenamiento en memoria**, por lo que los datos no se persisten al reiniciar la aplicación.

---

### Frontend (`taskmanager.web`)

1. En la misma ventana o en otra de **Visual Studio Code**, abre la carpeta `taskmanager.web`.
2. Ejecuta la aplicación con `F5` o el botón **Run**.
3. El frontend estará disponible en:  
   👉 `https://localhost:44396/` (Dependerá del local, actualizar en appseting.json por url del backend)   

> El frontend está construido con **Blazor Server**.

---

## 🧱 Tecnologías utilizadas

- **ASP.NET Core 9**
- **Blazor WebAssembly**
- **Entity Framework Core**
- **Patrón Unit of Work**
- **Inyección de dependencias (DI)**
- **Swagger**

---

## Funcionalidades

### Administrador
- Puede **crear**, **modificar** o **eliminar** usuarios.
- Puede **ver todas las solicitudes**.
- Puede **aprobar o rechazar solicitudes**, pero **solo si están pendientes**.

### Usuario
- Puede **ver todas sus solicitudes**.
- Puede **crear nuevas solicitudes**, siempre que **no estén solapadas con otras existentes**.

### Consideraciones
- Las solicitudes solapadas serán rechazadas automáticamente o bloqueadas al momento de la creación.
- Se requiere autenticación para acceder a las funcionalidades del sistema.

---

## ✅ TODO

- [✓] Implementar la **interfaz de usuario** (componentes y páginas en Blazor).
- [✓] **Proteger rutas de vistas** según autenticación/autorización.
- [✓] Implementar los **servicios de comunicación con la API** desde el frontend.
- [✓] Crear e integrar **pruebas unitarias** para backend y frontend.

---

## 📌 Notas adicionales

- Asegúrate de que el backend esté corriendo antes de usar el frontend.
- Se utiliza SQLite.
- Si tienes errores con certificados HTTPS, puede que necesites confiar en el certificado de desarrollo generado por .NET.
  

Readme generado con IA y actualizado manualmente.
