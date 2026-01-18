# 🧥 KalleWear – Backend API

Backend profesional de **KalleWear**, una plataforma web de **e-commerce de ropa urbana y juvenil**, desarrollada con una **arquitectura limpia, escalable y segura**, alineada a buenas prácticas utilizadas en entornos reales de producción.

Este repositorio contiene **exclusivamente el backend del sistema**, incluyendo la **API REST**, la **lógica de negocio** y la **interacción con la base de datos**, organizados de forma clara, desacoplada y mantenible.

---
 
## 🚀 Visión General

**KalleWear** es una plataforma de comercio electrónico enfocada en la venta de **ropa urbana** como:

- Chompas  
- Casacas  
- Polos  

Está dirigida principalmente a un público juvenil entre **13 y 30 años**, tanto hombres como mujeres.

Además de la experiencia de compra para el cliente final, el sistema incluye un **entorno administrativo completo**, que permite a la empresa gestionar productos, precios, descuentos y operaciones internas de forma segura y centralizada.

---

## 🎯 Objetivo del Backend

El backend de KalleWear tiene como objetivo principal:

- Proveer una **API REST robusta, segura y consistente**
- Centralizar y proteger la **lógica de negocio**
- Garantizar **rendimiento, escalabilidad y mantenibilidad**
- Facilitar la integración con cualquier frontend moderno
- Permitir crecimiento futuro sin reestructuración del sistema

📌 El frontend se mantiene en un repositorio independiente para asegurar **limpieza arquitectónica**, **responsabilidades claras** y **despliegues desacoplados**.

---
## 🛠 Stack Tecnológico

El backend de **KalleWear** está desarrollado sobre el ecosistema **.NET**, utilizando una arquitectura moderna, escalable y orientada a buenas prácticas de producción.

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Arquitectura en capas
- Clean Architecture
- FluentValidation
- AutoMapper
- JWT (Json Web Tokens)
- Hangfire (background jobs)
- Swagger / OpenAPI
- xUnit + Moq (testing)
- EPPlus (exportación de reportes)

---

## 📦 Dependencias Principales

### 🗄 Persistencia y Acceso a Datos
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.Data.SqlClient

### 🔐 Seguridad y Autenticación
- Microsoft.AspNetCore.Authentication.JwtBearer
- BCrypt.Net-Next

### 🔄 Mapeo, Validación y Serialización
- AutoMapper
- FluentValidation
- Newtonsoft.Json
- Microsoft.AspNetCore.Mvc.NewtonsoftJson
- Microsoft.AspNetCore.JsonPatch

### ⏱ Procesos en Segundo Plano
- Hangfire.AspNetCore
- Hangfire.SqlServer

### 📄 Documentación y Contratos de API
- Swashbuckle.AspNetCore (Swagger / OpenAPI)

### 🧪 Testing
- xUnit
- xUnit.runner.visualstudio
- Moq
- coverlet.collector
- Microsoft.NET.Test.Sdk

### 📊 Utilidades y Reportes
- EPPlus

---

## 🧩 Alcance del Sistema

Este backend es responsable de:

- Autenticación y autorización de usuarios
- Seguridad basada en JWT y control de permisos
- CRUD administrativo completo
- Gestión de productos, precios y descuentos
- Validaciones centralizadas
- Acceso a datos desacoplado
- Procesos transversales y tareas en segundo plano
- Respuestas estandarizadas para consumo del frontend

---

## 🏗 Arquitectura General

El backend está organizado en **dos proyectos principales**, siguiendo una **arquitectura en capas** y principios de **separación de responsabilidades**:

KallewearPaginaWeb-Backend  
│  
├── ApiRopa  
│   └── Capa de presentación y aplicación (Web API)  
│  
└── BibliotecaClass  
    └── Núcleo del dominio, lógica de negocio y persistencia  

Esta separación permite un sistema **más limpio, testeable y escalable**, donde cada proyecto cumple un rol claramente definido.

---

# 📦 BibliotecaClass

Biblioteca de clases centralizada que encapsula la **lógica de negocio**,  
el **modelo de dominio**, el **acceso a datos** y las **validaciones** del backend de **KalleWear**.

Está diseñada como una **capa independiente, reutilizable y desacoplada**, desarrollada en **.NET 8**, y actúa como el **núcleo del backend**, sobre el cual se construye la Web API (`ApiRopa`).

Su objetivo principal es garantizar un sistema **mantenible, escalable y consistente**, minimizando dependencias con la capa de presentación.

---

## 🎯 Propósito

`BibliotecaClass` concentra toda la lógica crítica del sistema y define las reglas que gobiernan el dominio del negocio.

Proporciona:

- Separación clara de responsabilidades
- Reutilización de lógica entre capas
- Consistencia del dominio
- Base sólida para escalabilidad y testing

---

### 🏗 Arquitectura Interna

La biblioteca sigue una **arquitectura en capas orientada al dominio**, basada en principios de **Clean Architecture** y **Domain-Oriented Design**.  
Cada componente tiene responsabilidades claras, separando la lógica de negocio de la infraestructura.

---

### 📁 Persistencia de Datos

Se encarga de gestionar el acceso a la base de datos mediante **Entity Framework Core**.

**Incluye:**

- `AppDbContext` centralizado
- `DbSet` para todas las entidades del dominio
- Configuración de relaciones, claves y restricciones
- Control de eliminaciones y protección de datos críticos

Este enfoque asegura integridad, reduce errores y facilita el mantenimiento de la base de datos.

---

### 📁 Entidades (Domain / Entities)

Representan el **modelo de datos del sistema**.

**Características:**

- Simples y claras
- Independientes de la capa de presentación
- Sin lógica de infraestructura ni dependencias externas

---

### 📁 DTOs (Data Transfer Objects)

Permiten la **comunicación entre capas** de forma segura y controlada.

**Convenciones:**

- `CreateDto` → creación de registros  
- `UpdateDto` → actualización de registros  
- `Dto` → lectura o salida de datos  

**Beneficios:**

- Mantiene la API desacoplada del dominio
- Controla qué datos se exponen
- Facilita cambios y mantiene la seguridad

---

### 📁 Validaciones

Las reglas de negocio se aplican mediante **FluentValidation**, separadas de la API y las entidades.

**Tipos de validaciones:**

- Crear, actualizar o eliminar registros  
- Validaciones parciales  
- Obtención por identificador  

Esto centraliza las reglas, facilita la reutilización y mejora el mantenimiento.

---

### 🧠 Principios de Diseño

- Separación de responsabilidades  
- Dominio independiente de la infraestructura  
- Persistencia centralizada  
- Validaciones externas a dominio y API  
- Preparado para testing, mantenimiento y escalabilidad

---

### 🔗 Relación con ApiRopa

`BibliotecaClass` es consumida por **ApiRopa**, que se encarga de:

- Exponer endpoints HTTP  
- Gestionar seguridad y permisos  
- Orquestar flujos de aplicación  

De este modo, **la API se mantiene ligera y enfocada**, mientras que toda la lógica de negocio se centraliza en la biblioteca.

---

# 🚀 ApiRopa

**ApiRopa** es la **Web API** del backend de **KalleWear**, desarrollada en **ASP.NET Core (.NET 8)**.  
Se encarga de recibir solicitudes HTTP, validar datos, aplicar seguridad y coordinar la lógica del negocio, que reside en **BibliotecaClass**.

Este proyecto sigue principios de **Clean Architecture**, enfocándose en **mantenibilidad, escalabilidad y separación de responsabilidades**.

---

## 🎯 Propósito

`ApiRopa` permite que aplicaciones web, móviles o integraciones externas interactúen con el backend de manera **segura y consistente**.

**Funciones principales:**

- Exponer endpoints RESTful
- Gestionar autenticación y autorización (JWT + permisos)
- Validar y transformar datos de entrada/salida
- Orquestar la lógica del negocio
- Ejecutar tareas en segundo plano
- Centralizar manejo de errores y respuestas estándar

---

## 🏗 Arquitectura

`ApiRopa` actúa como la **capa de presentación y aplicación**, mientras que toda la lógica de negocio y acceso a datos se encuentra en **BibliotecaClass**.  

Esto permite que la API sea:

- Ligera y fácil de mantener  
- Testeable  
- Escalable a futuro  

**Principios aplicados:**

- Clean Architecture y arquitectura en capas  
- Inyección de dependencias  
- Servicios desacoplados  
- Validaciones externas a los controladores  
- Seguridad declarativa  
- Infraestructura encapsulada
  
---

## 🗂 Estructura General del Proyecto

ApiRopa  
│  
├── Controllers  
│   └── Endpoints REST (entrada HTTP)  
│  
├── Services  
│   ├── Dominio  
│   ├── Hangfire  
│   ├── Help  
│   └── IServices  
│  
├── Repositories  
│   ├── Interfaces  
│   └── Implementaciones  
│  
├── Infraestructura  
│   └── Services  
│       ├── IServices  
│       │   └── IExcelService.cs  
│       └── ExcelService.cs  
│  
├── Mapping  
│   └── AutoMapper Profiles  
│  
├── Models  
│   ├── Dtos  
│   ├── Helpers  
│   └── Responses  
│       └── ApiResponse<T>  
│  
├── Security  
│   ├── JWT  
│   ├── Permisos  
│   └── Attributes  
│  
├── Program.cs  
└── appsettings.json  

---

## 🎮 Controllers

Los **Controllers** son la puerta de entrada HTTP del sistema.  
Se encargan de recibir solicitudes, validar datos, orquestar servicios y devolver respuestas estándar.

**Responsabilidades clave:**

- Exponer endpoints RESTful
- Validar y transformar solicitudes
- Coordinar la ejecución de servicios
- Gestionar códigos de respuesta HTTP
- Aplicar autorización por permisos

---

## 🧠 Services

La capa **Services** orquesta la lógica de negocio proveniente de **BibliotecaClass**.

**Responsabilidades clave:**

- Ejecutar reglas de negocio
- Coordinar repositorios y operaciones de datos
- Preparar información para la API
- Ejecutar procesos transversales

---

## 📊 ExcelService

Servicio especializado en generar reportes Excel de forma desacoplada y configurable.

**Características:**

- Generación de reportes genéricos (`GenerarExcel<T>`)
- Configuración dinámica de columnas y estilos
- Exportación como `byte[]` para descarga HTTP
- Reutilizable y testeable

---

## 🗄 Repositories

Encapsula el acceso a la base de datos usando **Entity Framework Core**.

**Características:**

- Repositorios por entidad
- Repositorio genérico para operaciones comunes
- `DbContext` centralizado
- Simplifica mantenimiento y escalabilidad

---

## 🧪 Validaciones

Se implementan con **FluentValidation**, desacopladas de controladores.

**Enfoque:**

- Validaciones por DTO
- Reglas reutilizables
- Separación por tipo de operación (Create, Update, Delete)
- Registro en `Program.cs`

---

## 🔐 Seguridad

Gestión centralizada y declarativa de autenticación y permisos.

**Componentes:**

- Autenticación JWT
- Autorización basada en permisos
- Atributos personalizados en endpoints

---

## 🏃‍♂️ Cómo Ejecutar el Backend de KalleWear

Sigue estos pasos para descargar, configurar y ejecutar el backend en tu máquina local.

---

### Requisitos

- Visual Studio 2022 (con workload **ASP.NET y desarrollo web**)  
- .NET 8 SDK  
- SQL Server (local o en Docker)  
- SSMS o Azure Data Studio para ejecutar scripts de base de datos  

---

### 1️⃣ Clonar, configurar y ejecutar

Abre una terminal y ejecuta todo seguido:

# Clonar el repositorio
git clone https://github.com/tu-usuario/KalleWearBackend.git
cd KalleWearBackend

# Ejecutar el script de base de datos desde /Model/KalleWear_DB.sql en SSMS o Azure Data Studio

# Entrar al proyecto de la API
cd ApiRopa

# Descargar todas las dependencias NuGet (bibliotecas y paquetes del proyecto)
dotnet restore

# Ejecutar la API
dotnet run




