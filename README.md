Aplicación móvil desarrollada con .NET MAUI y C# que implementa un sistema de inicio de sesión mediante una API REST. Permite autenticar usuarios, almacenar el token de acceso de forma segura, consultar el perfil y mantener la sesión iniciada.

Descripción del proyecto

Este proyecto corresponde a la práctica de la Unidad 7: Autenticación y seguridad.

El objetivo es aprender a consumir una API REST desde una aplicación móvil, utilizando solicitudes HTTP para autenticar usuarios y acceder a información protegida mediante un token de autenticación.

La aplicación utiliza DummyJSON como servicio externo para realizar las pruebas de inicio de sesión.

Funcionalidades

Inicio de sesión mediante usuario y contraseña.

Validación de campos vacíos.

Autenticación mediante una solicitud HTTP POST.

Obtención y almacenamiento del accessToken.

Almacenamiento seguro del token mediante SecureStorage.

Almacenamiento del nombre de usuario mediante Preferences.

Consulta del perfil mediante una solicitud HTTP GET.

Envío del token mediante el encabezado Authorization: Bearer.

Inicio de sesión automático cuando existe un token guardado.

Cierre de sesión y eliminación del token.

Manejo de errores de autenticación y conexión.

Tecnologías utilizadas

C#

.NET MAUI

Visual Studio

HttpClient

System.Text.Json

SecureStorage

Preferences

API REST

API utilizada

Inicio de sesión

POST https://dummyjson.com/auth/login

Consulta del perfil

GET https://dummyjson.com/auth/me


<img width="383" height="463" alt="Captura de pantalla 2026-11-26 234841" src="https://github.com/user-attachments/assets/928f7e43-70ba-4ad4-ba5a-ca8b12c39962" />
<img width="404" height="500" alt="Captura de pantalla 2026-11-26 235218" src="https://github.com/user-attachments/assets/d4800f25-2b47-45bb-a074-8852c26110dd" />
<img width="324" height="28" alt="Captura de pantalla 2026-11-26 234913" src="https://github.com/user-attachments/assets/d3e4a94b-7cdc-49c1-9177-8fc88231aa7b" />
