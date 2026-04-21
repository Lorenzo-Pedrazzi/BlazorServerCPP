CREATE TABLE IF NOT EXISTS utenti (
    id                SERIAL PRIMARY KEY,
    username          VARCHAR(50)  NOT NULL UNIQUE,
    email             VARCHAR(255) NOT NULL UNIQUE,
    password_hash     VARCHAR(255) NOT NULL,
    ruolo             VARCHAR(20)  NOT NULL DEFAULT 'user'
                      CHECK (ruolo IN ('admin', 'user', 'installatore')),
    data_creazione    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    data_ultimo_login TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS ix_utenti_email    ON utenti (email);
CREATE INDEX IF NOT EXISTS ix_utenti_username ON utenti (username);
