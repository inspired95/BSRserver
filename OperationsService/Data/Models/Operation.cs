using System;
using System.ComponentModel.DataAnnotations;

namespace OperationsService.Data.Models
{
    public class Operation
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
        
        [Required] 
        public Double Amount { get; set; }
        
        [Required] 
        public String Description { get; set; }
        
        [Required] 
        public BankEnum Bank { get; set; }
        
        [Required] 
        public OperationType Type { get; set; }

        public enum BankEnum
        {
            PkoBp
        }

        public enum OperationType
        {
            IncomeTransfer, 
            OutgoingTransfer, 
            DebitCardPayment, 
            MobileCodePayment, 
            CashWithdrawal, 
            Commission, 
            NotResolved, 
            Refund, 
            StandingOrder, 
            NotApplicable, 
            NotFound
        }
    }
}
