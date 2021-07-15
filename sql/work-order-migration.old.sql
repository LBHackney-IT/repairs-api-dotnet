-- PROCEDURE: dbo.migrate_work_orders()

-- DROP PROCEDURE dbo.migrate_work_orders();

CREATE OR REPLACE PROCEDURE dbo.migrate_work_orders(
	amount integer
	)

LANGUAGE 'plpgsql'

AS $BODY$
DECLARE wo_record RECORD;
DECLARE task_record RECORD;
DECLARE note_record RECORD;
DECLARE site_id integer;
DECLARE address_id integer;
DECLARE property_id integer;
DECLARE organization_id integer;
DECLARE party_id integer;
DECLARE work_order_id integer;
DECLARE work_element_id uuid;
DECLARE person_id integer;
DECLARE customer_id integer;
BEGIN
-- Select priority records from UH
 INSERT into public.sor_priorities(description, days_to_complete, enabled, priority_character)
    SELECT desc_tion as description, days_to_issue as days_to_complete, false as enabled, priority as priority_character
    FROM dbo.rmprior_uht_data_restore
    WHERE priority NOT IN ('I','E','U','N','L2','L3','P1','P2','P3'); -- we either already have these priorities or they're not used in UH

-- Select work orders from UH NOTE: there won't necessarily be a single work order record in this result set.
-- requests could have multiple Work orders which could have multiple tasks - also, LEFT joined on users in case they didn't exist otherwise the
-- record wouldn't be returned.
	FOR wo_record IN
		SELECT
		wo.wo_ref,
		req.prop_ref,
		req.rq_problem,
		wo.sup_ref,
		dbo.map_priority(req.rq_priority),--uses function to map priority
		req.rq_date,
		wo.completed,
		dbo.map_legacy_status(wo.wo_status),--uses function to map status
		req.rq_name as client_name,
		req.rq_phone as client_phone,
		req.rq_date_due,
		prop.short_address,
    prop.post_code,
		sup.sup_name,
		trade.trade,
		trade.trade_desc,
		uhw_user."User_Name" as agent_name,
		uhw_user."EMail" as agent_email
		FROM 
			dbo.rmreqst_uht_data_restore req
		INNER JOIN 
			dbo.rmworder_uht_data_restore wo ON wo.rq_ref = req.rq_ref
		INNER JOIN 
			dbo.property_uht_data_restore prop ON prop.prop_ref = req.prop_ref
		INNER JOIN
			dbo.supplier_uht_data_restore sup ON wo.sup_ref = sup.sup_ref
		INNER JOIN
			dbo.rmtask_uht_data_restore task ON task.wo_ref = wo.wo_ref
		INNER JOIN
			dbo.rmtrade_uht_data_restore trade ON trade.trade = task.trade
		LEFT JOIN
			dbo.auser_uht_data_restore uht_user ON uht_user.user_code = wo.user_code
		LEFT JOIN
			dbo."W2User_uhw_data_restore" uhw_user ON uht_user.user_login = uhw_user."User_ID"
		WHERE task.task_no = 1
		LIMIT amount
	LOOP
		INSERT INTO public.site DEFAULT VALUES RETURNING id INTO site_id;

		INSERT INTO public.property_address (address_line, postal_code) 
            VALUES (wo_record.short_address, wo_record.post_code) RETURNING id INTO address_id;

		INSERT INTO public.property_class (site_id, address_id, property_reference) 
			VALUES (site_id, address_id, wo_record.prop_ref) 
			RETURNING id INTO property_id;

		INSERT INTO public.organization DEFAULT VALUES RETURNING id INTO organization_id;

		INSERT INTO public.party (name, organization_id, contractor_reference) 
			VALUES (wo_record.sup_name, organization_id, wo_record.sup_ref) 
			RETURNING id INTO party_id;

		INSERT INTO public.person (name_full) VALUES (wo_record.client_name) RETURNING id INTO person_id;

		INSERT INTO public.party (name, person_id) 
			VALUES (wo_record.client_name, person_id) 
			RETURNING id INTO customer_id;


		INSERT INTO public.work_orders (description_of_work, 
										site_id, 
										work_class_work_class_code, 
										date_raised, 
										assigned_to_primary_id, 
										customer_id, 
										instructed_by_id,
										agent_name,
                    work_priority_priority_code,
										status_code)
			VALUES (wo_record.rq_problem, site_id, 0, wo_record.rq_date, party_id, customer_id, NULL, wo_record.agent_name,map_priority,map_legacy_status) 
			RETURNING id INTO work_order_id;
      
		INSERT INTO public.work_elements (work_order_id) VALUES (work_order_id) RETURNING id INTO work_element_id;

		INSERT INTO public.trade (code, custom_code, custom_name, work_element_id) 
			VALUES (46, wo_record.trade, wo_record.trade_desc, work_element_id);

		-- TODO Create Completion
		
		-- TODO Mark as original based on variation
		INSERT INTO public.rate_schedule_items (custom_code, 
												custom_name, 
												quantity_amount, 
												work_element_id, 
												original, 
												original_quantity, 
												date_created,
											    code_cost)
			SELECT job.job_code, job.short_desc, task.est_units, work_element_id, false, NULL, task.created,
			CASE 
				WHEN cont.cost IS NOT NULL THEN cont.cost
				WHEN cst.cost IS NOT NULL THEN cst.cost
				ELSE 0
			END
			FROM 
				dbo.rmtask_uht_data_restore task
			INNER JOIN
				dbo.rmjob_uht_data_restore job ON job.job_code = task.job_code
			LEFT JOIN ( -- Getting most recent cost
				SELECT innerCst.*
				FROM dbo.rmjobcst_uht_data_restore innerCst
				INNER JOIN dbo.rmjob_uht_data_restore innerJob ON innerCst.job_code = innerJob.job_code
				ORDER BY eff_date DESC 
				LIMIT 1
			) cst ON cst.job_code = job.job_code
			LEFT JOIN 
				dbo.rmcontdt_uht_data_restore cont ON job.job_code = cont.job_code AND cont.rc_ref = task.sor_contract
			WHERE 
				task.wo_ref = wo_record.wo_ref;

	-- Select notes for work order id and task and request
  --select "KeyObject","KeyNumb", "KeyText", "NDate", "NoteText", "NoteID", note."UserID",
		    --  uhw_user."User_Name" as agent_name,
		    --  uhw_user."EMail" as agent_email
      --from dbo."W2ObjectNote_uhw_data_restore" note
      --left JOIN dbo."W2User_uhw_data_restore" uhw_user ON uhw_user."User_ID" = note."UserID"
      --where 
      --("KeyObject" = 'UHRepairsTask' AND "KeyNumb" = wo_record.rmtask_sid)
      --OR
      --("KeyObject" = 'UHOrder' AND "KeyNumb" = wo_record.rmworder_sid)
      --OR
      --("KeyObject" = 'UHRepair' AND "KeyNumb" = wo_record.rmreqst_sid)
	------FOREACH note
	-- Create Updates
	--------ENDFOR note
	END LOOP;
COMMIT;
END
$BODY$;
