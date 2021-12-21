using CustomerAccountDeletionRequest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerAccountDeletionRequestTests.Data
{
    public class DeletionRequestModelObjects
    {
        public DeletionRequestModelObjects(){}

        /// <summary>
        /// Used to provide a list of test create objects.
        /// Array Args 1: CustomerID - Any Int32 > 0
        /// Array Args 2: DeletionReason - string not null
        /// Array Args 3: DateRequested - DateTime
        /// Array Args 4: DateApproved - DateTime
        /// Array Args 5: StaffID - Any Int32 > 0
        /// Array Args 6: DeletionRequestStatus - DeletionRequestStatusEnum
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Object[]> GetDeletionRequestModelCreateObjects()
        {
            return new List<Object[]>
            {
                new object[] { 1, "Prefer Amazon.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.AwaitingDecision },
                new object[] { 2, "Prefer Ebay.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.AwaitingDecision },
                new object[] { 3, "Nothing gets delivered.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.AwaitingDecision },
                new object[] { 4, "App broken.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.AwaitingDecision },
                new object[] { 5, "Scam.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.AwaitingDecision }
            };
        }

        /// <summary>
        /// Used to provide a list of test update objects.
        /// Array Args 1: CustomerID - Any Int32 > 0
        /// Array Args 2: DeletionReason - string not null
        /// Array Args 3: DateRequested - DateTime
        /// Array Args 4: DateApproved - DateTime
        /// Array Args 5: StaffID - Any Int32 > 0
        /// Array Args 6: DeletionRequestStatus - DeletionRequestStatusEnum
        /// Array Args 7: DeletionRequestID - Any Int32 > 0
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Object[]> GetDeletionRequestModelUpdateObjects()
        {
            return new List<Object[]>
            {
                new object[] { 1, "Prefer Amazon.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.Approved, 1 },
                new object[] { 2, "Prefer Ebay.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.Approved, 2 },
                new object[] { 3, "Nothing gets delivered.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.Approved, 3 },
                new object[] { 4, "App broken.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.Approved, 4 },
                new object[] { 5, "Scam.", DateTime.Now, DateTime.MinValue, 0, DeletionRequestStatusEnum.Approved, 5 }
            };
        }
    }
}
