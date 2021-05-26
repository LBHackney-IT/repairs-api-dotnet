-- FUNCTION: dbo.map_priority(character varying)

-- DROP FUNCTION dbo.map_priority(character varying);

CREATE OR REPLACE FUNCTION dbo.map_priority(
	legacy_priority character varying)
    RETURNS integer
    LANGUAGE 'sql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
select priority_code from public.sor_priorities
		where priority_character = legacy_priority;
$BODY$;

ALTER FUNCTION dbo.map_priority(character varying)
    OWNER TO repairs_admin;
