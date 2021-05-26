SELECT sc.sor_code_code, c.contractor_reference, COUNT (*) FROM 
public.sor_contracts sc
INNER JOIN public.contracts c ON sc.contract_reference = c.contract_reference
WHERE 
sc.sor_code_code IN (SELECT code FROM public.sor_codes WHERE enabled = true)
AND sc.contract_reference IN (select contract_reference FROM public.contracts WHERE termination_date > NOW() AND effective_date < NOW())
GROUP BY sc.sor_code_code,
c.contractor_reference
HAVING COUNT(*) > 1