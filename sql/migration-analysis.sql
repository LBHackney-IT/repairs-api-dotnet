--CREATE EXTENSION pgcrypto;
--select count(*) from public.job_status_updates --367 -> 367
--select count(*) from public.work_elements --561 -> 562
--select count(*) from public.work_orders --379 -> 380

--with corequery as (
--select 'FRONTEND -->' As SourceType, wo1.* from public.work_orders wo1 where id in ('10000380','10000362')
--union 
--select 'MIGRATION -->' As SourceType, wo2.* from public.work_orders wo2 where id in ('1000022')
--) select * from corequery order by SourceType

--select * from public.work_orders where id = '1000022'
--select * from public.work_elements where work_order_id = '1000021'