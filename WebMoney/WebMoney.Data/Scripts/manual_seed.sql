-- 1) Migration. Enums are populated and kept in DB as string
-- 2) dotnet run --project WebMoney.Tools.HashPasswords 

BEGIN;

INSERT INTO counterparties (name, created_at, created_by)
VALUES
  ('Контрагент 1', now(), 'seed'),
  ('Контрагент 2', now(), 'seed');

INSERT INTO users (user_name, email, hashed_password, role, created_at, created_by)
VALUES (
  'test',
  'test@test.com',
  'AQAAAAIAAYagAAAAED4JovDDvgc1UNyHKPZj6b2gKaEwj3zZuQQ9SQUP9h1zOH0bDQwXYU73amWoooEPYg==',
  'User',
  now(),
  'seed'
);

INSERT INTO users_profiles (user_id, identity_document_id, created_at, created_by)
VALUES (
  (SELECT id FROM users WHERE email = 'test@test.com'),
  NULL,
  now(),
  'seed'
);

INSERT INTO card_limits (daily_limit, monthly_limit, per_operation_limit, created_at, created_by)
VALUES (5000, 50000, 2000, now(), 'seed');

INSERT INTO cards (
  number, currency_code, card_status, hashed_pin_code,
  period_of_validity, balance, created_at, created_by
)
VALUES (
  '4111111111111111',
  'BYN',
  'Active',
  'AQAAAAIAAYagAAAAEOgawh9YzeiJ+D6+KKCTdUvqEPYfh+1+ieGNB2rA4gVaUikI/9i7P3+u+vvlVFxgqA==',
  (current_date + interval '5 years')::date,
  10000,
  now(),
  'seed'
);

INSERT INTO card_user_profiles (card_id, user_id, card_limit_id, created_at, created_by)
VALUES (
  (SELECT id FROM cards WHERE number = '4111111111111111' ORDER BY id DESC LIMIT 1),
  (SELECT id FROM users WHERE email = 'test@test.com'),
  (SELECT id FROM card_limits ORDER BY id DESC LIMIT 1),
  now(),
  'seed'
);

INSERT INTO transactions (
  card_id, transaction_type, transaction_status, counterparty_id,
  amount, created_at, created_by
)
VALUES
  (
    (SELECT id FROM cards WHERE number = '4111111111111111' ORDER BY id DESC LIMIT 1),
    'Deposit',
    'Completed',
    (SELECT id FROM counterparties ORDER BY id LIMIT 1),
    500,
    now() - interval '30 days',
    'seed'
  ),
  (
    (SELECT id FROM cards WHERE number = '4111111111111111' ORDER BY id DESC LIMIT 1),
    'Withdrawal',
    'Completed',
    (SELECT id FROM counterparties ORDER BY id LIMIT 1 OFFSET 1),
    100,
    now() - interval '10 days',
    'seed'
  );

UPDATE cards
SET balance = 10000 + 500 - 100,
    updated_at = now(),
    updated_by = 'seed'
WHERE number = '4111111111111111';

COMMIT;
