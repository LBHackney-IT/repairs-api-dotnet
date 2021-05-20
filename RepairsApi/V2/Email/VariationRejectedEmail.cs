namespace RepairsApi.V2.Email
{
    public class VariationRejectedEmail : EmailRequest
    {
        public VariationRejectedEmail(string address, int id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
