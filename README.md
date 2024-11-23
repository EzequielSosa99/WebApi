# Parte 1: Base de Datos

## Primera pregunta

```sql
SELECT 
    m.serialNumber, 
    m.clientCode,
    COUNT(CASE WHEN rh.type = 'I' THEN 1 END) AS cantidad_tipo_I,
    COUNT(CASE WHEN rh.type = 'H' THEN 1 END) AS cantidad_tipo_H
FROM 
    meter m
LEFT JOIN 
    registerHeader rh ON m.meterId = rh.meterId
GROUP BY 
    m.serialNumber, 
    m.clientCode;
```

## Segunda pregunta

```sql
SELECT 
    m.serialNumber, 
    m.clientCode, 
    rh.readDate,
    IFNULL(SUM(CASE WHEN rd.obisId = 20 THEN rd.value END), '-') AS `20`,
    IFNULL(SUM(CASE WHEN rd.obisId = 21 THEN rd.value END), '-') AS `21`,
    IFNULL(SUM(CASE WHEN rd.obisId = 22 THEN rd.value END), '-') AS `22`
FROM 
    meter m
INNER JOIN 
    registerHeader rh ON m.meterId = rh.meterId
INNER JOIN 
    registerDetail rd ON rh.registerId = rd.registerId
WHERE 
    rh.type = 'I' 
    AND rd.obisId IN (20, 21, 22)
GROUP BY 
    m.serialNumber, 
    m.clientCode, 
    rh.readDate
ORDER BY 
    m.serialNumber, rh.readDate;
```

## Tercera pregunta

```sql
SELECT 
    m.serialNumber, 
    m.clientCode, 
    rh.readDate,
    IFNULL(SUM(CASE WHEN rd.obisId = 20 THEN rd.value END), '-') AS `20`,
    IFNULL(SUM(CASE WHEN rd.obisId = 21 THEN rd.value END), '-') AS `21`,
    IFNULL(SUM(CASE WHEN rd.obisId = 22 THEN rd.value END), '-') AS `22`
FROM 
    meter m
INNER JOIN 
    registerHeader rh ON m.meterId = rh.meterId
INNER JOIN 
    registerDetail rd ON rh.registerId = rd.registerId
WHERE 
    rh.type = 'I' 
    AND rd.obisId IN (20, 21, 22)
GROUP BY 
    m.serialNumber, 
    m.clientCode, 
    rh.readDate
ORDER BY 
    m.serialNumber, rh.readDate;
```

## Cuarta pregunta

La razón por la cual `registerId` en `registerHeader` es `IDENTITY` y no en `registerDetail` es porque en `registerHeader` cada registro necesita un identificador único auto-generado, mientras que en `registerDetail`, `registerId` se usa como clave foránea para asociar detalles con un registro de cabecera, sin necesidad de un identificador único por fila.

## Quinta pregunta

**Uso de índices adicionales:** Aunque ya existe una clave primaria, se podrían añadir índices en las columnas que se usan con frecuencia para filtros o uniones, como `meterId` en `registerHeader`, `obisId` en `registerDetail`, y `readDate`. Esto ayudaría a mejorar el rendimiento de las consultas que involucran estas columnas.

**Normalización de datos (tipos de valores en `registerDetail`):** El campo `value` en `registerDetail` está definido como `varchar(18)`, lo cual no es ideal para almacenar valores numéricos. Sería más eficiente usar un tipo como `DECIMAL` o `FLOAT`, lo que no solo optimiza el espacio, sino que también mejora la precisión en las consultas.

**Eliminación de la tabla `registerHeader`:** Si la información de la cabecera (como `readDate`, `meterId` y `type`) se repite mucho, podría ser útil combinar la tabla `registerHeader` con `registerDetail`. Esto eliminaría la necesidad de hacer tantas uniones entre las tablas y simplificaría las consultas.

**Desnormalización de `registerDetail`:** Usar `varchar` para almacenar el `obisId` en `registerDetail` podría estar generando un sobrecosto en almacenamiento y en las consultas. Sería más eficiente cambiar este campo a una clave foránea que apunte directamente a la tabla `obis`, siempre y cuando no se esté utilizando como un campo calculado.

**Uso de `INNER JOIN` en lugar de `LEFT JOIN`:** Si no se esperan valores nulos en las uniones, cambiar de `LEFT JOIN` a `INNER JOIN` puede mejorar el rendimiento. Esto es porque el `INNER JOIN` solo devuelve los registros coincidentes, lo que elimina la necesidad de verificar los nulos y hace que las consultas se ejecuten más rápido.


## Parte 2: Frontend

### Primera pregunta

1. **Habilitar el reordenamiento con jQuery UI**  
   Usaría el widget `sortable` de jQuery UI, que permite arrastrar y soltar elementos dentro de una lista o tabla para cambiar su orden.

2. **Agregar un identificador único para las filas**  
   Para poder rastrear el orden de los elementos, es importante que cada fila tenga un identificador único. Esto podría lograrse añadiendo un atributo `data-id` en cada fila, basado en el identificador del parámetro.

3. **Estilizar la tabla para una mejor experiencia**  
   Para mejorar la experiencia del usuario, podrías añadir un ícono de arrastre junto a cada fila. Esto podría lograrse con CSS o añadiendo un elemento visual en las celdas.

### Segunda pregunta

Sí, es posible ocultar una columna sin hacer una nueva consulta a la base de datos. Esto se puede lograr manipulando la grilla con JavaScript o jQuery. Básicamente, al desmarcar un parámetro disponible, solo sería necesario identificar qué columna corresponde a ese parámetro y ocultarla directamente en el DOM.

Para esto, se asocia cada checkbox con una columna específica, y cuando el usuario interactúa con el checkbox, se muestra u oculta la columna según corresponda. Esto funciona bien si todas las columnas ya están cargadas al renderizar la grilla.

Una cosa a tener en cuenta es que la grilla debe estar configurada para soportar esta interacción, y ocultar columnas dinámicamente puede ser algo más costoso en rendimiento si se trabaja con una gran cantidad de datos. Si usas algo como jqGrid, puedes aprovechar funciones integradas como `hideCol` para optimizar la implementación.

### Tercera pregunta

Es posible. Esto se puede hacer mediante una petición AJAX al servidor, enviando el identificador del nuevo parámetro seleccionado. El servidor devolvería únicamente los datos correspondientes a esa columna, y con JavaScript, se podría actualizar la grilla añadiendo esa columna y rellenándola con los valores recibidos.

Una alternativa a esto sería cargar todos los datos posibles desde el inicio, pero mantener las columnas ocultas hasta que el usuario las seleccione. Esto evita hacer peticiones adicionales al servidor, pero puede impactar en el rendimiento si el dataset es muy grande. La elección dependerá del balance entre rendimiento y flexibilidad requerido por la aplicación.

### Cuarta pregunta

Para automatizar la tarea, se podría implementar una funcionalidad de "favoritos", "configuraciones guardadas" o "historial". Esto permitiría al usuario guardar los parámetros que selecciona con frecuencia (por ejemplo, los 3 parámetros) como una configuración predefinida. Luego, al abrir el reporte, podría cargar esa configuración con un solo clic.

Otra opción sería recordar las últimas selecciones del usuario utilizando almacenamiento local en el navegador o en el servidor, dependiendo de si se quiere mantener la configuración entre dispositivos. De esta forma, cada vez que acceda al reporte, los parámetros más usados se cargarían automáticamente.

### Bibliografias
- https://api.jquery.com
- https://www.w3schools.com/jquery/
