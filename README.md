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

Por implementar

> El frontend está construido con **Blazor Server**.

---

## 🏗️ Estado del Proyecto

- ✅ **Backend completo**: API funcional con capa de datos implementada utilizando Unit of Work y repositorios.
- ⚠️ **Frontend en desarrollo**: Aún no se ha implementado la interfaz de usuario, pero ya está configurado el `HttpClient` para comunicarse con la API.

## 🧱 Tecnologías utilizadas

- **ASP.NET Core 9**
- **Blazor WebAssembly**
- **Entity Framework Core**
- **Patrón Unit of Work**
- **Inyección de dependencias (DI)**
- **Swagger**

## ✅ TODO

- [ ] Implementar la **interfaz de usuario** (componentes y páginas en Blazor).
- [ ] **Proteger rutas de vistas** según autenticación/autorización.
- [ ] Implementar los **servicios de comunicación con la API** desde el frontend.
- [ ] Crear e integrar **pruebas unitarias** para backend y frontend.
