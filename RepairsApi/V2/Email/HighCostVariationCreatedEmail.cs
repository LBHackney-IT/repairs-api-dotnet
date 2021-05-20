namespace RepairsApi.V2.Email
{
    public class HighCostVariationCreatedEmail : EmailRequest
    {
        public HighCostVariationCreatedEmail(string address, int id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
