-- PROCEDURE: dbo.migrate_contracts()

-- DROP PROCEDURE dbo.migrate_contracts();

CREATE OR REPLACE PROCEDURE dbo.migrate_contracts(
	)
LANGUAGE 'sql'
AS $BODY$

ALTER TABLE public.available_appointments DROP CONSTRAINT fk_available_appointments_contractors_contractor_reference;

TRUNCATE public.contractors
,public.contracts
,public.property_contracts
,public.sor_codes
,public.sor_contracts
,public.sor_priorities
,public.trades;

/*
CREATE TABLE priority_map AS
SELECT * FROM (VALUES 
	('I', 1, true),
	('E', 2, true),
	('U', 3, true),
	('N', 4, true)) 
as t (uh_code, repair_code, enabled);
*/
INSERT INTO public.contractors (reference, "name")
SELECT sup_ref, sup_name FROM dbo.supplier_uht_data_restore;

INSERT INTO public.trades (code, "name")
SELECT trade, trade_desc FROM dbo.rmtrade_uht_data_restore;

INSERT INTO public.contracts (contract_reference, termination_date, effective_date, contractor_reference)
SELECT rc_ref, rc_term, rc_eff, sup_ref FROM dbo.rmcontct_uht_data_restore
WHERE sup_ref IN (SELECT reference FROM public.contractors);

INSERT INTO public.sor_priorities (priority_code, description, days_to_complete, enabled) 
SELECT m.repair_code, pri.desc_tion, pri.days_to_complete, m.enabled FROM 
dbo.rmprior_uht_data_restore pri
INNER JOIN priority_map m ON m.uh_code = pri.priority;

INSERT INTO public.sor_codes (code, short_description, long_description, 
								"cost", --PROBLEM: missing rows in jobcst 
								trade_code, 
								priority_id,
								enabled)
SELECT 
DISTINCT -- PROBLEM duplicate jobs in UH
sor.job_code, 
sor.short_desc,
sor.full_desc, 
cst.cost, --PROBLEM: missing rows in jobcst
sor.trade, 
m.repair_code,
	CASE WHEN sor.job_code IN (SELECT code FROM dbo.sor_code_whitelist) THEN true ELSE false END
FROM dbo.rmjob_uht_data_restore sor
INNER JOIN priority_map m ON m.uh_code = sor.def_prior
LEFT JOIN ( -- Getting most recent cost
	SELECT innerCst.*
	FROM dbo.rmjobcst_uht_data_restore innerCst
	INNER JOIN dbo.rmjob_uht_data_restore innerJob ON innerCst.job_code = innerJob.job_code
	ORDER BY eff_date DESC 
	LIMIT 1
) cst ON cst.job_code = sor.job_code
WHERE sor.trade IN (SELECT code FROM public.trades);

INSERT INTO public.sor_contracts (sor_code_code, contract_reference, "cost")
SELECT 
DISTINCT --PROBLEM duplicate rows in contdt probably related to duplicate rmjobs
job_code, rc_ref, "cost" FROM dbo.rmcontdt_uht_data_restore
WHERE rc_ref IN (SELECT contract_reference FROM public.contracts) 
AND job_code IN (SELECT code FROM public.sor_codes)
AND sysgen=false;

-- PROBLEM: Takes Too Long
INSERT INTO public.property_contracts (prop_ref, contract_reference) 
SELECT prop_ref, rc_ref FROM dbo.rmcontprop_uht_data_restore
WHERE rc_ref IN (SELECT contract_reference FROM public.contracts);

ALTER TABLE public.available_appointments
    ADD CONSTRAINT fk_available_appointments_contractors_contractor_reference FOREIGN KEY (contractor_reference)
    REFERENCES public.contractors (reference) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE RESTRICT;
$BODY$;
