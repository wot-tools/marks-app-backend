CREATE TABLE events (number INT UNSIGNED, guid VARCHAR(36), occured DATETIME, recorded DATETIME, data VARCHAR(2048), CHECK(JSON_VALID(data)));

ALTER TABLE events ADD mark_level tinyint unsigned AS (JSON_VALUE(data, '$.Level'));
ALTER TABLE events ADD player_id integer unsigned AS (JSON_VALUE(data, '$.ID'));
