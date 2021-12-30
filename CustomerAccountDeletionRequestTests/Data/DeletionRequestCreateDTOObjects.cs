using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerAccountDeletionRequestTests.Data
{
    /// <summary>
    /// Class to serve as a data provider for Unit Tests.
    /// </summary>
    public class DeletionRequestCreateDTOObjects
    {
        public DeletionRequestCreateDTOObjects(){}

        /// <summary>
        /// Used to provide a list of test objects.
        /// </summary>
        /// <returns>
        /// Array Args 1: CustomerID - Any Int32 > 0
        /// Array Args 2: DeletionRequest - string not null
        /// </returns>
        public static IEnumerable<object[]> GetDeletionRequestCreateDTOObjects()
        {
            return new List<Object[]>
            {
                new object[] { 6, "TEST Deleting my account." },
                new object[] { 6, "TEST Horrendous Store." },
                new object[] { 8, "TEST Prefer brick over click." },
                new object[] { 10, "TEST Too buggy." },
                new object[] { 25, "TEST Just found Wish." }
            };
        }
    }
}
