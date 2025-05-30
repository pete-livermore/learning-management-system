IF NOT EXISTS (
  SELECT
    name
  FROM
    sys.databases
  WHERE
    name = 'learning_management_system'
) BEGIN CREATE DATABASE learning_management_system;

END