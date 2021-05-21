namespace RepairsApi.V2.Email
{
    public class WorkRejectedEmail : EmailRequest
    {
        public WorkRejectedEmail(string address, object id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
