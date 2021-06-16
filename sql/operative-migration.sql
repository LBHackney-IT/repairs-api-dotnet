BEGIN;

do $$
declare
   operative_record RECORD;
   operative_id integer;
   archived boolean;
begin
	FOR operative_record IN SELECT * FROM dbo.drs_operative_dump
	LOOP
		IF operative_record."NumberOfResources" > 0 THEN
			archived := false;
		ELSE
			archived := true;
		END IF;
		INSERT INTO public.operatives (payroll_number, is_archived, name, resource_id)
			VALUES (operative_record."ExternalResourceCode", archived, operative_record."ResourceName", operative_record."ResourceId")
			RETURNING id INTO operative_id;
	end loop;
end; $$
;

COMMIT;