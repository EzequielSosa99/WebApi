# Respuestas a las preguntas

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
    AND rd.obisId IN (20, 21, 22)  -- Esto se puede ajustar dependiendo de lo que reciba el parámetro
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

### Uso de índices adicionales

Aunque ya existe una clave primaria, se podrían añadir índices en las columnas que se usan con frecuencia para filtros o uniones, como `meterId` en `registerHeader`, `obisId` en `registerDetail`, y `readDate`. Esto ayudaría a mejorar el rendimiento de las consultas que involucran estas columnas.

### Normalización de datos (tipos de valores en `registerDetail`)

El campo `value` en `registerDetail` está definido como `varchar(18)`, lo cual no es ideal para almacenar valores numéricos. Sería más eficiente usar un tipo como `DECIMAL` o `FLOAT`, lo que no solo optimiza el espacio, sino que también mejora la precisión en las consultas.

### Eliminación de la tabla `registerHeader`

Si la información de la cabecera (como `readDate`, `meterId` y `type`) se repite mucho, podría ser útil combinar la tabla `registerHeader` con `registerDetail`. Esto eliminaría la necesidad de hacer tantas uniones entre las tablas y simplificaría las consultas.

### Desnormalización de `registerDetail`

Usar `varchar` para almacenar el `obisId` en `registerDetail` podría estar generando un sobrecosto en almacenamiento y en las consultas. Sería más eficiente cambiar este campo a una clave foránea que apunte directamente a la tabla `obis`, siempre y cuando no se esté utilizando como un campo calculado.

### Uso de `INNER JOIN` en lugar de `LEFT JOIN`

Si no se esperan valores nulos en las uniones, cambiar de `LEFT JOIN` a `INNER JOIN` puede mejorar el rendimiento. Esto es porque el `INNER JOIN` solo devuelve los registros coincidentes, lo que elimina la necesidad de verificar los nulos y hace que las consultas se ejecuten más rápido.
