CREATE TABLE events (
    number BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    guid VARCHAR(36),
    occured DATETIME,
    recorded DATETIME,
    data LONGTEXT,
    PRIMARY KEY (number),
    CHECK(JSON_VALID(data))
);

ALTER TABLE events ADD mark_level TINYINT UNSIGNED AS (JSON_VALUE(data, '$.Level'));
ALTER TABLE events ADD player_id INTEGER UNSIGNED AS (JSON_VALUE(data, '$.ID'));
