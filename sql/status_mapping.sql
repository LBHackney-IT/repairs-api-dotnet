-- FUNCTION: dbo.map_legacy_status(character varying)

-- DROP FUNCTION dbo.map_legacy_status(character varying);

CREATE OR REPLACE FUNCTION dbo.map_legacy_status(
	legacy_status character varying)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN
	CASE legacy_status
		when '100' then
			return 80;
		when '200' then -- locked
			return 200;
		when '300' then
			return 40;
		when '400' then
			return 70;
		when '425' then
			return 70;
		when '500' then
			return 40;
		when '600' then
			return 40;
		when '700' then
			return 30;
		when '900' then
			return 40;
		ELSE
      --  do nothing
	END CASE;
END;
$BODY$;

ALTER FUNCTION dbo.map_legacy_status(character varying)
    OWNER TO repairs_admin;
