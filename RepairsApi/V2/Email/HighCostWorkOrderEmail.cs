namespace RepairsApi.V2.Email
{
    public class HighCostWorkOrderEmail : EmailRequest
    {
        public HighCostWorkOrderEmail(string address, int id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
