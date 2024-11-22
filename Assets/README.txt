# Generador Procedural de Mapas 3D

Este proyecto es un generador procedural de mapas en 3D desarrollado en Unity. Genera mapas dinámicamente a medida que el jugador explora, ofreciendo posibilidades infinitas para la creación de terrenos y caminos.

## Cómo Ejecutar el Proyecto

1. **Abrir la Escena:**
   - Abre la escena `ProceduralScene` en Unity.

2. **Ajustar Parámetros del Mapa:**
   - Selecciona el objeto `MapGenerator` en la jerarquía.
   - Ajusta los parámetros del mapa (ancho, alto, espaciado, número de módulos, etc.) desde el inspector según tus necesidades.

3. **Probar el Juego:**
   - Presiona el botón **Play** en el editor de Unity.
   - Usa las teclas **WASD** para moverte por la escena y observa cómo el mapa se genera dinámicamente a tu alrededor.

## Características Principales

- **Generación Procedural:**
  - Genera terrenos y caminos dinámicamente utilizando módulos.
  - Combina mallas para optimizar el rendimiento.

- **Movimiento Suave de la Cámara:**
  - Navega por el mapa utilizando el script `CameraController` para una exploración fluida.

- **Sistema de Caminos y Módulos:**
  - Un enfoque modular garantiza una generación eficiente y expansiva del mapa.

- **Optimización de Rendimiento:**
  - Utiliza un sistema de "Object Pooling" y combinación de mallas para mantener altos índices de fotogramas.

## Descripción de Scripts

- **CameraController.cs:**
  - Gestiona el movimiento suave de la cámara utilizando las teclas 

