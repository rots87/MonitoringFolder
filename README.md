# 🧬 Phantera LIS Middleware

![Status](https://img.shields.io/badge/Status-Production%20Ready-success)
![Platform](https://img.shields.io/badge/Platform-Windows%20Forms%20(.NET)-blue)
![Language](https://img.shields.io/badge/Language-C%23-green)
![Database](https://img.shields.io/badge/Database-SQL%20Server-lightgrey)

Interfaz de automatización (Middleware) desarrollada en C# para la captura, procesamiento y transmisión de resultados moleculares desde equipos **Phantera** hacia el Sistema de Información de Laboratorio (LIS).

## 📋 Descripción del Proyecto

Este software resuelve la necesidad de interoperabilidad entre el equipo de PCR en tiempo real Phantera y la base de datos central del laboratorio. Elimina la transcripción manual de resultados, reduciendo el error humano y acelerando la entrega de diagnósticos críticos (GBS).

El sistema implementa el patrón **Hot Folder**, monitoreando en tiempo real la generación de archivos de resultados (TSV), aplicando reglas de negocio complejas y actualizando la base de datos SQL Server mediante transacciones seguras.

## 🚀 Características Principales

* **Monitoreo en Tiempo Real:** Uso de `FileSystemWatcher` gestionado en hilos independientes (Background Threads) para detectar nuevos archivos TSV al instante sin congelar la UI.
* **Lectura Inteligente (Dynamic Parsing):** Algoritmo capaz de leer archivos TSV mapeando columnas por nombre dinámicamente, tolerando cambios en el orden de las columnas del archivo origen.
* **Lógica de Negocio Clínica:**
    * Filtrado automático de muestras (Ignora Controles, Calibradores y canales internos como IC).
    * Interpretación de resultados (Ct < 40 = Positivo, 'nc' = Negativo).
* **Gestión de Archivos:** Organización automática de archivos procesados en carpetas de `Procesados` y `Errores` (Auditoría de archivos).
* **Resiliencia:** Sistema de recuperación ante fallos. Al iniciar, procesa el *backlog* (archivos pendientes) antes de activar la vigilancia en tiempo real.
* **Logging Profesional:** Implementación de **Serilog** para trazabilidad completa de eventos, errores y advertencias.

## 🛠️ Arquitectura y Tecnologías

El proyecto sigue una arquitectura modular con **Separación de Responsabilidades**:

* **Lenguaje:** C# (.NET Windows Forms).
* **Persistencia:** SQL Server (ADO.NET con `SqlParameter` para prevenir SQL Injection).
* **Configuración:** Gestión de entorno mediante archivos JSON (`AppConfigModel`), permitiendo cambios de servidor o rutas sin recompilar.
* **Patrones de Diseño:**
    * **Repository Pattern:** (`PhanteraRepository`) Para abstraer la lógica de base de datos.
    * **Observer Pattern:** Uso de Eventos y Delegados para la comunicación segura entre el hilo de procesamiento y la Interfaz Gráfica (Thread-Safe UI Updates).
    * **Singleton (Config):** Carga única de la configuración al inicio.

## ⚙️ Flujo de Datos

1.  **Detección:** El equipo Phantera deposita un archivo `.tsv` en la carpeta compartida.
2.  **Validación:** El sistema valida la integridad del archivo y la presencia de columnas críticas (`SampleID`, `Channel`, `LR_Ct`).
3.  **Procesamiento:**
    * Se extrae el `SampleID` (Código de barras).
    * Se busca la Orden correspondiente en la tabla `Ordenes` del LIS.
4.  **Transacción SQL:** Si la orden existe, se actualiza la tabla `Laboratorios` insertando el resultado interpretado, fecha de modificación y estado.
5.  **Disposición:** El archivo se mueve a la carpeta `Procesados` (si fue exitoso) o `Errores` (si hubo fallos de validación).

## 📸 Capturas de Pantalla

*(Aquí puedes agregar una captura de tu interfaz funcionando)*

## 📦 Instalación y Configuración

1.  Clonar el repositorio.
2.  Abrir la solución en Visual Studio.
3.  Configurar el archivo `config.json` o usar el menú de Configuración en la UI:
    * **Ruta CSV/TSV:** Carpeta donde el equipo guarda los archivos.
    * **SQL Connection:** Credenciales de la base de datos.
    * **ID Prueba:** Código interno del examen (ej. 2219).
4.  Compilar y Ejecutar.

## ⚖️ Licencia

Este proyecto es software libre: puedes redistribuirlo y/o modificarlo bajo los términos de la **GNU General Public License** publicada por la Free Software Foundation, ya sea la versión 3 de la Licencia, o (a tu elección) cualquier versión posterior.

Consulta el archivo [LICENSE](LICENSE) para más detalles.

### Resumen de derechos y obligaciones (GPLv3):
* ✅ **Permisos:** Uso comercial, modificación, distribución y uso de patentes.
* 🔄 **Condición Clave (Copyleft):** Si distribuyes este software o una versión modificada, **debes liberar el código fuente** bajo la misma licencia (GPLv3). No puedes cerrar el código.
* ℹ️ **Aviso:** Debes mantener los avisos de derechos de autor y licencia.
* 🛡️ **Sin Garantía:** El software se entrega "tal cual".

---
*Desarrollado por [Nestor Cañas / Rots87](https://github.com/rots87)*
